using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.GameEvent;
using ColonizingBastards.GameClasses.Match.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation
{
	class Foliage : Actor
	{

		public readonly GameEventList CharacterCutEventList;

		public bool Active { get; set; }
        public bool Moving { get; set; }

	    private Scene scene;
	    private Spritesheet spritesheet;
	    private int animDuration;
	    private float animRestartCounter;

		public Vector3 PositionForContent;

	    private float burnDuration;

	    private Random random;
	    private int animNumber;

        public Foliage(RenderObject rep, Scene scene, Vector3 positionForContent) : base(rep)
		{
			CharacterCutEventList = new GameEventList();
			Active = true;
		    this.scene = scene;
			this.random = scene.Random;

            spritesheet = rep as Spritesheet;
		    Moving = spritesheet != null;

			this.PositionForContent = positionForContent;

			if (Moving)
		    {
                animNumber = 1 + random.Next(3);
                animDuration = spritesheet.GetAnimation("Undergrowth0" + animNumber).Duration + 1000 + random.Next(3000);
		        animRestartCounter = 0f;
		    }


		}

	    public override void Update(GameTime gameTime)
	    {
	        if (Active)
	        {
	            if (Moving)
	            {
	                animRestartCounter -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;
	                if (animRestartCounter <= 0)
	                {
	                    animNumber = 1 + random.Next(3);
	                    spritesheet.RestartAnimation();
	                    spritesheet.PlayAnimation("Undergrowth0" + animNumber);
	                    animRestartCounter = animDuration;
	                }
	            }

	            if (burnDuration > 0)
	            {
	                burnDuration -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;
	                if (burnDuration <= 0)
	                {
	                    Active = false;
	                    CharacterCutEventList.Execute(new List<object>() {null}, new List<object>() {this});
	                }
	            }

	            base.Update(gameTime);
	        }
	    }

	    public override void Draw(SpriteBatch batch)
		{
		    if (Active)
		    {
		        base.Draw(batch);
		    }
		}

		public void CharacterCut(Character actuatingCharacter)
		{
		    if (!Active) return;
		    Active = false;
		    CharacterCutEventList.Execute(new List<object>(){ actuatingCharacter }, new List<object>() { this });	
		    //show particle effect

		   FoliageCutParticleEffectFactory foliageCutFactory = 
		        new FoliageCutParticleEffectFactory(new Vector2(position.X, position.Y - size.Y/2), (int)size.Length(), 500, new Vector2(5, 5), Vector2.Zero, 1f, 0f, Color.White, Color.White, 500, rotVel:2f);
		    ParticleEffectManager foliageCutEffectManager = new ParticleEffectManager(ParticleConfig.FOLIAGE_CUT_TEXTURE, foliageCutFactory, 1, 128, false, BlendState.NonPremultiplied, effectDuration:500);
		    scene.ParticleEffects.Add(foliageCutEffectManager);
		}

	    public void StartBurning()
	    {
	        Vector3 centerPos = GetCenterPosition();
	        float endPosY;
	        if (Moving)
	            endPosY = centerPos.Y;
	        else
	            endPosY = centerPos.Y - size.Y;
            //ParticleEffectFactory fireParticleEffectFactory = new FireParticleEffectFactory(FireParticleEffectType.Default, new Vector2(centerPos.X, centerPos.Y), (int)size.Length());
            ParticleEffectFactory fireParticleEffectFactory = new FireParticleEffectFactory(new Vector2(centerPos.X, endPosY), 3, 2000, new Vector2(0.3f,0f), 
            new Vector2(60f, 400), 0.35f, -0.2f, Color.Red, Color.Yellow, 1750, 0.9f);
            ParticleEffectManager fireParticleEffectManager = new ParticleEffectManager(new List<RenderObject>() { new Sprite(ParticleConfig.WHITE_CIRCLE)}, fireParticleEffectFactory, 5, 16, false, BlendState.Additive, effectDuration:5000);
            scene.RegisterParticleEffect(fireParticleEffectManager);
	        burnDuration = fireParticleEffectManager.EffectDuration + 100;
	    }
		
	}
}
