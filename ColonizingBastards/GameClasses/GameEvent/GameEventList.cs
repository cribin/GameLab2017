using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.GameEvent
{    /// <summary>
	 /// This class is a helper intended to simplify handling lists of events.
	 /// Event flags will be properly respected.
	 /// </summary>
	class GameEventList
	{
		public readonly List<GameEvent> EventList;
		private List<GameEvent> pendingRemovals;

		public GameEventList(List<GameEvent> eventList)
		{
			this.EventList = eventList;
			pendingRemovals = new List<GameEvent>();
		}

		public GameEventList(GameEvent gameEvent)
		{
			EventList = new List<GameEvent>();
			EventList.Add(gameEvent);
			pendingRemovals = new List<GameEvent>();
		}

		public GameEventList()
		{
			 EventList = new List<GameEvent>();
			pendingRemovals = new List<GameEvent>();
		}

		public void Execute(List<object> actuator, List<object> target)
		{
			pendingRemovals.Clear();
			foreach (GameEvent gameEvent in EventList)
			{
				if (gameEvent.Active)
				{
					gameEvent.execute(actuator, target);
				}
				gameEvent.Active = gameEvent.ActiveAfterExecution;
				if (gameEvent.RemoveWhenBecomesInactive && !gameEvent.Active)
				{
					pendingRemovals.Add(gameEvent);
				}
			}

			foreach (GameEvent pendingRemovalGameEvent in pendingRemovals)
			{
				EventList.Remove(pendingRemovalGameEvent);
			}
		}

	}
}
