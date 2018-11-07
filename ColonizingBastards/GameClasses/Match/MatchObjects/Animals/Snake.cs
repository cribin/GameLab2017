using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Animals
{
    class Snake:Actor
    {
        private Spritesheet charSheet;
        public Rectangle BoundingBox { get; private set; }

        public bool Active { get; set; }

        public Snake(RenderObject rep, Vector2 startPos, Rectangle hitboxOffset) : base(rep)
        {
            charSheet = rep as Spritesheet;
            //Size of the snake corresponds to the size of the sprite(currently 120 x 60)
            SetSize(new Vector3(charSheet.SpriteWidth, charSheet.SpriteHeight, 0f));
			
            SetPosition(new Vector3(startPos.X, startPos.Y, 0f));
			
            BoundingBox = new Rectangle((int)(position.X + hitboxOffset.X),
               (int)(position.Y + hitboxOffset.Y), hitboxOffset.Width, hitboxOffset.Height);
        }

        /// <summary>
        /// Calls the appear animation of the snake
        /// </summary>
        /// <param name="characterFacingRight"></param>
        public void Appear(bool facingRight)
        {
            if (Active)
            {
                if (facingRight)
                    charSheet.PlayAnimation("appear", flipHorizontal:true);
                else
                    charSheet.PlayAnimation("appear");
            }
        }


		public override void Draw(SpriteBatch batch)
	    {
		    if (Active)
		    {
				base.Draw(batch);
		    }
	    }

		public float getCenterPointX()
		{
			return position.X + (size.X / 2);
		}
    }
}
