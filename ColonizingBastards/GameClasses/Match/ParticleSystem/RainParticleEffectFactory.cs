using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    public enum RainParticleEffectType
    {
        Custom,
        Default
        //Some predefined rain effects
    }
    class RainParticleEffectFactory:ParticleEffectFactory
    {
        private RainParticleEffectType rainParticleEffectType;
        private Vector2 baseScreenSize;

        public RainParticleEffectFactory(RainParticleEffectType rainParticleEffectType, Vector2 emitterLocation,
            int emitterRadius, Vector2 baseScreenSize, float maxVelX = -400, float maxVelY = 800, float rot = (float)Math.PI / 4)
        {
            random = new Random();
            this.rainParticleEffectType = rainParticleEffectType;
            this.EmitterLocation = emitterLocation;
            this.emitterRadius = emitterRadius;

            this.baseScreenSize = baseScreenSize;

            BaseParticleParams = new ParticleParameters
            {
                Vel = new Vector2(maxVelX, maxVelY),
                Rot = rot
            };


            switch (rainParticleEffectType)
            {
                case RainParticleEffectType.Default:
                    InitDefaultRainParticle();
                    break;
                default:
                    InitDefaultRainParticle();
                    break;

            }

        }

        private void InitDefaultRainParticle()
        {
            BaseParticleParams.TTL = 1500;

            BaseParticleParams.Acc = Vector2.Zero;
            BaseParticleParams.VelDamp = 1f;

            BaseParticleParams.RotVel = 0.0f;
            BaseParticleParams.RotDamp = 1.0f;

            BaseParticleParams.ScaleVel = 0.0f;
            BaseParticleParams.ScaleAcc = 0.0f;
            BaseParticleParams.ScaleMax = 1.0f;

            BaseParticleParams.InitCol = Color.White;
            BaseParticleParams.FinalCol = Color.White;
            BaseParticleParams.SetFinalColorAlpha(0);
        }




        public override ParticleParameters CreateParticleEffect()
        {
            InitParticleFromBase();

            switch (rainParticleEffectType)
            {
                case RainParticleEffectType.Custom:
                    SpawnCustomParticle();
                    break;
                case RainParticleEffectType.Default:
                    SpawnDefaultParticle();
                    break;
                default:
                    SpawnDefaultParticle();
                    break;

            }

            return ParticleParams;
        }

        private void InitParticleFromBase()
        {
            ParticleParams = new ParticleParameters
            {
                Vel = BaseParticleParams.Vel,
                Acc = BaseParticleParams.Acc,
                VelDamp = BaseParticleParams.VelDamp,
                Rot = BaseParticleParams.Rot,
                RotVel = BaseParticleParams.RotVel,
                RotDamp = BaseParticleParams.RotDamp,
                ScaleVel = BaseParticleParams.ScaleVel,
                ScaleAcc = BaseParticleParams.ScaleAcc,
                ScaleMax = BaseParticleParams.ScaleMax,
                InitCol = BaseParticleParams.InitCol,
                FinalCol = BaseParticleParams.FinalCol
            };

            ParticleParams.SetInitColorAlpha(BaseParticleParams.InitCol.A);
            ParticleParams.SetFinalColorAlpha(BaseParticleParams.FinalCol.A);

        }

        /// <summary>
        /// Init non static parameters of the particle(using random numbers)
        /// </summary>
        private void SpawnDefaultParticle()
        {
            ParticleParams.Scale = 0.5f + ((float)random.Next(10)) / 20.0f;

            ParticleParams.TTL = (int)(BaseParticleParams.TTL / ParticleParams.Scale);

            float offsetX =  -Math.Sign(BaseParticleParams.Vel.X)*(ParticleParams.Rot / (float)(2 * Math.PI)) * baseScreenSize.X;
 
            Vector2 offset1 = Vector2.Zero, offset2 = Vector2.Zero;
            offset1.X = ((float)(random.Next(emitterRadius) * Math.Cos(random.Next(360))));
            offset1.Y = ((float)(random.Next(emitterRadius) * Math.Sin(random.Next(360))));
            //offset2.X += (float)(600 * Math.Cos(effectDuration / 500.0));
            ParticleParams.Pos = new Vector2(random.Next(2*(int)baseScreenSize.X )  +(int)offsetX, EmitterLocation.Y) + offset1;
            ParticleParams.Vel = new Vector2(BaseParticleParams.Vel.X, BaseParticleParams.Vel.Y * ParticleParams.Scale) ;

            ParticleParams.ColFadeTime = ParticleParams.TTL;

        }

        private void SpawnCustomParticle()
        {
            

        }

      
    }
}
