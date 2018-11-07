using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    public class ParticleParameters
    {
        //Time to live
        public int TTL { get; set; }

        public Vector2 Pos { get; set; }
        public Vector2 Vel { get; set; }
        public Vector2 Acc { get; set; }

        //Dampening simulates friction of the particle due to wind or other type of resistances
        //Value is between 0 and 1(no friction)
        public float VelDamp { get; set; }

        public float Rot { get; set; }
        public float RotVel { get; set; }
        public float RotDamp { get; set; }

        public float Scale { get; set; }
        public float ScaleVel { get; set; }
        public float ScaleAcc { get; set; }
        public float ScaleMax { get; set; }

        private Color finalCol;
        private Color initCol;
        public Color ParticleCol { get; set; }
        public Color InitCol { get { return initCol; } set { initCol = value; } }
        public Color FinalCol { get { return finalCol; } set { finalCol = value; } }
        //If TTL < colorFadeTime => linearly interpolate between initial and final color
        public int ColFadeTime { get; set; }

        //public ParticleParameters()

        public void SetParticleCol(Color particleCol)
        {
            this.ParticleCol = particleCol;
        }

        public void SetFinalColorAlpha(byte alpha)
        {
            this.finalCol.A = alpha;
        }

        public void SetInitColorAlpha(byte alpha)
        {
            this.initCol.A = alpha;
        }

    }

    internal class Particle:Actor
    {

        #region Properties

        public ParticleParameters ParticleParams { get; }

        private readonly Sprite currentSprite;

        #endregion

        #region Methods

        public Particle(RenderObject rep, ParticleParameters particleParams) : base(rep)
        {
            ParticleParams = particleParams;
            currentSprite = (Sprite) rep;
        }


        public override void Update(GameTime gameTime)
        {
            ParticleParams.TTL -= gameTime.ElapsedGameTime.Milliseconds;

            //Update Position using Integration
            ParticleParams.Vel *= ParticleParams.VelDamp;
            /*if (ParticleParams.Vel.X > 0)
                ParticleParams.Vel = new Vector2(ParticleParams.Vel.X + ParticleParams.Acc.X * (float) gameTime.ElapsedGameTime.TotalSeconds, ParticleParams.Vel.Y);

            if (ParticleParams.Vel.Y > 0)
                ParticleParams.Vel = new Vector2(ParticleParams.Vel.X, ParticleParams.Vel.Y + ParticleParams.Acc. * (float)gameTime.ElapsedGameTime.TotalSeconds);*/

            ParticleParams.Vel += ParticleParams.Acc * (float) gameTime.ElapsedGameTime.TotalSeconds;
            ParticleParams.Pos += ParticleParams.Vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update Angle using Integration 
            ParticleParams.Rot *= ParticleParams.RotDamp;
            ParticleParams.Rot += ParticleParams.RotVel * (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateScale(gameTime);
            UpdateColor(gameTime);
        }

        private void UpdateScale(GameTime gameTime)
        {
            ParticleParams.ScaleVel += ParticleParams.ScaleAcc * (float)gameTime.ElapsedGameTime.TotalSeconds;

            ParticleParams.Scale += ParticleParams.ScaleVel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ParticleParams.Scale = MathHelper.Clamp(ParticleParams.Scale, 0.0f, ParticleParams.ScaleMax);
        }

        private void UpdateColor(GameTime gameTime)
        {
            if (ParticleParams.TTL > ParticleParams.ColFadeTime && ParticleParams.ColFadeTime != 0)
                ParticleParams.ParticleCol = ParticleParams.InitCol;
            else
            {
                //Perform linear interpolation between init and final color
                float amtInit = (float)ParticleParams.TTL / (float)ParticleParams.ColFadeTime;
                float amtFinal = 1 - amtInit;

                Color tempColor = new Color
                {
                    R = (byte)(amtInit * ParticleParams.InitCol.R + amtFinal * ParticleParams.FinalCol.R),
                    G = (byte)(amtInit * ParticleParams.InitCol.G + amtFinal * ParticleParams.FinalCol.G),
                    B = (byte)(amtInit * ParticleParams.InitCol.B + amtFinal * ParticleParams.FinalCol.B),
                    A = (byte)(amtInit * ParticleParams.InitCol.A + amtFinal * ParticleParams.FinalCol.A)
                };

                ParticleParams.SetParticleCol(tempColor);
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            currentSprite.Draw(batch, ParticleParams);
        }

        #endregion

    }
}
