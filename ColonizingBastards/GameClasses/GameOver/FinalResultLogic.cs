using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.LogicUtil;
using ColonizingBastards.Base.MiscUtil.TiledMapImporterUtil;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects;
using ColonizingBastards.GameClasses.Match.MatchSound;
using ColonizingBastards.GameClasses.Menu;
using ColonizingBastards.GameClasses.Menu.MenuObjects;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.GameOver
{
    class FinalResultLogic:Logic
    {
        private bool[] selectedPlayers;

        private bool transitionFinished = false;

        //show the final result screen for at least 5 seconds
        private float countDownUntiltransition = 2f;

        public FinalResultLogic(Director director, Scene scene, bool[] selectedPlayers)
        {
            this.director = director;
            this.scene = scene;
            this.selectedPlayers = selectedPlayers;

        }

        public override void Initialize()
        {
            TiledMap tiledFinalResultScreen = new TiledMap(MainConfig.CONTENT_MAP_DIRECTORY + "WinScreen.xml", director.Content, MainConfig.PIPELINE_GRAPHICS_DIRECTORY, scene);
            BaseScreenSize = tiledFinalResultScreen.MapSize;

            //Explanation: Aggregate does a fold in functional programming to find the key with the highest value
            int highestScore =
                scene.Scores.Aggregate((l, r) => l.Value.getScore() > r.Value.getScore() ? l : r).Value.getScore();

            //check whether there is a tie between the players with the highest score
            var highestScoreIndices = scene.Scores.Where(s => s.Value.getScore() == highestScore).Select(s=> s.Key.playerIndex)
             .ToList();
            
            for (var i = 0; i < 4; i++)
            {
                if (selectedPlayers[i])
                {
                    scene.RegisterObject(highestScoreIndices.Contains((PlayerIndex) i)
                        ? scene.CharacterWonPos[i]
                        : scene.CharacterLostPos[i]);
                    
                    var player = new Player((PlayerIndex)i, new MenuInputMapper(true));
                    scene.RegisterPlayer(player);
                 
                }
                else
                    scene.RegisterObject(scene.CharacterNotPlayedPos[i]);

            }

			scene.MatchSoundManager.PauseSong();
			scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Fanfare);

        }

        public override void Update(GameTime gameTime)
        {
            countDownUntiltransition -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (countDownUntiltransition > 0) return;
          
            foreach (Player p in scene.GetPlayers())
            {
                p.Update(gameTime);
                ActionSet actions = p.GetCurrentActionSet();
                int index = (int) p.playerIndex;

                foreach (int t in actions.actions)
                {
                    if (t == InputConfig.Actions.MENU)
                    {
                        TransitionToMenu();
                    }
                }

                if (actions.actions.Count != 0)
                    actions.actions.Clear();
            }
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
    }
}
