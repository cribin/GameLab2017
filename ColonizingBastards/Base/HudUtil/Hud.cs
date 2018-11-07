using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.ContentRestrictions;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.GameClasses.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.ScreenUtil
{
	class Hud
	{

		protected Director.Director director;
		protected Scene.Scene scene;

		protected List<HudElement> hudElements;
		public readonly List<SpriteFont> fonts;

		public readonly SpriteFont defaultFont;


		public Hud(ContentManager content, Director.Director director, Scene.Scene scene)
		{
			this.director = director;
			this.scene = scene;

			hudElements = new List<HudElement>();
			defaultFont = content.Load<SpriteFont>(MainConfig.PIPELINE_FONTS_DIRECTORY + "default");
			fonts = new List<SpriteFont>();
			fonts.Add(defaultFont);
			//TODO get from config
			fonts.Add(content.Load<SpriteFont>(MainConfig.PIPELINE_FONTS_DIRECTORY + "dimbo"));
			fonts.Add(content.Load<SpriteFont>(MainConfig.PIPELINE_FONTS_DIRECTORY + "ventura"));
		}

		public void AddHudElement(HudElement hudElement)
		{
			hudElements.Add(hudElement);
		}

		public void Update(GameTime gameTime)
		{
			// Don't process update requests since we don't need them in hud elements. They should be time-invariant.
		}


		public void DrawHud(SpriteBatch spriteBatch)
		{
			foreach (HudElement e in hudElements)
			{
				e.Draw(spriteBatch);
			}
		}


	}
}
