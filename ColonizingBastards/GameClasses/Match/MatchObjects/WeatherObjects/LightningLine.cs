using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.GameClasses.Match.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.WeatherObjects
{
    class LightningLine:Actor
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        public float Thickness;

        private Sprite[] sprites;

        /// <summary>
        /// Lightning line has multiple renderObjects, each representing a different part of a glowing horizontal line
        /// </summary>
        /// <param name="sprites"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="thickness"></param>
        public LightningLine(Vector2 startPoint, Vector2 endPoint, float thickness = 1) : base(null)
        {     
            StartPoint = startPoint;
            EndPoint = endPoint;
            Thickness = thickness;

            sprites = new Sprite[2];
            sprites[0] = new Sprite(LightningConfig.LIGHTNING_TEXTURES[0]);
            sprites[1] = new Sprite(LightningConfig.LIGHTNING_TEXTURES[1]);
        }

        public override void Draw(SpriteBatch batch, Color color)
        {
            Vector2 tangent = EndPoint - StartPoint;
            float rot = (float)Math.Atan2(tangent.Y, tangent.X);

            float ImageThickness = LightningConfig.IMAGE_THICKNESS;
            float thicknessScale = Thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(sprites[0].Texture.Width, sprites[0].Texture.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, sprites[1].Texture.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

            sprites[0].Draw(batch, StartPoint, color, rot, capOrigin, thicknessScale, SpriteEffects.None);
            sprites[1].Draw(batch, StartPoint, color, rot, middleOrigin, middleScale, SpriteEffects.None);
            sprites[0].Draw(batch, EndPoint, color, rot + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None);
        }

    }
}

