using ColonizingBastards.Base.PlayerInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.Config
{
    static class InputConfig
    {

        public static class Actions
        {
            // Vector2
			public const int MOVE_DIRECTION = 1;
			public const int LOOK_DIRECTION = 3;
            public const int MOUSE_POSITION = 4;

            // Single actions
            public const int SELECT = 12;
            public const int EXIT = 13;
            public const int BACK = 14;
			public const int MENU = 15;
			public const int VIEW = 16;

            // Menu Single Actions
            public const int UP = 51;
            public const int LEFT = 52;
            public const int DOWN = 53;
            public const int RIGHT = 54;
            
            // Ingame Actions
            public const int IDLE = 100;
            public const int JUMP = 101;
            public const int COLLECT = 102;
	        public const int HIT = 103;
	        public const int SHOOT = 104;
	        public const int TRAP = 105;
        }


    }
}
