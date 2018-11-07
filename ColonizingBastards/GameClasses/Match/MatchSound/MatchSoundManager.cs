using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.SoundUtil;
using Microsoft.Xna.Framework.Content;

namespace ColonizingBastards.GameClasses.Match.MatchSound
{
	class MatchSoundManager : SoundManager<SoundEffectEnumeration, SongEnumeration>
	{

		public MatchSoundManager(ContentManager contentManager) : base(new SoundEffectLibrary(contentManager), new SongLibrary(contentManager))
		{
			
		}

	}
}
