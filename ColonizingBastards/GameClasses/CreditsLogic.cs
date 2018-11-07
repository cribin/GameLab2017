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
using ColonizingBastards.GameClasses.Menu;
using ColonizingBastards.GameClasses.Menu.MenuObjects;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses
{
    class CreditsLogic:Logic
    {
        private bool transitionFinished;

        public CreditsLogic(Director director, Scene scene)
        {
            this.director = director;
            this.scene = scene;
        }

        public override void Initialize()
        {
            TiledMap tiledStoryScreen = new TiledMap(MainConfig.CONTENT_MAP_DIRECTORY + "Credits.xml", director.Content, MainConfig.PIPELINE_GRAPHICS_DIRECTORY, scene);
            BaseScreenSize = tiledStoryScreen.MapSize;

            //create temporary players
            for (var i = 0; i < 4; i++)
            {
                var player = new Player((PlayerIndex)i, new MenuInputMapper(true));
                scene.RegisterPlayer(player);
            }
        }

        public override void Update(GameTime gameTime)
        {
            /*foreach (Player p in scene.GetPlayers())
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
            }*/
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
