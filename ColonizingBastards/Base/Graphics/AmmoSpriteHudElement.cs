using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.ScreenUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.Graphics
{
    class AmmoSpriteHudElement:SimpleSpriteHudElement
    {
        private SpriteFont font;
        private Func<Sprite> getSpriteDelegate;
      
        public AmmoSpriteHudElement(Hud hud, Func<Sprite> getSpriteDelegate, object spriteDelegateArgument) : base(hud, getSpriteDelegate())
        {
            this.getSpriteDelegate = getSpriteDelegate;
        }

        public override void Draw(SpriteBatch batch)
        {
            Sprite sprite = getSpriteDelegate();
            sprite?.Draw(batch);
        }
    }
}
