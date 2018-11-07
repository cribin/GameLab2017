using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.MatchShopKeeper
{
    class ShopKeeper:Character
    {
        public string CHARACTER_NAME { get; set; }
        public string CHARACTER_DESCRIPTION { get; set; }
        private Spritesheet charSheet;
        public Rectangle BoundingBox { get; private set; }

        public bool ContactPlayer { get; set; }

        private int animDuration;
        private float animRestartCounter;

        public ShopKeeper(RenderObject rep, Scene scene, Rectangle hitboxOffset) : base(rep)
        {       
            Rectangle shopKeeperStartPos = scene.ShopKeeperStartPos[0];
            SetPosition(new Vector3(shopKeeperStartPos.X, shopKeeperStartPos.Y - shopKeeperStartPos.Height, 0f));
            SetSize(new Vector3(shopKeeperStartPos.Width, shopKeeperStartPos.Height, 0f));
            charSheet = getRepresentation() as Spritesheet;

           
            BoundingBox = new Rectangle((int)(position.X + hitboxOffset.X),
               (int)(position.Y + hitboxOffset.Y), hitboxOffset.Width, hitboxOffset.Height);

            animDuration = charSheet.GetAnimation("appear").Duration;
            animRestartCounter = animDuration;
            charSheet.PlayAnimation("appear");
        }

      
        public override void Update(GameTime gameTime)
        {
            if (ContactPlayer)
            {              
                charSheet.PlayAnimation("chashin");
                animDuration = charSheet.GetAnimation("chashin").Duration;
                animRestartCounter = animDuration;
                ContactPlayer = false;
            }
            else
            {
                animRestartCounter -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (animRestartCounter <= 0f)
                    charSheet.PlayAnimation("idle");
            }


            base.Update(gameTime);
        }
    }
}
