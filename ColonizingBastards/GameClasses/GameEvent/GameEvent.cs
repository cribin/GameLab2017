using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.GameEvent
{
	abstract class GameEvent
	{

		public bool Active;

		public bool ActiveAfterExecution;

		public bool RemoveWhenBecomesInactive;

		protected GameEvent(bool ActiveAfterExecution, bool RemoveWhenBecomesInactive)
		{
			this.Active = true;
			this.ActiveAfterExecution = ActiveAfterExecution;
			this.RemoveWhenBecomesInactive = RemoveWhenBecomesInactive;
		}

		public abstract void execute(List<object> actuator, List<object> target);

	}
}
