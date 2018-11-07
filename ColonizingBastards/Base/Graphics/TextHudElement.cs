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
	class TextHudElement : HudElement
	{

		protected SpriteFont font;
		protected Func<object, string> getTextDelegate;
		protected Func<object, Color> getColorDelegate;

		protected object textDelegateArgument;
		protected object colorDelegateArgument;

	    private Color textColor;
	    private bool blinking;
	    private bool fadeOut;

	    private Vector2 scale;

		public TextHudElement(Hud hud, SpriteFont font, Func<object, string> getTextDelegate, Func<object, Color> getColorDelegate, object textDelegateArgument, object colorDelegateArgument, bool blinking = false, Vector2 scale = default(Vector2)) : base(hud)
		{
			this.font = font ?? hud.defaultFont;
			
			this.getTextDelegate = getTextDelegate;
			this.getColorDelegate = getColorDelegate;

			this.textDelegateArgument = textDelegateArgument;
			this.colorDelegateArgument = colorDelegateArgument;

		    this.blinking = blinking;
		    if (blinking)
		        fadeOut = true;    
		    textColor = this.getColorDelegate(colorDelegateArgument);

            this.scale = scale.Equals(Vector2.Zero) ? Vector2.One : scale;
		}

		public override void Draw(SpriteBatch batch)
		{       
            batch.DrawString(font, getTextDelegate(textDelegateArgument), new Vector2(position.X, position.Y), textColor,0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		public override void Update(GameTime gameTime)
		{
		    if (!blinking) return;
		    if (fadeOut)
		    {
		        textColor *= 0.95f;
		        if (textColor.A <= 10)
		            fadeOut = false;
		    }
		    else
		    {
		        textColor *= 1.05f;
		        if (textColor.A >= 250)
		            fadeOut = true;
		    }
		}
	}
}
