using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.LogicUtil;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using ColonizingBastards.Base.MiscUtil;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.Base.MiscUtil.TiledMapImporterUtil;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses.GameEvent;
using ColonizingBastards.GameClasses.GameOver;
using ColonizingBastards.GameClasses.Match.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Ai;
using ColonizingBastards.GameClasses.Match.MatchObjects.Animals;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using ColonizingBastards.GameClasses.Match.MatchObjects.Items;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.MatchObjects.WeatherObjects;
using ColonizingBastards.GameClasses.Match.MatchSound;
using ColonizingBastards.GameClasses.Match.ParticleSystem;
using ColonizingBastards.GameClasses.Menu;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match
{
    class MatchLogic : Logic
    {
		private Vector2 lastPos;
        private bool[] selectedPlayers;
        private int maxNumOfPlayers = 4;
        private int numOfTreasureTypes = 5;
        private WeatherSystem weatherSystem;

	    private double rainTimeDelta;
	    private bool previouslyRaining; 

        private bool transitionFinished = false;
        private int maxAmmoCount = 3;
        private float resumeCounter = 0;

        public MatchLogic(Director director, Scene scene, bool[] selectedPlayers)
        {
            this.director = director;
            this.scene = scene;
            this.selectedPlayers = selectedPlayers;
        }

        public override void Initialize()
        {
            //************************* INIT TEXTURES(needs to be init before map) ************************//

            //Load in bullet texture
            ProjectileConfig.BULLET_TEXTURE =
                director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "Bullet");
            ProjectileConfig.BULLET_SIZE = new Vector3(ProjectileConfig.BULLET_TEXTURE.Width/1.5f,
                ProjectileConfig.BULLET_TEXTURE.Height/1.5f, 0f);//new Vector3(ProjectileConfig.BULLET_TEXTURE.Width/2f, ProjectileConfig.BULLET_TEXTURE.Height/2f, 0);

            //Load in dart texture
            ProjectileConfig.DART_TEXTURE =
                director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "Dart");
            ProjectileConfig.DART_SIZE = new Vector3(ProjectileConfig.DART_TEXTURE.Width,
                ProjectileConfig.DART_TEXTURE.Height, 0f);

            //Init all treasure and ammo crate textures into the ItemsConfig class
            ItemsConfig.LOOT_TEXTURE = new List<Texture2D>();
            string treasurePrefix = "Treasure0";
            for (var i = 0; i < numOfTreasureTypes; i++)
            {
                ItemsConfig.LOOT_TEXTURE.Add(
                    director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY +  treasurePrefix + (i+1)));
            }

			ItemsConfig.AMMO_CRATE_TEXTURE =
				director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "NewCrate");

            for (var i = 0; i < maxAmmoCount+1; i++)
                ItemsConfig.AMMO_COUNT_TEXTURES.Add(
                    director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "Ammo" + i));

			ItemsConfig.TRAP_CLOSED_TEXTURE =
				director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "TrapClosed");

			ItemsConfig.TRAP_OPEN_TEXTURE =
				director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "TrapOpen");

			ParticleConfig.WHITE_CIRCLE =
                director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "whiteCircle");

            ParticleConfig.RAIN_DROP =
                director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "raindrop");

            int numOfFootstepTextures = 3;
            string texturePrefix = "FootstepParticle0";
            for (var i = 0; i < numOfFootstepTextures; i++)
            {
                ParticleConfig.FOOTSTEP_TEXTURE.Add(new Sprite(director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + texturePrefix + i)));
            }

            int numOfFoliageTextures = 5;
            texturePrefix = "UndergrowthParticle0";
            for (var i = 0; i < numOfFoliageTextures; i++)
            {
                ParticleConfig.FOLIAGE_CUT_TEXTURE.Add(new Sprite(director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + texturePrefix + (i + 1))));                 
            }

            LightningConfig.LIGHTNING_TEXTURES[0] = director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "Half Circle");
            LightningConfig.LIGHTNING_TEXTURES[1] = director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "Lightning Segment");

            //************************* INIT MAP(map needs to be initialized before player) ************************//

            //Import a simple test level 
            TiledMap tiledMap = new TiledMap(MainConfig.CONTENT_MAP_DIRECTORY + "FinalLevel.xml", director.Content, MainConfig.PIPELINE_GRAPHICS_DIRECTORY, scene);

            this.BaseScreenSize = tiledMap.MapSize;


			//************************* INIT SHOP KEEPER ************************//
			Character shopkeeper = CharacterImporter.ImportCharacter(
					MainConfig.CONTENT_CHARACTERS_DIRECTORY + MainConfig.CONTENT_SHOPKEEPER_FILE, director.Content, scene);
			scene.RegisterObject(shopkeeper);



			//************************* INIT AI GRAPH ************************//
			NavGraph navGraph = new NavGraph(scene);

			//************************* INIT FOLIAGE (after the characters!!) ************************//


			// Initialize the foliage events
			// Initialize treasures and ammo crates
			int foliageCount = scene.GetFoliage().Count;
            int treasureCount = selectedPlayers.Length * MatchConfig.TreasuresPerPlayer;//(int)(foliageCount * MatchConfig.TreasureFactor);
	        int ammoCrateCount = (int) (foliageCount * MatchConfig.AmmoCrateFactor);
	        int indigenousCount = (int) Math.Ceiling(selectedPlayers.Length * MatchConfig.IndPeoplePerPlayer);//(foliageCount * MatchConfig.IndigenousFactor);
            int snakeCount = (int) (foliageCount * MatchConfig.SnakeSpawnFactor);

			scene.TreasuresLeft = treasureCount;

			List<Foliage> undecidedFoliage = new List<Foliage>();
			undecidedFoliage.AddRange(scene.GetFoliage());

			// Favor higher distances to the shopkeeper
	        ProbabilityList<Foliage, double> undecidedFoliageProbabilityList =
		        new ProbabilityList<Foliage, double>(MatchConfig.TreasureDistanceProbabilityMeanFactor, MatchConfig.TreasureDistanceProbabilityVarianceFactor, foliage =>
			        Math.Sqrt(
						Math.Pow(foliage.GetPosition().X - shopkeeper.GetPosition().X, 2) +
						Math.Pow(foliage.GetPosition().Y - shopkeeper.GetPosition().Y, 2) +
						Math.Pow(foliage.GetPosition().Z - shopkeeper.GetPosition().Z, 2) )
				, scene.Random);


	        Random random = scene.Random;


			// WARNING: Order of hiding matters! 1. AI, 2. Snakes, 3. Rest
			// This way, snakes can be hidden with other things.

			// Hide Indigenous AI
			for (int i = 0; i < indigenousCount; i++)
			{
				Foliage currentFoliage;
				SpawnAiPossessedCharacterEvent currentSpawnAiPossessedCharacterEvent;
				Character currentIndigenousCharacter;
				// Select one random foliage to put treasure inside
				currentFoliage = undecidedFoliage[random.Next(undecidedFoliage.Count)];
				undecidedFoliage.Remove(currentFoliage);
				currentIndigenousCharacter = CharacterImporter.ImportCharacter(MainConfig.CONTENT_CHARACTERS_DIRECTORY + "IndigenousChar.xml", director.Content, scene);
				currentIndigenousCharacter.InitCharacter(new Vector3(currentFoliage.PositionForContent.X, currentFoliage.PositionForContent.Y - 150, 0f));
				currentSpawnAiPossessedCharacterEvent = new SpawnAiPossessedCharacterEvent(currentIndigenousCharacter, scene);
				currentFoliage.CharacterCutEventList.EventList.Add(currentSpawnAiPossessedCharacterEvent);
			}

			//Hide snakes
			for (int i = 0; i < snakeCount; i++)
			{
				Foliage currentFoliage;
				SpawnSnakeEvent currentSpawnSnakeEvent;
				currentFoliage = undecidedFoliage[random.Next(undecidedFoliage.Count)];

				// Don't remove currentfoliage from undecidedfoliage because snakes should appear in any foliage (except foliage with AI)

				Tuple<Spritesheet, Rectangle> snakeData = SpritesheetImporter.ImportSpritesheet(MainConfig.CONTENT_CHARACTERS_DIRECTORY + "Snake.xml", director.Content, scene);

				Snake snake = new Snake(snakeData.Item1, new Vector2(currentFoliage.PositionForContent.X, currentFoliage.PositionForContent.Y - 70), snakeData.Item2);
				scene.RegisterObject(snake);
				currentSpawnSnakeEvent = new SpawnSnakeEvent(snake, scene);
				//test
				var rand = random.Next(1, 4);//1,2,3

				if (rand < 2)
				{
					Foliage movingFoliage = CreateMovingFoliage(currentFoliage);

					movingFoliage.CharacterCutEventList.EventList.Add(currentSpawnSnakeEvent);
					scene.RegisterObject(movingFoliage);
					scene.RemoveActor(currentFoliage);

					// Make sure that things hidden later are hidden in foliage that is accessible and visible
					undecidedFoliage.Remove(currentFoliage);
					undecidedFoliage.Add(movingFoliage);
				}
				else
				{
					currentFoliage.CharacterCutEventList.EventList.Add(currentSpawnSnakeEvent);
				}

			}


			// Hide treasures
			undecidedFoliageProbabilityList.AddRange(undecidedFoliage);
			undecidedFoliageProbabilityList.LockList(true);
			
			for (int i = 0; i < treasureCount; i++)
            {
                Foliage currentFoliage;
                TreasureDropEvent currentTreasureDropEvent;
                // Select one random foliage to put treasure inside
	            currentFoliage = undecidedFoliageProbabilityList.DrawItem();
                //test
                var rand = random.Next(1, 4);
     
                if (rand < 2)
                {
                    Foliage movingFoliage = CreateMovingFoliage(currentFoliage);
                   
                    currentTreasureDropEvent = new TreasureDropEvent(random, currentFoliage.PositionForContent, scene);
                    movingFoliage.CharacterCutEventList.EventList.Add(currentTreasureDropEvent);
                    scene.RegisterObject(movingFoliage);
                    scene.RemoveActor(currentFoliage);
				}
                else
                {
                    currentTreasureDropEvent = new TreasureDropEvent(random, currentFoliage.PositionForContent, scene);
                    currentFoliage.CharacterCutEventList.EventList.Add(currentTreasureDropEvent);
                }

                undecidedFoliage.Remove(currentFoliage);
                
                
            }

			// Hide ammo crates
			for (int i = 0; i < ammoCrateCount; i++)
			{
				Foliage currentFoliage;
				AmmoDropEvent currentAmmoDropEvent;
				// Select one random foliage to put an ammo crate inside
				currentFoliage = undecidedFoliage[random.Next(undecidedFoliage.Count)];
                undecidedFoliage.Remove(currentFoliage);
				currentAmmoDropEvent = new AmmoDropEvent(ItemsConfig.AMMO_CRATE_TEXTURE, currentFoliage.PositionForContent, scene);
				currentFoliage.CharacterCutEventList.EventList.Add(currentAmmoDropEvent);
			}

			


            

			//************************* INIT PLAYERS ************************//

			// Creating as many players, as there are connected gamepads (up to four)
			// Player 1 and 2 are always created.
			Character defaultCharacter = null;
            for (var i = 0; i < maxNumOfPlayers; i++)
            {
			    if(!selectedPlayers[i])
                    continue;

                Player player;
                if (i == 1 || i == 2)
                {
                    player = new Player((PlayerIndex) i, new MatchInputMapper(true, i), scene);
                    scene.RegisterPlayer(player);
                }
                else
                {
                    player = new Player((PlayerIndex)i, new MatchInputMapper(false, -1), scene);
                    scene.RegisterPlayer(player);
                }

			
				defaultCharacter = CharacterImporter.ImportCharacter(
					MainConfig.CONTENT_CHARACTERS_DIRECTORY + MainConfig.CONTENT_CHARACTER_FILE_XML(i + 1), director.Content, scene);
				player.Possess(defaultCharacter);
				//Character can only be initialized after the possesing player has been set.
				defaultCharacter.InitCharacter();
                scene.Characters.Add(defaultCharacter);
				//scene.RegisterObject(defaultCharacter);
				
			}

            //Correct walkable areas of each walkable polygons(MinX and MaxX may overlap)
            var character = (DefaultCharacter) defaultCharacter;
            if (character != null)
            {
                //TODO:DELTAY is only a crude approximation. Find better way
                float deltaY = 2* character.HitboxOffset.Height;

                foreach (var c1 in scene.WalkableCollidables)
                {
                    foreach (var c2 in scene.WalkableCollidables)
                    {
                        if (c1 == c2) continue;
                        float centerDistY = Math.Abs(c1.Center.Y - c2.Center.Y);
                        if (centerDistY > deltaY) continue;
                        //If centerDistY < deltaY, then one walkable polygon may overlap another walkable
                        if (c1.WalkableMinMax.X < c2.WalkableMinMax.X && c1.WalkableMinMax.Y > c2.WalkableMinMax.X)
                        {                    
                            if (c1.Center.Y < c2.Center.Y)
                            {
                                //c1 is overlapping(above) c2 => adjust walkables positions of c2
                                c2.WalkableMinMax.X = c1.WalkableMinMax.Y;
                            }
                            else
                            {
                                //c2 is overlapping(above) c1 => adjust walkables positions of c2
                                c1.WalkableMinMax.Y = c2.WalkableMinMax.X;
                            }                           
                        }
                    }
                }
            }

			//************************* INIT SCORE ************************//

			// Initialize the score
			scene.Scores = new Dictionary<Player, Score>();
			foreach (Player p in scene.GetPlayers())
			{
				scene.Scores.Add(p, new Score());
			}


            //************************* INIT PARTICLE EFFECTS ************************//
            /*Rectangle startPosRec = scene.ShopKeeperStartPos[0];
            Vector3 firePos = new Vector3(startPosRec.X, startPosRec.Y, 0f);
            ParticleEffectFactory fireParticleEffectFactory = new FireParticleEffectFactory(FireParticleEffectType.Default, new Vector2(firePos.X, firePos.Y), 10);
            //ParticleEffectFactory fireParticleEffectFactory = new FireParticleEffectFactory(new Vector2(firePos.X, firePos.Y), 10, 3000, new Vector2(0.5f,0f), 
            // new Vector2(0f, 200), 0.5f, -0.3f, Color.Red, Color.LightYellow, 2750);
            ParticleEffectManager fireParticleEffectManager = new ParticleEffectManager(new Sprite(ParticleConfig.WHITE_CIRCLE),fireParticleEffectFactory, 10, 16, true, BlendState.Additive);
            scene.RegisterParticleEffect(fireParticleEffectManager);*/

          
            weatherSystem = new WeatherSystem(scene, BaseScreenSize);
            scene.CurrWeatherSystem = weatherSystem;

            //************************* PLAY INGAME THEME ************************//
            scene.MatchSoundManager.PlaySong(SongEnumeration.InGame);

        }

	    public override void Update(GameTime gameTime)
	    {
	        resumeCounter -= (float) gameTime.ElapsedGameTime.TotalSeconds;
	        if (scene.MatchPaused)
	        {
                foreach (Player p in scene.GetPlayers())
                {
                    p.Update(gameTime);
                    ActionSet actions = p.GetCurrentActionSet();
                   
                    foreach (int t in actions.actions)
                    {
                        if (t == InputConfig.Actions.JUMP)
                        {
                            ResumeGame();
                            resumeCounter = 0.2f;
                        }

                        if (t == InputConfig.Actions.TRAP)
                        {
                            TransitionToMenu();
                        }

                    }

                    if (actions.actions.Count != 0)
                        actions.actions.Clear();
                }
            }
	        else
	        {
	            scene.MatchShopKeeper.Update(gameTime);

	            if (resumeCounter <= 0)
	            {
	                foreach (Player p in scene.GetPlayers())
	                {
	                    p.Update(gameTime);
	                    ActionSet actions = p.GetCurrentActionSet();

	                    foreach (int t in actions.actions)
	                    {
	                        if (t == InputConfig.Actions.MENU)
	                        {
	                            PauseGame();
	                        }


	                    }
	                }
	            }

	            List<AiController> aiControllers = scene.GetAiControllers();
	            foreach (AiController controller in aiControllers)
	            {
	                controller.Update(gameTime);
	            }

	            foreach (AiController controller in scene.GetAndClearPendingAiRemovals())
	            {
	                aiControllers.Remove(controller);
	            }

	            weatherSystem.Update(gameTime);

	            if (weatherSystem.IsRaining && previouslyRaining)
	            {
	                rainTimeDelta += gameTime.ElapsedGameTime.TotalSeconds;
	                if (rainTimeDelta >= 5)
	                {
	                    scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Rain);
	                    rainTimeDelta = 0;
	                }
	            }
	            else if (weatherSystem.IsRaining && !previouslyRaining)
	            {
	                scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Rain);
	                previouslyRaining = true;
	            }
	            else
	            {
	                rainTimeDelta = 0;
	            }

                //Update moving foliages
                foreach (Foliage foliage in scene.GetFoliage())
	            {
	                foliage.Update(gameTime);
	            }

	            //Update snakes
	            foreach (Snake snake in scene.GetSnakes())
	            {
	                snake.Update(gameTime);
	            }

	            foreach (ParticleEffectManager pEffect in scene.ParticleEffects)
	                pEffect.Update(gameTime);

	            scene.LightningManager.Update();

	            //Winning Condition: End Match if there are no treasures left
	            if (scene.TreasuresLeft <= 0)
	                TransitionToWinningScreen();
                 
	            // DEBUG! TODO: REMOVE LATER
