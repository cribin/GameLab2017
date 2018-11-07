using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.PlayerInteraction
{
	abstract class InputMapper
	{
        public enum actions { };
        public abstract ActionSet MapInputs(InputState inputs);

	}
}
