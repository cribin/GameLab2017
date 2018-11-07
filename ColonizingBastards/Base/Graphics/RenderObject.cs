using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.Graphics
{
	abstract class RenderObject
	{

        public Vector3 position { get; protected set; }
        public double rotation { get; protected set; }
		public Vector3 size { get; protected set; }

		public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch batch);

        public abstract void SetPosition(Vector3 position);
        public abstract void SetRotation(Vector3 rotation);
        public abstract void SetSize(Vector3 size);

	}
}
