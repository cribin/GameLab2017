using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    class FoliageCutParticleEffectFactory:ParticleEffectFactory
    {
        public FoliageCutParticleEffectFactory(Vector2 emitterLocation, int emitterRadius, int maxTTL, Vector2 maxVel, Vector2 maxAcc,
            float initScale, float scaleVel, Color initColor, Color finalColor, int colFadeTime,
             float velDamp = 1f, float initRot = 0f, float rotVel = 0f, float rotDamp = 1f, float scaleAcc = 0f, float scaleMax = 1, byte finalColAlpha = 0)
        {
            
            random = new Random();
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

        private void InitParticleFromBase()
        {
            ParticleParams = new ParticleParameters
            {
                TTL = BaseParticleParams.TTL,
                Vel = BaseParticleParams.Vel,
                Acc = BaseParticleParams.Acc,
                VelDamp = BaseParticleParams.VelDamp,
                RotVel = BaseParticleParams.RotVel,
                RotDamp = BaseParticleParams.RotDamp,
                Scale = BaseParticleParams.Scale,
                ScaleAcc = BaseParticleParams.ScaleAcc,
                ScaleMax = BaseParticleParams.ScaleMax,
                InitCol = BaseParticleParams.InitCol,
                FinalCol = BaseParticleParams.FinalCol,
                ColFadeTime = BaseParticleParams.ColFadeTime
                
            };

            ParticleParams.SetInitColorAlpha(BaseParticleParams.InitCol.A);
            ParticleParams.SetFinalColorAlpha(BaseParticleParams.FinalCol.A);

        }

        public override ParticleParameters CreateParticleEffect()
        {
            InitParticleFromBase();

            SpawnParticle();

            return ParticleParams;
        }

        private void SpawnParticle()
        {
            Vector2 offset;
            Vector2 offset2;
            offset.X = (float)(random.Next(emitterRadius) * Math.Cos(random.Next(360)));
            offset.Y = (float)(random.Next(emitterRadius) * Math.Sin(random.Next(360)));
            ParticleParams.Pos = EmitterLocation;
            ParticleParams.Vel = new Vector2(offset.X * ParticleParams.Vel.X, offset.Y * ParticleParams.Vel.Y);

            ParticleParams.Rot = (float)(random.Next(360)*Math.PI/180);
        }
    }
}
