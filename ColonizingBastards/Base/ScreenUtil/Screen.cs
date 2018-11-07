using ColonizingBastards.Base.Cameras;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.ScreenUtil
{
	abstract class Screen
	{
        public Camera camera { get; set; }
        protected Director.Director director;
        protected Scene.Scene scene;

		protected Hud hud;

		public abstract void Draw(SpriteBatch batch);

	}
}
