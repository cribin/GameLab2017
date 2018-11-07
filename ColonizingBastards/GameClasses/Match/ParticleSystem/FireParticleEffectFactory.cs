using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    public enum FireParticleEffectType
    {
        Custom,
        Default
        //Some predefined fire effects
    }
    class FireParticleEffectFactory:ParticleEffectFactory
    {
        private FireParticleEffectType fireParticleEffectType;

        public FireParticleEffectFactory(FireParticleEffectType fireParticleEffectType, Vector2 emitterLocation,
            int emitterRadius)
        {
            random = new Random();
            this.fireParticleEffectType = fireParticleEffectType;
            this.EmitterLocation = emitterLocation;
            this.emitterRadius = emitterRadius;
        
            BaseParticleParams = new ParticleParameters();

            switch (fireParticleEffectType)
            {
                case  FireParticleEffectType.Default:
                    InitDefaultFireParticle();
                    break;
                default:
                    InitDefaultFireParticle();
                    break;

            }

        }

       /// <summary>
       /// Initilaizes a custom fire particle
       /// </summary>
       /// <param name="emitterLocation"></param>
       /// <param name="emitterRadius"></param>
       /// <param name="ttl">Time to live</param>
       /// <param name="maxVel"></param>
       /// <param name="maxAcc"></param>
       /// <param name="initScale"></param>
       /// <param name="scaleVel"></param>
       /// <param name="initColor"></param>
       /// <param name="finalColor"></param>
       /// <param name="colFadeTime">Time when intialCol should be faded into the final color</param>
       /// <param name="velDamp">0:full damp => no velocity, 1: no damp</param>
       /// <param name="initRot"></param>
       /// <param name="rotVel"></param>
       /// <param name="rotDamp">Same as velDamp but for rotation</param>
       /// <param name="scaleAcc"></param>
       /// <param name="scaleMax"></param>
       /// <param name="initColAlpha"></param>
       /// <param name="finalColAlpha"></param>
        public FireParticleEffectFactory(Vector2 emitterLocation, int emitterRadius, int maxTTL, Vector2 maxVel, Vector2 maxAcc,            
            float initScale, float scaleVel, Color initColor, Color finalColor, int colFadeTime,
             float velDamp = 1f, float initRot = 0f, float rotVel = 0f, float rotDamp = 1f, float scaleAcc = 0f, float scaleMax = 1, byte finalColAlpha = 0)
        {
            random = new Random();
            this.fireParticleEffectType = FireParticleEffectType.Custom;
            this.EmitterLocation = emitterLocation;
            this.emitterRadius = emitterRadius;

            BaseParticleParams = new ParticleParameters
            {
                TTL = maxTTL,
                Vel = maxVel,
                Acc = maxAcc,
                VelDamp = velDamp,
                Rot = initRot,           
                RotVel = rotVel,
                RotDamp = rotDamp,
                Scale = initScale,
                ScaleVel = scaleVel,
                ScaleAcc = scaleAcc,
                ScaleMax = scaleMax,
                InitCol = initColor,
                FinalCol = finalColor,
                ColFadeTime = colFadeTime
            };
          
            //set alpha value of the colors
            BaseParticleParams.SetFinalColorAlpha(finalColAlpha);
            

        }

        private void InitDefaultFireParticle()
        {
            BaseParticleParams.TTL = 3000;

            BaseParticleParams.VelDamp = 0.96f;

            BaseParticleParams.Rot = 0.0f;
            BaseParticleParams.RotVel = 0.0f;
            BaseParticleParams.RotDamp = 1.0f;

            BaseParticleParams.Scale = 0.5f;
            BaseParticleParams.ScaleVel = -0.1f;
            BaseParticleParams.ScaleAcc = 0.0f;
            BaseParticleParams.ScaleMax = 1.0f;

            BaseParticleParams.InitCol = Color.Red;
            BaseParticleParams.FinalCol = Color.Yellow;
            BaseParticleParams.SetFinalColorAlpha(0);
            BaseParticleParams.ColFadeTime = 2750;

        }

        public override ParticleParameters CreateParticleEffect()
        {
            InitParticleFromBase();

            switch (fireParticleEffectType)
            {
                case FireParticleEffectType.Custom:
                    SpawnCustomParticle();
                    break;
                case FireParticleEffectType.Default:
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
                TTL = BaseParticleParams.TTL,
                Vel = BaseParticleParams.Vel,
                Acc = BaseParticleParams.Acc,
                VelDamp = BaseParticleParams.VelDamp,
                Rot = BaseParticleParams.Rot,
                RotVel = BaseParticleParams.RotVel,
                RotDamp = BaseParticleParams.RotDamp,
                Scale = BaseParticleParams.Scale,
                ScaleVel = BaseParticleParams.ScaleVel,
                ScaleAcc = BaseParticleParams.ScaleAcc,
                ScaleMax = BaseParticleParams.ScaleMax,
                InitCol = BaseParticleParams.InitCol,
                FinalCol = BaseParticleParams.FinalCol,
                ColFadeTime = BaseParticleParams.ColFadeTime
            };

            ParticleParams.SetInitColorAlpha(BaseParticleParams.InitCol.A);
            ParticleParams.SetFinalColorAlpha(BaseParticleParams.FinalCol.A);

        }

        /// <summary>
        /// Init non static parameters of the particle(using random numbers)
        /// </summary>
        private void SpawnDefaultParticle()
        {
            Vector2 offset;
            Vector2 offset2;
            offset.X = ((float)(random.Next(emitterRadius) * Math.Cos(random.Next(360))));
            offset.Y = ((float)(random.Next(emitterRadius) * Math.Sin(random.Next(360))));
            ParticleParams.Pos = EmitterLocation + offset;
            ParticleParams.Vel = new Vector2(-offset.X * 0.5f, 0.0f);
            ParticleParams.Acc = new Vector2(0, -random.Next(400));
          

        }

        private void SpawnCustomParticle()
        {
            Vector2 offset;
            float rand = -ParticleParams.Acc.X + 2*random.Next((int)ParticleParams.Acc.X);
            ParticleParams.TTL = 2000 + random.Next(ParticleParams.TTL);
            ParticleParams.RotVel = 1f;
            offset.X = ((float)(random.Next(emitterRadius) * Math.Cos(random.Next(180))));
            offset.Y = ((float)(random.Next(emitterRadius) * Math.Sin(random.Next(180))));
            ParticleParams.Pos = EmitterLocation + offset;
            ParticleParams.Vel = new Vector2(-offset.X * ParticleParams.Vel.X, ParticleParams.Vel.Y);
            ParticleParams.Acc = new Vector2(rand, -random.Next((int)ParticleParams.Acc.Y));
         
        }

        
    }
}
