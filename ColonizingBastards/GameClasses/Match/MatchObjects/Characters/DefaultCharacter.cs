using ColonizingBastards.Base.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.SoundUtil;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.GameEvent;
using ColonizingBastards.GameClasses.Match.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Ai;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters.Skills;
using ColonizingBastards.GameClasses.Match.MatchObjects.Items;
using ColonizingBastards.GameClasses.Match.MatchObjects.Projectiles;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.MatchSound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Characters
{
	class DefaultCharacter : Character
	{
	 
		public string CHARACTER_NAME { get; set; }
		public string CHARACTER_DESCRIPTION { get; set; }

		// Inventory (simple)
		public Item InvTreasure { get; set; }
		public uint AmmoCount { get; set; }
		public uint TrapCount { get; set; }

		// Skills
		private SkillShoot skillShoot;
	    private SkillCut skillCut;
	    private SkillCollect skillCollect;
		private SkillTrap skillTrap;

		private TimeSpan SkillCollectButtonTimeSpan = TimeSpan.MaxValue;

		public CharacterPhysics CharacterPhysics { get; protected set; }
	    private Scene scene;
	    public GameEventList characterTreasureLostEventList;

		public GameEventList TrapDropEventList;

	    public Rectangle HitboxOffset { get;}

        public float currentAnimDuration { get; private set; }

        public float currentAnimRestartCounter { get; set; }

        public DefaultCharacter(RenderObject rep, Scene scene, Rectangle hitboxOffset) : base(rep)
		{
		    this.scene = scene;
		    this.HitboxOffset = hitboxOffset;
		}

	    public override void InitCharacter(Vector3 position)
	    {
	        var playerIndex = ((Player)possessedBy)?.GetPlayerIndex();
	        if (playerIndex != null)
	        {
	            Rectangle characterStartPos = scene.CharacterStartPos[(int)playerIndex];
	            SetSize(new Vector3(characterStartPos.Width, characterStartPos.Height, 0f));
	        }
	        else
	        {
				// i.e. for AI
		        if (rep is Spritesheet)
		        {
			        Spritesheet repSpritesheet = (Spritesheet) rep;
					rep.SetSize(new Vector3(repSpritesheet.SpriteWidth, repSpritesheet.SpriteHeight, 0f));
					SetSize(new Vector3(repSpritesheet.SpriteWidth, repSpritesheet.SpriteHeight, 0f));
		        }
	        }

			SetPosition(position);

			CharacterPhysics = new CharacterPhysics(this, position, size, HitboxOffset, scene);
			
		    (rep as Spritesheet)?.SetVisibilityLayer(1, false);

            skillShoot = new SkillShoot(scene, this, CharacterPhysics, CharacterConfig.SKILL_COOLDOWN_SHOOT);
            skillCut = new SkillCut(scene, this, CharacterPhysics, CharacterConfig.SKILL_COOLDOWN_CUT);
            skillCollect = new SkillCollect(scene, this, CharacterPhysics, CharacterConfig.SKILL_COOLDOWN_COLLECT);
			skillTrap = new SkillTrap(scene, this, this.CharacterPhysics, CharacterConfig.SKILL_COOLDOWN_TRAP);

            characterTreasureLostEventList = new GameEventList();
			TrapDropEventList = new GameEventList();

		    AddTrap();
        }

		public override void InitCharacter()
		{
			var playerIndex = ((Player)possessedBy)?.GetPlayerIndex();
			if (playerIndex != null)
			{
				Rectangle characterStartPos = scene.CharacterStartPos[(int)playerIndex];
				InitCharacter(new Vector3(characterStartPos.X, characterStartPos.Y - characterStartPos.Height, 0f));
			}
		}

		public override void Update(GameTime gameTime, ActionSet actionSet)
	    {

			CharacterPhysics.Update(gameTime, actionSet);

			// Handle the remaining (non-movement) actions
		    List<Actor> touchedActors = CharacterPhysics.GetTouchedActors();
		    bool isFacingRight = CharacterPhysics.IsFacingRight();

	        if (!CharacterPhysics.IsStunned)
	        {

				// Check if the character carries a treasure and stands within the base
				if (CharacterPhysics.IsTouchingShopkeeper && !CharacterPhysics.IsInAir && InvTreasure != null)
				{
					InvTreasure = null;
					(rep as Spritesheet)?.SetVisibilityLayer(1, false);
					IncreaseScore();
				    scene.TreasuresLeft--;
				}

				// Check if the player touches any traps
		        foreach (Actor actor in CharacterPhysics.GetTouchedActors())
		        {
			        Trap currentTrap = actor as Trap;
			        if (currentTrap != null && currentTrap.Owner != this && (GetPosession().GetType() != typeof(AiIndigenController)))
			        {
						currentTrap.TrapTouchEventList.Execute(new List<object>() { this }, new List<object>() { currentTrap });
			        }

		        }

                var charSpritesheet = rep as Spritesheet;
                foreach (int actionSetAction in actionSet.actions)
	            {
	                switch (actionSetAction)
	                {
	                    case (InputConfig.Actions.HIT):
                            // Check surroundings - cut foliage (execute event), hit other player
			                if (CharacterPhysics.IsInAir || InvTreasure != null)
				                break;

                            if (charSpritesheet != null)
                            {
                                currentAnimDuration = charSpritesheet.GetAnimation("machete").Duration;
                                currentAnimRestartCounter = currentAnimDuration;
                            }
                            skillCut.Execute((long) gameTime.TotalGameTime.TotalMilliseconds, touchedActors);
	                        break;

	                    case (InputConfig.Actions.COLLECT):

	                        skillCollect.Execute((long) gameTime.TotalGameTime.TotalMilliseconds, touchedActors);

							break;

	                    case (InputConfig.Actions.SHOOT):
                            if (AmmoCount > 0)
                            {
                                if (charSpritesheet != null)
                                {
                                    currentAnimDuration = charSpritesheet.GetAnimation("shoot").Duration;
                                    currentAnimRestartCounter = currentAnimDuration;
                                }
                            }
                            skillShoot.Execute((long) gameTime.TotalGameTime.TotalMilliseconds);
	                                                
	                        break;

						case (InputConfig.Actions.TRAP):
			                if (CharacterPhysics.TrapPlacementPossible && TrapCount > 0 && InvTreasure == null && !CharacterPhysics.IsClimbing && !CharacterPhysics.IsStunned && !CharacterPhysics.IsInAir)
			                {
								skillTrap.Execute((long) gameTime.TotalGameTime.TotalMilliseconds);
				                TrapCount--;
								SkillCollectButtonTimeSpan = gameTime.TotalGameTime;
							}

							break;
	                }

	            }


	        }

	        base.Update(gameTime);
            
            // Check if the time it takes to place the trap is over (if player is placing a trap)
            if (gameTime.TotalGameTime.Subtract(SkillCollectButtonTimeSpan) > TimeSpan.FromMilliseconds(CharacterConfig.SKILL_DEPLOYTIME_TRAP * 1000))
		    {
			    skillTrap.ConcludePlacing();
				SkillCollectButtonTimeSpan = TimeSpan.MaxValue;
			}
			
		}

	    public void LooseTreasure(Vector2 dropPos)
	    {
	        if (InvTreasure != null)
	        {
	            
	            Spritesheet charSheet = getRepresentation() as Spritesheet;

	            charSheet?.SetVisibilityLayer(1, false);

                TreasureDropEvent currentTreasureDropEvent = new TreasureDropEvent(InvTreasure.getRepresentation(), new Vector3(dropPos.X, dropPos.Y, 0), scene);
                characterTreasureLostEventList.EventList.Add(currentTreasureDropEvent);
                characterTreasureLostEventList.Execute(new List<object>() { this }, new List<object>() { this });

                InvTreasure = null;
            }

        }

		public void AddTrap()
		{
			TrapDropEventList.EventList.Add(new TrapDropEvent(ItemsConfig.TRAP_CLOSED_TEXTURE, ItemsConfig.TRAP_OPEN_TEXTURE, this, scene));
		    TrapCount++;
		}

	    public void IncreaseScore()
	    {
			scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Deliver);
			scene.Scores[(Player)GetPosession()]?.addPoints(100);
	    }

	    public void StunAndSteal(Character character)
	    {
	        if (possessedBy is Player)
	        {
	            CharacterPhysics.IsStunned = true;
	            CharacterPhysics.CurrentStunTime = CharacterConfig.HIT_STUN_TIME;
	            if (InvTreasure != null)
	            {
	                //LooseTreasure(new Vector2(position.X, CharacterPhysics.BoundingBox.Bottom));
	                scene.TreasuresLeft--;
                    scene.RemoveActor(InvTreasure);
	                (rep as Spritesheet).SetVisibilityLayer(1, false);
	                InvTreasure = null;
	            }
	        }
	    }
	}
}
