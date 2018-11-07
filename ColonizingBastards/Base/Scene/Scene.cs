using ColonizingBastards.Base.Objects;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.SoundUtil;
using ColonizingBastards.GameClasses.Match.MatchObjects;
using ColonizingBastards.GameClasses.Match.MatchObjects.Ai;
using ColonizingBastards.GameClasses.Match.MatchObjects.Animals;
using ColonizingBastards.GameClasses.Match.MatchObjects.MatchShopKeeper;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.MatchObjects.WeatherObjects;
using ColonizingBastards.GameClasses.Match.MatchSound;
using ColonizingBastards.GameClasses.Match.ParticleSystem;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.Scene
{
	class Scene
	{

		// This list contains ALL entities of the scene.
        private List<Entity> objects;

		// This list contains ALL actors of the scene.
		private List<Actor> actors;

        private List<Player> players;

		private List<AiController> aiControllers;

		// This list contains ALL collidables of the scene.
	    private List<Collidable> collidables;

	    public List<Collidable> WalkableCollidables { get; private set; }

        public List<Collidable> ClimableCollidables { get; private set; }

		// This list contains ALL foliage of the scene.
		private List<Foliage> foliage;

		// This dictionary contains the scores for all players.
		public Dictionary<Player, Score> Scores;

        //Indicates how many treasures are left, if TreasuresLeft = 0 the game will end
        public int TreasuresLeft { get; set; }

        // The base where the treasure and loot can be kept
	    public List<Rectangle> ShopKeeperStartPos { get; set; }

        public ShopKeeper MatchShopKeeper { get; set; }

		public List<Rectangle> CharacterStartPos { get; set; }

        public List<Actor> CharacterMenuReadyPos { get; set; }

        public List<Actor> CharacterMenuJoinPos { get; set; }

        public List<Actor> CharacterWonPos { get; set; }

        public List<Actor> CharacterLostPos { get; set; }

        public List<Actor> CharacterNotPlayedPos { get; set; }

        public List<Rectangle> FinalScorePos { get; set; }

        public List<Actor> Characters { get; set; }

		public List<Snake> Snakes { get; set; }

		//First 4 Ui's correspond to the players score boards and the last corresponds to the treasure UI
		public List<Actor> MatchUiPos { get; set; }

        public Actor MatchPauseScreen { get; set; }

        public bool MatchPaused { get; set; }

        public List<ParticleEffectManager> ParticleEffects { get; }

		public MatchSoundManager MatchSoundManager { get; protected set; }

        public NavGraph NavigationGraph { get; set; }

        public WeatherSystem CurrWeatherSystem { get; set; }

        public LightningManager LightningManager { get; set; }

		public Random Random;

		public Scene()
		{
            objects = new List<Entity>();
			actors = new List<Actor>();
            players = new List<Player>();
            collidables = new List<Collidable>();
            WalkableCollidables = new List<Collidable>();
            ClimableCollidables = new List<Collidable>();
			foliage = new List<Foliage>();
			Snakes = new List<Snake>();

			CharacterStartPos = new List<Rectangle>();
            CharacterMenuReadyPos = new List<Actor>();
            CharacterMenuJoinPos = new List<Actor>();
            CharacterWonPos = new List<Actor>();
            CharacterLostPos = new List<Actor>();
            CharacterNotPlayedPos = new List<Actor>();
            FinalScorePos = new List<Rectangle>();
            Characters = new List<Actor>();
            MatchUiPos = new List<Actor>();
            ParticleEffects = new List<ParticleEffectManager>();
            ShopKeeperStartPos = new List<Rectangle>();
            LightningManager = new LightningManager();
			aiControllers = new List<AiController>();

			Random = new Random();
		}

        // Objects
        public void RegisterObject(Entity o)
        {
            objects.Add(o);
            if (o.GetType().GetTypeInfo().IsSubclassOf(typeof(Foliage)) || o.GetType() == typeof(Foliage))
            {
                foliage.Add((Foliage)o);

            }
            else if (o.GetType().GetTypeInfo().IsSubclassOf(typeof(Actor)) || o.GetType() == typeof(Actor))
	        {
		        actors.Add((Actor) o);
			}

            if (o.GetType().GetTypeInfo().IsSubclassOf(typeof(ShopKeeper)) || o.GetType() == typeof(ShopKeeper))
            {
                MatchShopKeeper = (ShopKeeper) o;
            }
			if (o.GetType().GetTypeInfo().IsSubclassOf(typeof(Snake)) || o.GetType() == typeof(Snake))
			{
				Snakes.Add((Snake) o);
			}

		}

	    public void AddFoliageToActors()
	    {
	        actors.AddRange(foliage);
	    }

        public List<Entity> GetObjects()
        {
            return objects;
        }

        public List<Actor> GetActors()
        {
            return actors;
        }

	    public void RemoveActor(Actor actor)
	    {
	        if (actors.Contains(actor))
	            actors.Remove(actor);

            if (actor.GetType().GetTypeInfo().IsSubclassOf(typeof(Foliage)) || actor.GetType() == typeof(Foliage))
            {
                foliage.Remove((Foliage)actor);

            }
        }

		public List<Foliage> GetFoliage()
		{
			return foliage;
		}


		public List<Snake> GetSnakes()
		{
			return Snakes;
		}


		public List<Entity> GetCollidableObjects()
        {
            //...
            return null;
        }


        // Players
        public void RegisterPlayer(Player player)
        {
            if (!players.Contains(player))
                players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            if (players.Contains(player))
                players.Remove(player);
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

		public List<AiController> GetAiControllers()
		{
			return aiControllers;
		}

		public void AddAiController(AiController aiController)
		{
			aiControllers.Add(aiController);
		}

		//Collidables
		public void RegisterCollidable(Collidable collidable)
        {
            if (!collidables.Contains(collidable))
                collidables.Add(collidable);

            if(collidable.IsClimbable)
                ClimableCollidables.Add(collidable);
            else if(collidable.IsWalkable)
                WalkableCollidables.Add(collidable);
        }

        public void RemoveCollidable(Collidable collidable)
        {
            if (collidables.Contains(collidable))
                collidables.Remove(collidable);

            if (collidable.IsClimbable)
                ClimableCollidables.Remove(collidable);
            else if (collidable.IsWalkable)
                WalkableCollidables.Remove(collidable);
        }

        public List<Collidable> GetCollidables()
        {
            return collidables;
        }

        //Particle Effects
	    public void RegisterParticleEffect(ParticleEffectManager particleEffect)
	    {
	        if(!ParticleEffects.Contains(particleEffect))
                ParticleEffects.Add(particleEffect);
	    }

	    public void RemoveParticleEffect(ParticleEffectManager particleEffect)
	    {
	        ParticleEffects.Remove(particleEffect);
	    }


		public void RegisterMatchSoundManager(MatchSoundManager matchSoundManager)
		{
			this.MatchSoundManager = matchSoundManager;
		}

	    private List<AiController> aiControllersToRemove = new List<AiController>();
        public void RemoveAiController(AiController aiController)
	    {
	        aiControllersToRemove.Add(aiController);
	    }

	    public List<AiController> GetAndClearPendingAiRemovals()
	    {
		    List<AiController> res = new List<AiController>(aiControllersToRemove);
			aiControllersToRemove.Clear();
		    return res;
	    }

	}
}
