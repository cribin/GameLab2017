using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.LogicUtil
{
	class LogicManager
	{

		Stack<Logic> logicStack;

		public LogicManager()
		{
            logicStack = new Stack<Logic>();
		}

		public void AddLogic(Logic logic)
		{
			logicStack.Push(logic);
		}

		public void ReplaceCurrentLogic(Logic logic)
		{
			logicStack.Pop();
			logicStack.Push(logic);
		}

        public Logic GetCurrentLogic()
        {
            return logicStack.First();
        }

		public void ToLastLogic()
		{
			logicStack.Pop();
		}

        public void UpdateCurrentLogic(GameTime gameTime)
        {
            logicStack.First().Update(gameTime);
        }

	}
}
