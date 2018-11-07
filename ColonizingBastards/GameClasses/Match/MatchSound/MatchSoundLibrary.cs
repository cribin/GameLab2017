using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.SoundUtil;
using ColonizingBastards.GameClasses.Config;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace ColonizingBastards.GameClasses.Match.MatchSound
{
	class SoundEffectLibrary : SoundEffectLibrary<SoundEffectEnumeration>
	{

		private Dictionary<SoundEffectEnumeration, SoundEffect> dict;

		public SoundEffectLibrary(ContentManager contentManager)
		{
			dict = new Dictionary<SoundEffectEnumeration, SoundEffect>();

			foreach (SoundEffectEnumeration e in Enum.GetValues(typeof(SoundEffectEnumeration)))
			{
				dict.Add(e, contentManager.Load<SoundEffect>(MainConfig.PIPELINE_SOUNDS_DIRECTORY + Enum.GetName(typeof(SoundEffectEnumeration), e)));
			}
		}
		

		public override SoundEffect GetSoundEffect(SoundEffectEnumeration e)
		{
			return dict[e];
		}

	}

	class SongLibrary : SongLibrary<SongEnumeration>
	{

		private Dictionary<SongEnumeration, Song> dict;

		public SongLibrary(ContentManager contentManager)
		{
			dict = new Dictionary<SongEnumeration, Song>();

			foreach (SongEnumeration e in Enum.GetValues(typeof(SongEnumeration)))
			{
				dict.Add(e, contentManager.Load<Song>(MainConfig.PIPELINE_SONGS_DIRECTORY + Enum.GetName(typeof(SongEnumeration), e)));
			}
		}

		public override Song GetSong(SongEnumeration e)
		{
			return dict[e];
		}

	}




}
