using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.GameClasses.Match;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ColonizingBastards.Base.SoundUtil
{
	class SoundManager<E, F> 
		where E : struct
		where F : struct
	{

		public SoundEffectLibrary<E> SoundEffectLibrary { get; }
		public SongLibrary<F> SongLibrary { get; }

		public bool IsMuted { get; set; }

		private Song currentSong;

		public SoundManager(SoundEffectLibrary<E> soundEffectLibrary, SongLibrary<F> songLibrary)
		{
			this.SoundEffectLibrary = soundEffectLibrary;
			this.SongLibrary = songLibrary;
		}
		

		public void PlaySoundEffect(E soundEffectReference)
		{
			if (!IsMuted)
			{
				SoundEffectLibrary.GetSoundEffect(soundEffectReference).Play();			
			}
		}

		public SoundEffectInstance GetSoundEffectInstance(E soundEffectReference, bool startPlayback = true)
		{
			SoundEffectInstance ret = SoundEffectLibrary.GetSoundEffect(soundEffectReference).CreateInstance();
			if (startPlayback && !IsMuted)
			{
				ret.Play();
			}
			return ret;
		}

		public void PlaySong(F songReference)
		{
			currentSong = SongLibrary.GetSong(songReference);
			if (!IsMuted)
			{	
				MediaPlayer.Play(currentSong);
			}
			MediaPlayer.IsRepeating = true;
		}

		public void PauseSong()
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				MediaPlayer.Pause();
			}
		}

		public void ResumeSong()
		{
			if (MediaPlayer.State == MediaState.Paused)
			{
				if (!IsMuted)
				{
					MediaPlayer.Resume();
				}
			}
		}

		public void StopSong()
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				MediaPlayer.Stop();
			}
		}
	}

}
