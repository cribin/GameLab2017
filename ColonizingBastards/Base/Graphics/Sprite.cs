 using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ColonizingBastards.GameClasses.Match.ParticleSystem;
 using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.Graphics
{
	class Sprite : RenderObject
	{

		public Texture2D Texture { get; }
	    private bool flipVertically;

        public Sprite(Texture2D texture, bool flipVertically = false) {
            this.Texture = texture;
            this.flipVertically = flipVertically;
        }

        public override void Draw(SpriteBatch batch)
		{
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)(position.Y - size.Y), (int)size.X, (int)size.Y);
            if(flipVertically)
                batch.Draw(Texture, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
		    else
			    batch.Draw(Texture, destinationRectangle, Color.White);
		}

        public void Draw(SpriteBatch batch, float alpha)
        {
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)(position.Y - size.Y), (int)size.X, (int)size.Y);
            batch.Draw(Texture, destinationRectangle, Color.White * alpha);
        }

        //Special draw method to draw particles
        public void Draw(SpriteBatch spriteBatch, ParticleParameters particleParams)
        {
            Rectangle sourceRectangle = Texture.Bounds;
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

            spriteBatch.Draw(Texture, particleParams.Pos, sourceRectangle, particleParams.ParticleCol,
                particleParams.Rot, origin, particleParams.Scale, SpriteEffects.None, 0f);
        }

        //Draw function with more params
	    public void Draw(SpriteBatch spriteBatch, Vector2 startPos, Color color, float rot, Vector2 origin, Vector2 scale, SpriteEffects spriteEffect,float layerDepth = 0f, Rectangle sourceRec = default(Rectangle))
	    {
	        spriteBatch.Draw(Texture, startPos, null, color, rot, origin, scale, spriteEffect, layerDepth );
	    }

        //Draw function with more params
        public void Draw(SpriteBatch spriteBatch, Vector2 startPos, Color color, float rot, Vector2 origin, float scale, SpriteEffects spriteEffect, float layerDepth = 0f, Rectangle sourceRec = default(Rectangle))
        {
            spriteBatch.Draw(Texture, startPos, null, color, rot, origin, scale, spriteEffect, layerDepth);
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
            this.size = size;
        }

		public override void Update(GameTime gameTime)
		{
			// Nothing
		}
	}
}
