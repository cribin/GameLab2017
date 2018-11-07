using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.PlayerInteraction
{
    class InputState
    {

        public List<Buttons> ButtonsDown;
	    public List<Keys> KeyboardKeys;
        public List<Tuple<AxisInputs, Vector2>> axisInputs;

        public InputState()
        {
            ButtonsDown = new List<Buttons>();
			KeyboardKeys = new List<Keys>();
            axisInputs = new List<Tuple<AxisInputs, Vector2>>();
        }

        
        public enum AxisInputs{
            ThumbstickLeft,
            ThumbstickRight,
            Mouse
        };

    }
}
