using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// INTITIALIZE!!!
/// </summary>
namespace ColonizingBastards.Base.LogicUtil
{
	abstract class Logic
	{

        protected Director.Director director;
        protected Scene.Scene scene;
        public Vector2 BaseScreenSize { get; protected set; }

        public abstract void Initialize();

		public abstract void Update(GameTime gameTime);

	}
}
