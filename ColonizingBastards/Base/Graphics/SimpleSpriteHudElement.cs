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
	class SimpleSpriteHudElement : HudElement
	{
		private Sprite sprite;

		public SimpleSpriteHudElement(Hud hud, Sprite sprite) : base(hud)
		{
			this.sprite = sprite;
		}

		public override void Draw(SpriteBatch batch)
		{ 
			sprite.Draw(batch);
		}

		public override void Update(GameTime gameTime)
		{
			sprite.Update(gameTime);
		}
	}
}
