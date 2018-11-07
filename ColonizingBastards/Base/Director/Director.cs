using ColonizingBastards.Base.LogicUtil;
using ColonizingBastards.Base.ScreenUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace ColonizingBastards.Base.Director
{
	class Director
	{

        public GraphicsDeviceManager Graphics { get; set; }
        public ContentManager Content { get; }
        private SpriteBatch RenderBatch;

		private LogicManager logicManager;
		private ScreenManager screenManager;

		public Director(GraphicsDeviceManager graphics, ContentManager content)
		{
            this.logicManager = new LogicManager();
            this.screenManager = new ScreenManager();

            this.Graphics = graphics;
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            this.RenderBatch = new SpriteBatch(graphics.GraphicsDevice);

			this.Content = content;

		}
	
		public void AddLogic(Logic logic)
		{
			logicManager.AddLogic(logic);
		}

		public void AddScreen(Screen screen)
		{
			screenManager.AddScreen(screen);
		}
		
		public void Update(GameTime gameTime)
		{
			logicManager.UpdateCurrentLogic(gameTime);
		}

		public void Draw()
		{
            //Debug Code for Collision detection: Draws the wireframes of the drawable objects          
            /*RenderBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,transformMatrix: screenManager.GetCurrentScreen().camera.GetViewMatrix());
            RasterizerState state = new RasterizerState();
            state.FillMode = FillMode.WireFrame;
            RenderBatch.GraphicsDevice.RasterizerState = state;*/
            //RenderBatch.Begin(transformMatrix: screenManager.GetCurrentScreen().camera.GetViewMatrix());
            screenManager.DrawCurrentScreen(RenderBatch);
            //RenderBatch.End();
		}

		public void TransitionToScreen(Screen screen, long deltaTime, int idTransition)
		{
            screenManager.ReplaceCurrentScreen(screen);
		}

		public void TransitionToLogic(Logic logic)
		{
            logicManager.ReplaceCurrentLogic(logic);
		}

        public Logic GetCurrentLogic()
        {
            return logicManager.GetCurrentLogic();
        }

        public Screen GetCurrentScreen()
        {
            return screenManager.GetCurrentScreen();
        }
		
	}
}
