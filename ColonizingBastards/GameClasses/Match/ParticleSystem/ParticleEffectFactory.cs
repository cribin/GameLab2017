using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    abstract class ParticleEffectFactory
    {
        protected ParticleParameters ParticleParams { get; set; }
        protected ParticleParameters BaseParticleParams { get; set; }
        protected Random random;
        public Vector2 EmitterLocation { get; set; }
        protected int emitterRadius;
       
        public abstract ParticleParameters CreateParticleEffect();
    }
}
