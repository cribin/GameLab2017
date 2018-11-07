using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    public enum FogParticleEffectType
    {
        Custom,
        Default
        //Some predefined fog effects
    }

    class FogParticleEffectFactory:ParticleEffectFactory
    {
        private FogParticleEffectType fogParticleEffectType;
        private int duration;

        public FogParticleEffectFactory(FogParticleEffectType fogParticleEffectType, Vector2 emitterLocation,
            int emitterRadius, int duration)
        {
            random = new Random();
            this.fogParticleEffectType = fogParticleEffectType;
            this.EmitterLocation = emitterLocation;
            this.emitterRadius = emitterRadius;
            this.duration = duration;

            BaseParticleParams = new ParticleParameters();

            switch (fogParticleEffectType)
            {
                case FogParticleEffectType.Default:
                    InitDefaultFogParticle();
                    break;
                default:
                    InitDefaultFogParticle();
                    break;
            }
        }

        private void InitDefaultFogParticle()
        {
            BaseParticleParams.TTL = 5000;

            BaseParticleParams.VelDamp = 1f;

            BaseParticleParams.Rot = 0.0f;
            BaseParticleParams.RotVel = 0.0f;
            BaseParticleParams.RotDamp = 1.0f;

            BaseParticleParams.Scale = 0.6f;
            BaseParticleParams.ScaleAcc = 0.0f;
            BaseParticleParams.ScaleMax = 3.0f;

            BaseParticleParams.InitCol = Color.White;
            BaseParticleParams.SetInitColorAlpha(4);
            BaseParticleParams.FinalCol = Color.White;//new Color(32, 32, 32);
            BaseParticleParams.SetFinalColorAlpha(0);
        }

        public override ParticleParameters CreateParticleEffect()
        {
            InitParticleFromBase();

            switch (fogParticleEffectType)
            {
                case FogParticleEffectType.Custom:
                    SpawnCustomParticle();
                    break;
                case FogParticleEffectType.Default:
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
                VelDamp = BaseParticleParams.VelDamp,
                Rot = BaseParticleParams.Rot,
                RotVel = BaseParticleParams.RotVel,
                RotDamp = BaseParticleParams.RotDamp,
                Scale = BaseParticleParams.Scale,
                ScaleAcc = BaseParticleParams.ScaleAcc,
                ScaleMax = BaseParticleParams.ScaleMax,
                InitCol = BaseParticleParams.InitCol,
                FinalCol = BaseParticleParams.FinalCol
            };

            ParticleParams.SetInitColorAlpha(BaseParticleParams.InitCol.A);
            ParticleParams.SetFinalColorAlpha(BaseParticleParams.FinalCol.A);

        }

        private void SpawnDefaultParticle()
        {
            ParticleParams.TTL = BaseParticleParams.TTL + random.Next(5000);

            Vector2 offset1 = Vector2.Zero, offset2 = Vector2.Zero;
            offset1.X = ((float)(random.Next(emitterRadius) * Math.Cos(random.Next(360))));
            offset1.Y = ((float)(random.Next(emitterRadius) * Math.Sin(random.Next(360))));
            //offset2.X += (float)(400 * Math.Cos(duration / 500.0));
            ParticleParams.Pos = EmitterLocation + offset1 + offset2;
            ParticleParams.Vel = new Vector2(-20 - random.Next(20), 0f);//new Vector2(0f, -30 - random.Next(30)));
            ParticleParams.Acc = new Vector2(0, -2 + random.Next(4));

            ParticleParams.ScaleVel = random.Next(10) / 50.0f;

            ParticleParams.ColFadeTime = ParticleParams.TTL - random.Next(5000);

        }

        private void SpawnCustomParticle()
        {


        }
    }
}
