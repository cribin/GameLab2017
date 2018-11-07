using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace ColonizingBastards.Base.Graphics
{
	class Animation
	{
		public string Name { get; protected set; }
		// Sorted List (<layer, Sequence>) of sequences with the sprite ids on the spritesheet, played sequentially
		public List<SpriteSequence> Layers { get; protected set; }
		// Indicates, if this animation should be restarted after ending
		public bool IsLoopable { get; protected set; }
		// (Maximal) Duration of the whole animation in milliseconds
		public int Duration { get; protected set; }


		public Animation(string name, int layer, int[] spriteSequence, int frameDurationMs, bool loopable)
		{
			this.Name = name;

			this.Layers = new List<SpriteSequence>();
			SpriteSequence newLayer = new SpriteSequence(layer, spriteSequence, new []{ frameDurationMs });		
			this.Layers.Add(newLayer);

			this.IsLoopable = loopable;

			this.Duration = frameDurationMs * spriteSequence.Length;
		}

		public Animation(string name, int layer, int[] spriteSequence, int[] frameDurationMs, bool loopable)
		{
			this.Name = name;

			this.Layers = new List<SpriteSequence>();
			SpriteSequence newLayer = new SpriteSequence(layer, spriteSequence, frameDurationMs);
			this.Layers.Add(newLayer);

			this.IsLoopable = loopable;

			if (newLayer.Duration > this.Duration)
				this.Duration = newLayer.Duration;
		}

		// Creates an animation object with the array indices of the spriteSequences/durations as layer indicator
		// (i.e. layer 0 => spriteSequence[0] (Background)
		public Animation(string name, int[][] spriteSequence, int[][] frameDurationMs, bool loopable)
		{
			this.Name = name;

			this.Layers = new List<SpriteSequence>();
			this.Duration = 0;
			this.IsLoopable = loopable;

			for (int i = 0; i < spriteSequence.Length; i++)
			{
				SpriteSequence newLayer = new SpriteSequence(i, spriteSequence[i], frameDurationMs[i]);
				this.Layers.Add(newLayer);

				if (newLayer.Duration > this.Duration)
					this.Duration = newLayer.Duration;
			}

			this.Layers.Sort();
		}

		public Animation(string name, List<Tuple<int, int[], int[]>> layerSpritesDurations, bool loopable)
		{
			this.Name = name;

			this.Layers = new List<SpriteSequence>();
			this.Duration = 0;
			this.IsLoopable = loopable;

			foreach (Tuple<int, int[], int[]> t in layerSpritesDurations)
			{
				SpriteSequence newLayer = new SpriteSequence(t.Item1, t.Item2, t.Item3);
				this.Layers.Add(newLayer);

				if (newLayer.Duration > this.Duration)
					this.Duration = newLayer.Duration;
			}

			this.Layers.Sort();
		}

		public bool AddNewLayer(int layer, int[] spriteSequence, int[] frameDurationMs)
		{
			foreach (SpriteSequence seq in Layers)
				if (seq.Layer == layer)
					return false;

			SpriteSequence newLayer = new SpriteSequence(layer, spriteSequence, frameDurationMs);
			Layers.Add(newLayer);

			Layers.Sort();

			if (newLayer.Duration > this.Duration)
				Duration = newLayer.Duration;

			return true;
		}

		public SpriteSequence GetLayer(int layer)
		{
			foreach (SpriteSequence seq in Layers)
				if (seq.Layer == layer)
					return seq;

			return null;
		}

		public bool RemoveLayer(int layer)
		{
			int i = Layers.FindIndex(s => s.Layer == layer);
			if (i >= 0)
			{
				Layers.RemoveAt(i);
				return true;
			}

			return false;
		}

		public bool ReplaceLayer(int layer, int[] spriteSequence, int[] frameFurationMs)
		{
			int i = Layers.FindIndex(s => s.Layer == layer);
			if (i >= 0)
			{
				Layers[i] = new SpriteSequence(layer, spriteSequence, frameFurationMs);

				if (Layers[i].Duration > this.Duration)
					Duration = Layers[i].Duration;
			}

			return false;
		}



		// Returns the id of sprite i on the specified layer
		public int GetSpriteId(int id, int layer)
		{
			if (id <= 0)
				return -1;

			foreach (SpriteSequence seq in Layers)
			{
				if (seq.Layer == layer && seq.SpriteIds.Length > id)
					return seq.SpriteIds[id];
			}

			return -1;
		}

		// Gets the layer and sprite id at the given position (between 0 and 1) in the animation
		public List<Tuple<int, int>> GetSpriteId(float playbackPosition)
		{
			List<Tuple<int, int>> res = new List<Tuple<int, int>>(Layers.Count);

			foreach (SpriteSequence seq in Layers)
			{
				int ind = CurrentIdFromPos(playbackPosition, seq);
				int sId = seq.SpriteIds[ind];
				res.Add(new Tuple<int, int>(seq.Layer, sId));
			}

			return res;
		}

		// Gets the sprite id on the specified layer at the given position (between 0 and 1)
		public int GetSpriteId(float playbackPosition, int layer)
		{
			if (playbackPosition <= 0)
				return -1;

			foreach (SpriteSequence seq in Layers)
			{
				if (seq.Layer == layer)
					return seq.SpriteIds[CurrentIdFromPos(playbackPosition, seq)];
			}

			return -1;
		}

		// Returns the duration in milliseconds of the frame i on the given layer in this animation
		public int GetDuration(int id, int layer)
		{
			if (id <= 0)
				return -1;

			foreach (SpriteSequence seq in Layers)
			{
				if (seq.Layer == layer && seq.SpriteIds.Length > id)
					return seq.MsPerFrame[id];
			}

			return -1;
		}

		// Gets the layer id and duration of the frame at the given position (between 0 and 1) of all layers 
		public Tuple<int, int>[] GetDuration(float playbackPosition)
		{
			throw new NotImplementedException();
		}

		// Gets the duration of the frame at the given position (between 0 and 1) on the specified layer
		public int GetDuration(float playbackPosition, int layer)
		{
			if (playbackPosition <= 0)
				return -1;

			foreach (SpriteSequence seq in Layers)
			{
				if (seq.Layer == layer)
					return seq.MsPerFrame[CurrentIdFromPos(playbackPosition, seq)];
			}

			return -1;
		}

		// Returns the index of the content arrays at the given position on the specified layer
		private int CurrentIdFromPos(float position, SpriteSequence seq)
		{
			
			if (position >= 1.0f)
				return seq.SpriteIds.Length - 1;

			// Current position in time (ms) in the current animation
			int curr = (int) Math.Round(position * this.Duration);

			if (curr >= seq.Duration)
			{
				if (IsLoopable)
					curr = curr % seq.Duration;
				else
					return seq.SpriteIds.Length - 1;
			}

			int nextF = 0;
			if (seq.MsPerFrame.Length == 1)
			{
				return (int) curr / seq.MsPerFrame[0];
			}
			else
			{
				for (int i = 0; i < seq.SpriteIds.Length; i++)
				{
					nextF += seq.MsPerFrame[i % seq.MsPerFrame.Length];
					if (curr < nextF)
						return i;
				}
			}

			// Something has gone wrong here (curr >= seq.Duration and loopable, but is not shorter than seq.Duration after modulo)
			return -1;
		}


		public class SpriteSequence : IComparable
		{
			// References the layer on which the animation is played (internally) - is sorted in Animation (background to foreground, increasing)
			public int Layer { get; set; }
			// Sequence of the ids referencing the sprites in the spritesheet
			public int[] SpriteIds { get; }
			// Duration of each corresponding frame (sprite) in milliseconds
			public int[] MsPerFrame { get; }
			// Length (in sprites) of the sequence
			public int Length { get; }
			// Total duration of the sequence (in ms)
			public int Duration { get; }

			public SpriteSequence(int layer, int[] spriteIds, int[] msPerFrame)
			{
				this.Layer = layer;
				this.SpriteIds = spriteIds;
				this.MsPerFrame = msPerFrame;
				this.Length = SpriteIds.Length;

				if (msPerFrame.Length == 1)
					this.Duration = spriteIds.Length * msPerFrame[0];
				else
				{
					int tmp = 0;
					foreach (int ms in msPerFrame)
						tmp += ms;
					this.Duration = tmp;
				}
			}

			public int CompareTo(object obj)
			{
				SpriteSequence objSeq = obj as SpriteSequence;
				;
				if (objSeq != null)
				{
					if (objSeq.Layer < this.Layer)
						return -1;
					else if (objSeq.Layer > this.Layer)
						return 1;
					else
						return 0;
				}

				throw new ArgumentException("Input object is not of type SpriteSequence or null!");
			}
		}

	}
}