#if DEBUG
	            if (Keyboard.GetState() != null)
	            {
	                KeyboardState state = Keyboard.GetState();
	                if (state.IsKeyDown(Keys.I) && scene.GetAiControllers().Count < 1)
	                {
	                    Character ch;
	                    ch =
	                        CharacterImporter.ImportCharacter(
	                            MainConfig.CONTENT_CHARACTERS_DIRECTORY + "IndigenousChar.xml", director.Content, scene);
	                    ch.InitCharacter(new Vector3(500, 500 - 70, 0f));
	                    ch.InitCharacter();
	                    AiIndigenController c = new AiIndigenController(scene, scene.NavigationGraph, new AiBehavior());
	                    c.Possess(ch);
	                    scene.Characters.Add(ch);
	                    scene.GetActors().Add(ch);
	                    scene.AddAiController(c);
	                }
	            }
#endif
	        }
	    }

        private void TransitionToWinningScreen()
        {
            if (transitionFinished) return;

            Scene finalResultScene = new Scene();
            finalResultScene.Scores = scene.Scores;
			finalResultScene.RegisterMatchSoundManager(scene.MatchSoundManager);
            Logic finalResultLogic = new FinalResultLogic(director, finalResultScene, selectedPlayers);
            finalResultLogic.Initialize();
            FinalResultHud hud = new FinalResultHud(director.Content, director, finalResultScene, selectedPlayers);
            Screen finalResultScreen = new DefaultScreen(director, finalResultScene, finalResultLogic.BaseScreenSize, hud);

            director.TransitionToLogic(finalResultLogic);
            director.TransitionToScreen(finalResultScreen, 10, -1);

            transitionFinished = true;
        }

        private void TransitionToMenu()
        {
            if (transitionFinished) return;

            Scene menuScene = new Scene();

            Logic menuLogic = new MenuLogic(director, menuScene);
            menuLogic.Initialize();
            Screen menuScreen = new DefaultScreen(director, menuScene, menuLogic.BaseScreenSize);

            director.TransitionToLogic(menuLogic);
            director.TransitionToScreen(menuScreen, 3, -1);

            transitionFinished = true;
        }

        private Foliage CreateMovingFoliage(Foliage currentFoliage)
        {
            RenderObject foliageSpritesheet = SpritesheetImporter.ImportSpritesheet(MainConfig.CONTENT_CHARACTERS_DIRECTORY + "UndergrowthAnimation.xml", director.Content, scene).Item1;
            Foliage movingFoliage = new Foliage(foliageSpritesheet, scene, currentFoliage.GetPosition());

            Vector3 currPos = currentFoliage.GetPosition();
            Vector3 currRot = currentFoliage.GetRotation();
            Vector3 currSize = currentFoliage.GetSize();

            movingFoliage.SetPosition(currentFoliage.Moving
                ? new Vector2(currPos.X, currPos.Y)
                : new Vector2(currPos.X, currPos.Y - currSize.Y - 1f));

            movingFoliage.SetRotation(currRot);
            movingFoliage.SetSize(currSize);

            return movingFoliage;
        }

        private void PauseGame()
        {
            scene.MatchPaused = true;
        }

        private void ResumeGame()
        {
            scene.MatchPaused = false;
        }
    }
}
