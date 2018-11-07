using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Cameras;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.ParticleSystem
{
    class ParticleEffectManager:Actor
    { 
         #region Fields

        protected List<RenderObject> CurrentSprites;
        protected Random Random;
        private List<Particle> particles;
        public bool Continuous { get;}
       
        //Duration in Milliseconds
        public int EffectDuration { get; private set; }
        //Indicates how many particles are created at each burst
        private int newParticleAmount;
        //Indicates how the new particleAmount changes after each frequency
        private int newParticleAmountDelta;
        //Length of time between burst
        private int burstFrequencyMs;
        private int burstCountDownMs;

        protected ParticleParameters ParticleParams;

        protected BlendState CurrentBlendState;

        private ParticleEffectFactory particleEffectFactory;

        public bool Paused { get; set; } = false;

        private Vector2 baseScreenSize;
        #endregion

        #region Properties


        #endregion

        #region Methods

        public ParticleEffectManager(List<RenderObject> rep, ParticleEffectFactory particleEffectFactory, int newParticleAmount, int burstFrequencyMs, bool continuous, BlendState blendState, int effectDuration = 0, Vector2 baseScreenSize = default(Vector2), int newParticleAmountDelta = 0) : base(rep[0])
        {                   
            CurrentSprites = rep;
            this.particleEffectFactory = particleEffectFactory;
            this.newParticleAmount = newParticleAmount;
            this.burstFrequencyMs = burstFrequencyMs;
            this.burstCountDownMs = 0;
            this.Continuous = continuous;
            if(!continuous)
                this.EffectDuration = effectDuration;

            CurrentBlendState = blendState;

            this.particles = new List<Particle>();
            Random = new Random();

            this.baseScreenSize = baseScreenSize;

            this.newParticleAmountDelta = newParticleAmountDelta;
        }

     
        public override void Update(GameTime gameTime)
        {
            //Create new particles if not paused
            if (!Paused)
            {
                EffectDuration -= gameTime.ElapsedGameTime.Milliseconds;
                burstCountDownMs -= gameTime.ElapsedGameTime.Milliseconds;

                if (burstCountDownMs <= 0 && (EffectDuration >= 0 || Continuous))
                {

                    for (int i = 0; i < newParticleAmount; i++)
                    {
                        ParticleParams = particleEffectFactory.CreateParticleEffect();
                        if (CurrentSprites.Count == 1)
                            particles.Add(new Particle(CurrentSprites[0], ParticleParams));
                        else
                        {
                            int pos = Random.Next(CurrentSprites.Count);
                            particles.Add(new Particle(CurrentSprites[pos], ParticleParams));
                        }
                    }

                    burstCountDownMs = burstFrequencyMs;

                    newParticleAmount += newParticleAmountDelta;
                }
            }

            //We move through the list backwards, so that particles are removed at the correct order
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);

                if (particles[i].ParticleParams.TTL <= 0 || particles[i].ParticleParams.Scale <= 0 || (baseScreenSize.Y > 0 && particles[i].ParticleParams.Pos.Y > baseScreenSize.Y))
                    particles.RemoveAt(i);
            }
        }

        //!!!!! Particle system call their own Begin and End Methods on the batch!!!!!
        public override void Draw(SpriteBatch batch, Camera camera)
        {
            if (Paused && particles.Count == 0) return;
            batch.Begin(SpriteSortMode.BackToFront, CurrentBlendState, transformMatrix: camera.GetViewMatrix());

            foreach (Particle p in particles)
            {
                p.Draw(batch);
            }

            batch.End();
        }

        #endregion

    }
}
