using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition.Interactions;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Ai;
using ColonizingBastards.GameClasses.Match.MatchObjects.Animals;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using ColonizingBastards.GameClasses.Match.MatchObjects.Items;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.MatchSound;
using ColonizingBastards.GameClasses.Match.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.GameEvent
{

	class TreasureDropEvent : GameEvent
	{

		// DESCRIPTION:
		// Creates a new treasure at the given location, initially inactive, active after execute
		// ACTUATOR:
		// (List of) character(s) cutting the grass
		// TARGET:
		// none

		private Treasure treasure;

		private RenderObject rep;

		private Scene scene;

		public TreasureDropEvent(Random random, Vector3 location, Scene scene) : base(false, true)
		{
		    int treasureType = random.Next(ItemsConfig.LOOT_TEXTURE.Count);
			rep = new Sprite(ItemsConfig.LOOT_TEXTURE[treasureType]);
			this.treasure = new Treasure(rep);
			this.scene = scene;
			treasure.SetPosition(new Vector2(location.X, location.Y - 20));
			treasure.TreasureCollectEventList.EventList.Add(new TreasureCollectEvent(scene, treasure));
			scene.RegisterObject(treasure);
		}

        public TreasureDropEvent(RenderObject rep, Vector3 location, Scene scene) : base(false, true)
        {
            this.rep = rep;
            this.treasure = new Treasure(rep);
            this.scene = scene;
            treasure.SetPosition(new Vector2(location.X, location.Y - 20));
            treasure.TreasureCollectEventList.EventList.Add(new TreasureCollectEvent(scene, treasure));
            scene.RegisterObject(treasure);
        }

        public override void execute(List<object> actuator, List<object> target)
		{
			treasure.Active = true;
		}
	}



	class TreasureCollectEvent : GameEvent
	{

		// DESCRIPTION:
		// Is called when a character attempts to pick up treasure, if the character
		// is not yet carrying treasure, he picks it up, else, nothing happens.
		// ACTUATOR:
		// (List of) character(s) attempting to pick up the treasure
		// TARGET:
		// (List of) actors touched (unused)


		private Scene scene;
		private Treasure treasure;
		
		public TreasureCollectEvent(Scene scene, Treasure treasure) : base(true, true)
		{
			this.scene = scene;
			this.treasure = treasure;
		}

		public override void execute(List<object> actuator, List<object> target)
		{
			foreach (object act in actuator)
			{
				DefaultCharacter currentCharacter;
				
				if (act.GetType().GetTypeInfo().IsSubclassOf(typeof(DefaultCharacter)) || act.GetType() == typeof(DefaultCharacter))
				{
					currentCharacter = (DefaultCharacter) act;
					
					if (currentCharacter.InvTreasure == null && treasure.Active)
					{

						scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Collect);
						currentCharacter.InvTreasure = treasure;
						(currentCharacter.getRepresentation() as Spritesheet)?.SetVisibilityLayer(1, true);
						// Let this event be removed
						Active = false;
						treasure.Active = false;
						scene.RemoveActor(treasure);
					}
									
				}
			}
		}
	}

	class AmmoDropEvent : GameEvent
	{

		// DESCRIPTION:
		// Creates a new ammo crate at the given location, initially inactive, active after execute
		// ACTUATOR:
		// none
		// TARGET:
		// none

		private AmmoCrate ammoCrate;

		private RenderObject rep;

		private Scene scene;


		public AmmoDropEvent(Texture2D texture, Vector3 location, Scene scene) : base(false, true)
		{
			this.rep = new Sprite(texture);
			this.ammoCrate = new AmmoCrate(rep);
			this.scene = scene;
			ammoCrate.SetPosition(new Vector2(location.X, location.Y - 20));
			ammoCrate.AmmoCrateCollectEventList.EventList.Add(new AmmoCrateCollectEvent(scene, ammoCrate));
			scene.RegisterObject(ammoCrate);
		}

		public override void execute(List<object> actuator, List<object> target)
		{
			ammoCrate.Active = true;
		}
	}



	class AmmoCrateCollectEvent : GameEvent
	{

		// DESCRIPTION:
		// Is called when a character picks up an ammo crate
		// ACTUATOR:
		// (List of) character(s) attempting to pick up the treasure
		// TARGET:
		// (List of) actors touched (unused)


		private Scene scene;
		private AmmoCrate ammoCrate;

		public AmmoCrateCollectEvent(Scene scene, AmmoCrate ammoCrate) : base(false, true)
		{
			this.scene = scene;
			this.ammoCrate = ammoCrate;
		}

		public override void execute(List<object> actuator, List<object> target)
		{
			foreach (object act in actuator)
			{
				DefaultCharacter currentCharacter;

				if (act.GetType().GetTypeInfo().IsSubclassOf(typeof(DefaultCharacter)) || act.GetType() == typeof(DefaultCharacter))
				{
					currentCharacter = (DefaultCharacter)act;

					if (ammoCrate.Active)
					{
						scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Collect);

						currentCharacter.AmmoCount = MatchConfig.AmmoCrateContentSize;
						
						ammoCrate.Active = false;
						scene.RemoveActor(ammoCrate);
					}

				}
			}
		}
	}


	class TrapDropEvent : GameEvent
	{

		// DESCRIPTION:
		// Creates a new trap at the given location, initially active and open
		// ACTUATOR:
		// (List of) location as Vector2 (1 item only)
		// TARGET:
		// none

		private Trap trap;

		private RenderObject closedTrapRep, openTrapRep;

		private Scene scene;

		private Character owner;


		public TrapDropEvent(Texture2D closedTexture, Texture2D openTexture, Character owner, Scene scene) : base(false, true)
		{
			this.closedTrapRep = new Sprite(closedTexture);
			this.openTrapRep = new Sprite(openTexture);

			this.owner = owner;

			this.trap = new Trap(closedTrapRep, openTrapRep, owner);
			this.scene = scene;
			
			trap.TrapTouchEventList.EventList.Add(new TrapTouchEvent(scene, trap));
			scene.RegisterObject(trap);
		}

		public override void execute(List<object> actuator, List<object> target)
		{
			if (actuator.Count == 1)
			{
				if (actuator[0] is Vector2)
				{
					Vector2 location = (Vector2) actuator[0];
					trap.SetPosition(new Vector2(location.X, location.Y - 20));

					if (owner is DefaultCharacter)
					{
						DefaultCharacter defaultCharacter = (DefaultCharacter) owner;
						if (defaultCharacter.TrapCount > 0)
						{
							defaultCharacter.TrapCount--;
						}		
					}

					trap.Active = true;
					trap.Loaded = true;
				}
			}
		}
	}



	class TrapTouchEvent : GameEvent
	{

		// DESCRIPTION:
		// Is called when a character other than the owner touches the trap
		// ACTUATOR:
		// (List of) character(s) touching the trap
		// TARGET:
		// (List of) actors touched (unused)


		private Scene scene;
		private Trap trap;

		public TrapTouchEvent(Scene scene, Trap trap) : base(true, true)
		{
			this.scene = scene;
			this.trap = trap;
		}

		public override void execute(List<object> actuator, List<object> target)
		{
			foreach (object act in actuator)
			{
				DefaultCharacter currentCharacter;

				if (act.GetType().GetTypeInfo().IsSubclassOf(typeof(DefaultCharacter)) || act.GetType() == typeof(DefaultCharacter))
				{
					currentCharacter = (DefaultCharacter)act;

					if (trap.Active && trap.Loaded)
					{
						scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Trap);

						trap.Close();

						// Stun the player
					    currentCharacter.CharacterPhysics.CurrentStunTime = ItemsConfig.TRAP_STUN_TIME;
						currentCharacter.CharacterPhysics.IsStunned = true;

						trap.TrapCollectEventList.EventList.Add(new TrapCollectEvent(scene, trap));

						trap.Loaded = false;
					}

				}
			}
		}
	}

	class TrapCollectEvent : GameEvent
	{

		// DESCRIPTION:
		// Enables players to pick up traps again after they activated
		// ACTUATOR:
		// (List of) character(s) attempting to pick up the trap
		// TARGET:
		// (List of) actors touched (unused)

		private Trap trap;

		private Scene scene;


		public TrapCollectEvent(Scene scene, Trap trap) : base(true, true)
		{
			this.trap = trap;
			this.scene = scene;
		}

		public override void execute(List<object> actuator, List<object> target)
		{
			if (actuator.Count == 1)
			{
				if (actuator[0] is DefaultCharacter)
				{
					DefaultCharacter character = (DefaultCharacter) actuator[0];
					if (character.TrapCount == 0)
					{
						character.AddTrap();
						scene.RemoveActor(trap);
						trap.Active = false;
						ActiveAfterExecution = false;

					}
					
				}
			}


		}
	}



	class SpawnAiPossessedCharacterEvent : GameEvent
	{

		// DESCRIPTION:
		// Spawns a new Character at the given location which will be possessed by a new default Ai Controller.
		// ACTUATOR:
		// none
		// TARGET:
		// none

		private Character character;

		private AiController controller;
		
		private Scene scene;

		public SpawnAiPossessedCharacterEvent(Character character, Scene scene) : base(false, true)
		{
			this.controller = new AiIndigenController(scene, scene.NavigationGraph, new AiBehavior());
			this.character = character;
			this.scene = scene;
		}
		

		public override void execute(List<object> actuator, List<object> target)
		{
			controller.Possess(character);
			scene.Characters.Add(character);
			scene.GetActors().Add(character);
			scene.AddAiController(controller);

		    Vector3 position = character.GetCenterPosition();
		    Vector3 size = character.GetSize();
			//Show grass particle effects when the character appears
			FoliageCutParticleEffectFactory foliageCutFactory =
				new FoliageCutParticleEffectFactory(new Vector2(position.X, position.Y + 20), (int)size.Length(), 600, new Vector2(0.8f, 1.3f), new Vector2(-0.5f, -1.5f)/*Vector2.Zero*/, 1f, 0f, Color.ForestGreen, Color.Brown, 400, rotVel: 0.75f);
			ParticleEffectManager foliageCutEffectManager = new ParticleEffectManager(ParticleConfig.FOLIAGE_CUT_TEXTURE, foliageCutFactory, 30, 200, false, BlendState.NonPremultiplied, effectDuration: 600);
			scene.ParticleEffects.Add(foliageCutEffectManager);
        }
	}




	class SpawnSnakeEvent : GameEvent
	{

		// DESCRIPTION:
		// Activates the given snake at the given location.
		// ACTUATOR:
		// (List of) character(s) cutting the grass
		// TARGET:
		// none

		private Snake snake;

		private Scene scene;

		public SpawnSnakeEvent(Snake snake, Scene scene) : base(false, true)
		{
			this.snake = snake;
			this.scene = scene;
		}


		public override void execute(List<object> actuator, List<object> target)
		{

			if (actuator.Count == 1)
			{
				if (actuator[0] is DefaultCharacter)
				{
					DefaultCharacter character = (DefaultCharacter)actuator[0];

					snake.Active = true;
					

					snake.Appear(character.CharacterPhysics.GetPosition().X > snake.getCenterPointX());

					character.CharacterPhysics.CurrentStunTime = CharacterConfig.SNAKE_STUN_TIME;
					character.CharacterPhysics.IsStunned = true;

				}
			}




		}
	}


}
