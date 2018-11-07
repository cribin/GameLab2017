using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.MatchObjects.WeatherObjects;
using ColonizingBastards.GameClasses.Match.MatchSound;
using ColonizingBastards.GameClasses.Match.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects
{
    class WeatherSystem
    {
        private Scene scene;
        private Vector2 baseScreenSize;
        private Random random;
        private RainParticleEffectFactory currentRainEffectFactory;
        private ParticleEffectManager currentRainEffectManager;

        private FogParticleEffectFactory currentFogEffectFactory;
        private ParticleEffectManager currentFogEffectManager;

        private BranchLightning branchLightning;

        public bool IsRaining { get; private set; }

        private float rainFrequencyS = 15;
        private float lightningFrequency = 5;

        private int[] newParticleAmounts;
        private bool heavyRain;

        private List<Foliage> foliages;

        public WeatherSystem(Scene scene, Vector2 baseScreenSize)
        {
            this.scene = scene;
            this.baseScreenSize = baseScreenSize;
            random = new Random();

            newParticleAmounts = new[] {15, 20, 25, 30, 35};
            heavyRain = false;
            foliages = scene.GetFoliage();
        }

        public void Update(GameTime gameTime)
        {
            rainFrequencyS -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            lightningFrequency -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            foliages.RemoveAll(f => !f.Active);
            if (rainFrequencyS <= 0)
            {
                if (currentRainEffectManager != null)
                {
                    scene.RemoveParticleEffect(currentRainEffectManager);
                }

                CreateRainEffect();
                scene.RegisterParticleEffect(currentRainEffectManager);
               
                rainFrequencyS = currentRainEffectManager.EffectDuration/1000 + 60 + random.Next(30);
            }

            if (currentRainEffectManager != null && currentRainEffectManager.EffectDuration <= 0)
            {
                IsRaining = false;
                //scene.RemoveParticleEffect(currentRainEffectManager);
                //currentRainEffectManager = null;

                /*if (currentFogEffectManager == null)
                {
                    CreateFogEffect();
                    scene.RegisterParticleEffect(currentFogEffectManager);
                }*/
            }
            else if(currentRainEffectManager != null && currentRainEffectManager.EffectDuration > 0)
            {
                IsRaining = true;
            }

            /*if(currentFogEffectManager == null)
            {
                CreateFogEffect();
                scene.RegisterParticleEffect(currentFogEffectManager);
            }*/
            if (currentFogEffectManager != null && currentFogEffectManager.EffectDuration <= 0)
            {
                scene.RemoveParticleEffect(currentFogEffectManager);
                currentFogEffectManager = null;
            }

            if (lightningFrequency <= 0)
            {
                if (IsRaining)
                {
                    CreateLightningEffect();
					scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Thunder);
                }

                lightningFrequency = 10 + random.Next(10);
            }

        }

        private void CreateRainEffect()
        {
            var duration = 10000 + random.Next(20000);
            var rainDir = random.Next(1,4);//1 <= rainDir < 4
            if (rainDir == 2)
                rainDir = 0;
            var rot = (float)(rainDir * Math.PI) / 4;
            var maxVelX = GetMaxVelX(rot);

            var newParticleAmount = newParticleAmounts[random.Next(newParticleAmounts.Length)];
            heavyRain = newParticleAmount > 20;
            currentRainEffectFactory = new RainParticleEffectFactory(RainParticleEffectType.Default, new Vector2(baseScreenSize.X / 2, -5), 20, baseScreenSize, maxVelX, rot:rot);
            currentRainEffectManager = new ParticleEffectManager(new List<RenderObject>() { new Sprite(ParticleConfig.RAIN_DROP)} , currentRainEffectFactory, newParticleAmount, 16, false, BlendState.NonPremultiplied, effectDuration:duration, baseScreenSize:baseScreenSize);
        }

        private void CreateFogEffect()
        {
            Rectangle emitterLocation = scene.ShopKeeperStartPos[1+random.Next(scene.ShopKeeperStartPos.Count-1)];
            int duration = 10000 + random.Next(20000);
            currentFogEffectFactory = new FogParticleEffectFactory(FogParticleEffectType.Default, new Vector2(emitterLocation.X + emitterLocation.Width, emitterLocation.Y - emitterLocation.Height / 4), 50, duration);
            currentFogEffectManager = new ParticleEffectManager(new List<RenderObject>() { new Sprite(ParticleConfig.WHITE_CIRCLE) }, currentFogEffectFactory, 2, 32, false, BlendState.NonPremultiplied, effectDuration:duration );
        }

        private void CreateLightningEffect()
        {
            float startX = random.Next((int)baseScreenSize.X);
            Vector2 endPos = new Vector2(random.Next((int)baseScreenSize.X), baseScreenSize.Y - 10);

            int burnFoliageProb = random.Next(3);
            if (burnFoliageProb < 2 && foliages.Count > 0)
            {
                Foliage chosenFoliage = foliages[random.Next(foliages.Count)];
                chosenFoliage.StartBurning();
                Vector3 tmp = chosenFoliage.GetPosition();
                Vector3 size = chosenFoliage.GetSize();
                if(chosenFoliage.Moving)
                    endPos = new Vector2(tmp.X, tmp.Y + size.Y);
                else
                    endPos = new Vector2(tmp.X, tmp.Y);
            }

            branchLightning = new BranchLightning(new Vector2(startX, -2),
               endPos);
            scene.LightningManager.CurrentLightnings.Add(branchLightning);
        } 

        private float GetMaxVelX(float angleRadian)
        {
            if (angleRadian > 0 && angleRadian < Math.PI / 2)
                return -400;
            else if (angleRadian == 0)
                return 0;
            else
                return 400;
        }

    }
}
