using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.LogicUtil;
using ColonizingBastards.Base.MiscUtil.TiledMapImporterUtil;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Instruction;
using ColonizingBastards.GameClasses.Match;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using ColonizingBastards.GameClasses.Match.MatchSound;
using ColonizingBastards.GameClasses.Menu.MenuObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Menu
{
    class MenuLogic:Logic
    { 
        private Player[] tempPlayer;
        private bool[] selectedPlayers;
        private int numOfReadyPlayers;

        private bool transitionFinished = false;
        private bool skipToMatch;

        public MenuLogic(Director director, Scene scene, bool skipToMatch = false)
        {
            this.director = director;
            this.scene = scene;
            this.skipToMatch = skipToMatch;
        }

        public override void Initialize()
        {
            //Init Menu map from tiled
            TiledMap tiledMenu = new TiledMap(MainConfig.CONTENT_MAP_DIRECTORY + "Menu.xml", director.Content, MainConfig.PIPELINE_GRAPHICS_DIRECTORY, scene);
            BaseScreenSize = tiledMenu.MapSize;

            //Init temporary players, which need to join to registered in the scene
            tempPlayer = new Player[4];
            selectedPlayers = new bool[4];
            numOfReadyPlayers = 0;

            for (var i = 0; i < 4; i++)
            {        
                tempPlayer[i] = new Player((PlayerIndex) i, new MenuInputMapper(true));           
            }

            //************************* INIT SOUND MANAGER ************************//
            MatchSoundManager matchSoundManager = new MatchSoundManager(director.Content);
			scene.RegisterMatchSoundManager(matchSoundManager);
#if DEBUG
			matchSoundManager.IsMuted = true;
#endif
			matchSoundManager.PlaySong(SongEnumeration.Title);

		}
          
        public override void Update(GameTime gameTime)
        {            
            foreach (Player p in tempPlayer)
            {
                p.Update(gameTime);
                ActionSet actions =  p.GetCurrentActionSet();
                int index = (int)p.playerIndex;
                //elapsedTime -= gameTime.ElapsedGameTime.Milliseconds;
                   
                foreach (int t in actions.actions)
                {
                    if (t == InputConfig.Actions.SELECT)
                    {                       
                        if (!selectedPlayers[index])
                        {
                            selectedPlayers[index] = true;
                            numOfReadyPlayers++;

                            scene.RemoveActor(scene.CharacterMenuJoinPos[index]);
                            scene.RegisterObject(scene.CharacterMenuReadyPos[index]);
                        }                                              
                    }

                    if (t == InputConfig.Actions.MENU)
                    {
                        if (selectedPlayers[index] && numOfReadyPlayers >= 2)
                        {
                           if(skipToMatch)
                                TransitionToMatch();
                           else 
                                TransitionToStory();
                        }
                    }
                }
                
                if(actions.actions.Count != 0)
                    actions.actions.Clear();                                 
            }
        }

        private void TransitionToStory()
        {
            if (transitionFinished) return;
            Scene storyScene = new Scene();
            storyScene.RegisterMatchSoundManager(scene.MatchSoundManager);
           
            Logic storyLogic = new StoryLogic(director, storyScene, selectedPlayers);
            storyLogic.Initialize();
            Screen storyScreen = new DefaultScreen(director, storyScene, storyLogic.BaseScreenSize);

            director.TransitionToLogic(storyLogic);
            director.TransitionToScreen(storyScreen, 3, -1);

            transitionFinished = true;
        }

        private void TransitionToMatch()
        {
            if (transitionFinished) return;

            Scene matchScene = new Scene();
            matchScene.RegisterMatchSoundManager(scene.MatchSoundManager);

            Logic matchLogic = new MatchLogic(director, matchScene, selectedPlayers);
            matchLogic.Initialize();
            Screen matchScreen = new MatchScreen(director, matchScene, matchLogic.BaseScreenSize, selectedPlayers);

            director.TransitionToLogic(matchLogic);
            director.TransitionToScreen(matchScreen, 3, -1);

            transitionFinished = true;
        }
    }
}
