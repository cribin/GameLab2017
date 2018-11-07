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
using ColonizingBastards.GameClasses.Instruction;
using ColonizingBastards.GameClasses.Menu.MenuObjects;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses
{
    class StoryLogic:Logic
    {
        private bool transitionFinished = false;

        //show the final result screen for at least 5 seconds
        private float countDownUntiltransition = 10f;

        private bool[] selectedPlayers;

        public StoryLogic(Director director, Scene scene, bool[] selectedPlayers)
        {
            this.director = director;
            this.scene = scene;
            this.selectedPlayers = selectedPlayers;
        }

        public override void Initialize()
        {
            TiledMap tiledStoryScreen = new TiledMap(MainConfig.CONTENT_MAP_DIRECTORY + "Story.xml", director.Content, MainConfig.PIPELINE_GRAPHICS_DIRECTORY, scene);
            BaseScreenSize = tiledStoryScreen.MapSize;

            for (var i = 0; i < 4; i++)
            {
                if (!selectedPlayers[i]) continue;
                var player = new Player((PlayerIndex)i, new MenuInputMapper(true));
                scene.RegisterPlayer(player);
            }
        }

        public override void Update(GameTime gameTime)
        {
            countDownUntiltransition -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Player p in scene.GetPlayers())
            {
                p.Update(gameTime);
                ActionSet actions = p.GetCurrentActionSet();

                if (countDownUntiltransition < 10)
                {
                    foreach (int t in actions.actions)
                    {
                        if (t == InputConfig.Actions.SELECT)
                        {
                            TransitionToInstructions();
                        }
                    }
                }
                if (actions.actions.Count != 0)
                    actions.actions.Clear();
            }

            if (countDownUntiltransition <= 0)
                TransitionToInstructions();
        }

        private void TransitionToInstructions()
        {
            if (transitionFinished) return;
            Scene instructionScene = new Scene();
            instructionScene.RegisterMatchSoundManager(scene.MatchSoundManager);

            Logic instructionLogic = new InstructionLogic(director, instructionScene, selectedPlayers);
            instructionLogic.Initialize();
            Screen instructionScreen = new DefaultScreen(director, instructionScene, instructionLogic.BaseScreenSize);

            director.TransitionToLogic(instructionLogic);
            director.TransitionToScreen(instructionScreen, 3, -1);

            transitionFinished = true;
        }
    }
}
