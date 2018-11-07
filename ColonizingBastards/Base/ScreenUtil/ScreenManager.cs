using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.ScreenUtil
{
	class ScreenManager
	{
        private Stack<Screen> screenStack;

        public ScreenManager()
		{
            screenStack = new Stack<Screen>();
		}


        public void AddScreen(Screen screen)
        {
            screenStack.Push(screen);
        }

        public void ReplaceCurrentScreen(Screen screen)
        {
            screenStack.Pop();
            screenStack.Push(screen);
        }

        public Screen GetCurrentScreen()
        {
            return screenStack.First();
        }

        public void ToLastScreen()
        {
            screenStack.Pop();
        }

        public void DrawCurrentScreen(SpriteBatch batch)
        {
            screenStack.First().Draw(batch);
        }

    }
}
