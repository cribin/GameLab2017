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
using ColonizingBastards.GameClasses.Match;
using ColonizingBastards.GameClasses.Menu.MenuObjects;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Instruction
{
    class InstructionLogic:Logic
    {
        private bool transitionFinished;

        private readonly bool[] selectedPlayers;

        private const float screenShowDuration = 10;
        private float transitionCountDownS = screenShowDuration;

        public InstructionLogic(Director director, Scene scene, bool[] selectedPlayers)
        {
            this.director = director;
            this.scene = scene;
            this.selectedPlayers = selectedPlayers;
        }

        public override void Initialize()
        {
            TiledMap tiledInstructionScreen = new TiledMap(MainConfig.CONTENT_MAP_DIRECTORY + "InstructionScreen.xml", director.Content, MainConfig.PIPELINE_GRAPHICS_DIRECTORY, scene);
            BaseScreenSize = tiledInstructionScreen.MapSize;

            for (var i = 0; i < 4; i++)
            {
                if (!selectedPlayers[i]) continue;
                var player = new Player((PlayerIndex)i, new MenuInputMapper(true));
                scene.RegisterPlayer(player);
            }

        }

        public override void Update(GameTime gameTime)
        {
            transitionCountDownS -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Player p in scene.GetPlayers())
            {
                p.Update(gameTime);
                ActionSet actions = p.GetCurrentActionSet();
              
                if (transitionCountDownS < screenShowDuration-1)
                {
                    foreach (int t in actions.actions)
                    {
                        if (t == InputConfig.Actions.SELECT)
                        {
                            TransitionToMatch();
                        }
                    }
                }
                if (actions.actions.Count != 0)
                    actions.actions.Clear();
            }

            if(transitionCountDownS <= 0)
                TransitionToMatch();
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
