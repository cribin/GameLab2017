using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ColonizingBastards.Base.SoundUtil
{

	abstract class SoundEffectLibrary<E> where E : struct
	{

		public abstract SoundEffect GetSoundEffect(E e);

	}

	abstract class SongLibrary<E> where E : struct
	{

		public abstract Song GetSong(E e);

	}
}
