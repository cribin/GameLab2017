using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.Match.MatchObjects
{
	class Score
	{

		protected int points;

		public void addPoints(int points)
		{
			this.points += points;
		}

		public void reset()
		{
			points = 0;
		}

		public int getScore()
		{
			return points;
		}

	}
}
