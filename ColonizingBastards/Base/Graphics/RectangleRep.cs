using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.Graphics
{
    class RectangleRep : RenderObject
    {

        private Rectangle rect;
        private Texture2D texture;
        private Color color;

        GraphicsDevice graphics;

        public RectangleRep(GraphicsDevice graphics, Vector3 position, double rotation, Vector2 size, Color color)
        {
            this.graphics = graphics;
            rect = new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y);
            this.position = position;
            this.rotation = rotation;
            this.size = new Vector3(size.X, size.Y, 0.0f);
            this.color = color;

            texture = new Texture2D(graphics, (int)size.X, (int)size.Y);

            Color[] data = new Color[((int) size.X) * ((int) size.Y)];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            texture.SetData(data);
        }

        public void SetColor(Color color)
        {
            this.color = color;
            UpdateTexture();
        }

        public override void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public override void SetRotation(Vector3 rotation)
        {
            this.rotation = rotation.Z;
        }

        public override void SetSize(Vector3 size)
        {
            this.size = new Vector3(size.X, size.Y, 0.0f);
            UpdateTexture();
        }

        public void UpdateTexture()
        {
            Color[] data = new Color[((int)size.X) * ((int)size.Y)];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            texture.SetData(data);
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y), color);
        }

		public override void Update(GameTime gameTime)
		{
			// Nothing
		}
	}
}
