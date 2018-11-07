using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.Graphics
{
	class Spritesheet : RenderObject
	{
		// Graphics
		public Texture2D Texture { get; }
		public int Rows { get; } = 0;
		public int Columns { get; } = 0;

		// Animation logistics
		private Dictionary<string, Animation> Animations;
		public float AnimPlaybackSpeed { get; set; } = 1.0f;
		private List<int> animCurrentSprites;
        private bool[] animLayerHidden;
		private Animation[] animCurrent;
		private float[] animCurrentPosition;
		private SpriteEffects animCurrentEffect = SpriteEffects.None;

		// Sprite logistics
		private Dictionary<string, int> Sprites;

		public int SpriteHeight { get; }
		public int SpriteWidth { get; }

		private Rectangle sourceRect;
		private Rectangle destRect;


		public Spritesheet(Texture2D texture, int numRows, int numColumns, int layers)
		{
			this.Texture = texture;
			this.Rows = numRows;
			this.Columns = numColumns;

			this.SpriteHeight = texture.Height / numRows;
			this.SpriteWidth = texture.Width / numColumns;
			this.sourceRect = new Rectangle(0, 0, SpriteWidth, SpriteHeight);
			this.destRect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
          
            this.Animations = new Dictionary<string, Animation>();

            this.animCurrent = new Animation[layers];
            this.animCurrentPosition = new float[layers];
            this.animLayerHidden = new bool[layers];
			this.animCurrentSprites = new List<int>();
             
		}

		public Spritesheet(Texture2D texture, int numRows, int numColumns, int spriteWidth, int spriteHeight, int layers)
		{
			this.Texture = texture;
			this.Rows = numRows;
			this.Columns = numColumns;

			this.SpriteHeight = spriteHeight;
			this.SpriteWidth = spriteWidth;
			this.sourceRect = new Rectangle(0, 0, SpriteWidth, SpriteHeight);
			this.destRect = new Rectangle(0, 0, (int)size.X, (int)size.Y);

			this.Animations = new Dictionary<string, Animation>();

            this.animCurrent = new Animation[layers];
            this.animCurrentPosition = new float[layers];
            this.animLayerHidden = new bool[layers];
			this.animCurrentSprites = new List<int>();

        }

		public override void Update(GameTime gameTime)
		{
			if (animCurrent == null || animCurrentSprites == null)
				return;

			animCurrentSprites.Clear();

			for (int i = 0; i < animCurrent.Length; i++)
            {
                if (animCurrent[i] == null || animLayerHidden[i])
                    continue;
                Animation a = animCurrent[i];          

                animCurrentPosition[i] += ((AnimPlaybackSpeed * gameTime.ElapsedGameTime.Milliseconds) / a.Duration);

                if (animCurrentPosition[i] > 1.0f)
                    animCurrentPosition[i] = (a.IsLoopable) ? animCurrentPosition[i] - ((int)animCurrentPosition[i]) : 1.0f;

				animCurrentSprites.Add(animCurrent[i].GetSpriteId(animCurrentPosition[i], i));

            }
        }

		public override void Draw(SpriteBatch batch)
		{
			if (animCurrentSprites == null || animCurrent == null)
				return;

			for (int i = 0; i < animCurrentSprites.Count; i++)
			{
				Point srcPos = GetPositionOfSprite(animCurrentSprites[i]);
				sourceRect.X = (int)srcPos.X;
				sourceRect.Y = (int)srcPos.Y;

				destRect.X = (int)position.X;
				destRect.Y = (int)position.Y;
				if (animCurrentEffect == SpriteEffects.None)
					batch.Draw(Texture, destRect, sourceRect, Color.White);
				else
				{
					batch.Draw(Texture, destRect, sourceRect, Color.White, 0, Vector2.Zero, animCurrentEffect, 0);
				}
			}
		}

		public override void SetPosition(Vector3 position)
		{
			this.position = position;
		}

		public override void SetRotation(Vector3 rotation)
		{
			this.rotation = rotation.Z;
		}

		public override void SetSize(Vector3 size)
		{
			this.size = size;
            this.destRect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
        }

		// Animation utils

		public bool AddAnimation(Animation anim)
		{
			if (Animations.ContainsKey(anim.Name))
				return false;

			Animations.Add(anim.Name, anim);

			return true;
		}

		public bool RemoveAnimation(string name)
		{
			if (!Animations.ContainsKey(name))
				return false;

			Animations.Remove(name);

			return true;
		}

		public bool RemoveAnimation(Animation anim)
		{
			if (!Animations.ContainsKey(anim.Name))
				return false;

			Animations.Remove(anim.Name);

			return true;
		}

		public bool ContainsAnimation(string name)
		{
			return Animations.ContainsKey(name);
		}

		public bool ContainsAnimation(Animation anim)
		{
			return Animations.ContainsKey(anim.Name);
		}

		public Animation GetAnimation(string name)
		{
			return Animations[name];
		}


		public bool PlayAnimation(Animation anim, float playbackSpeed = 1.0f, bool flipHorizontal = false, bool flipVertical = false)
		{
			return PlayAnimation(anim.Name, playbackSpeed, flipHorizontal, flipVertical);
		}

		public bool PlayAnimation(String name, float playbackSpeed = 1.0f, bool flipHorizontal = false, bool flipVertical = false)
		{
			if (!Animations.ContainsKey(name))
				return false;

			Animation anim = Animations[name];

            foreach (Animation.SpriteSequence seq in anim.Layers)
            {
                Animation curr = animCurrent[seq.Layer];
                if (curr == null || !curr.Name.Equals(name))
                {
                    animCurrent[seq.Layer] = anim;
                    animCurrentPosition[seq.Layer] = 0;
                }
            
            }

			AnimPlaybackSpeed = playbackSpeed;
			
			if (flipHorizontal && flipVertical)
				animCurrentEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
			else if (flipHorizontal)
				animCurrentEffect = SpriteEffects.FlipHorizontally;
			else if (flipVertical)
				animCurrentEffect = SpriteEffects.FlipVertically;
			else
				animCurrentEffect = SpriteEffects.None;

			return true;
		}

		public void PauseAnimation()
		{
			AnimPlaybackSpeed = 0;
		}

		public void ResumeAnimation(float playbackSpeed = 1.0f)
		{
			AnimPlaybackSpeed = playbackSpeed;
		}

		public void RestartAnimation(float playbackSpeed = 1.0f)
		{
			for (int i = 0; i < animCurrentPosition.Length; i++)
				animCurrentPosition[i] = 0;

			AnimPlaybackSpeed = playbackSpeed;
		}

		public bool SetAnimation(Animation anim)
		{
			return SetAnimation(anim.Name);
		}

		public bool SetAnimation(string name)
		{
			if (!Animations.ContainsKey(name))
				return false;

			Animation anim = Animations[name];

			foreach (Animation.SpriteSequence seq in anim.Layers)
			{
				animCurrent[seq.Layer] = anim;
			}

			return true;
		}

        public bool SetVisibilityLayer(int layer, bool visible)
        {
	        if (layer < 0 || layer >= animLayerHidden.Length)
		        return false;

            animLayerHidden[layer] = !visible;
	        return true;
        }

		public bool SetVisibilityAnimation(String name, bool visible)
		{
			bool succ = false;

			for (int i = 0; i < animCurrent.Length; i++)
			{
				if (animCurrent[i].Name.Equals(name))
				{
					animLayerHidden[i] = !visible;
					succ = true;
				}
			}

			return succ;
		}


		// Misc functions
		private Point GetPositionOfSprite(int id)
		{
			int row = id / Columns;
			int col = id % Columns;
			return new Point(col * SpriteWidth, row * SpriteHeight);
		}

	}
}
