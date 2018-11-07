using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.GameClasses.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ColonizingBastards.GameClasses.Menu.MenuObjects
{
    class MenuInputMapper:InputMapper
    {

        public bool ListenToKeyboard { get; set; }

        public MenuInputMapper(bool listenToKeyboard) : base()
        {
            this.ListenToKeyboard = listenToKeyboard;
        }

        public override ActionSet MapInputs(InputState inputs)
        {
            ActionSet res = new ActionSet();

            if (ListenToKeyboard)
            {
                foreach (Keys key in inputs.KeyboardKeys)
                {
                    if (keyboardMap.ContainsKey(key))
                        res.actions.Add(keyboardMap[key]);
                }
            }

            foreach (Buttons b in inputs.ButtonsDown)
            {
                if (buttonMap.ContainsKey(b))
                    res.actions.Add(buttonMap[b]);
            }

            foreach (Tuple<InputState.AxisInputs, Vector2> i in inputs.axisInputs)
            {
                if (i.Item1 == InputState.AxisInputs.ThumbstickLeft || i.Item1 == InputState.AxisInputs.ThumbstickRight)
                {
                    Vector2 dir = i.Item2;
                    if (dir.X > 0)
                        res.actions.Add(InputConfig.Actions.RIGHT);
                    else if (dir.X < 0)
                        res.actions.Add(InputConfig.Actions.LEFT);

                    if (dir.Y > 0)
                        res.actions.Add(InputConfig.Actions.UP);
                    else if (dir.Y < 0)
                        res.actions.Add(InputConfig.Actions.DOWN);
                }
            }

            return res;
        }


        // Controls for Gamepads
        public Dictionary<Buttons, int> buttonMap = new Dictionary<Buttons, int>
        {
            { Buttons.A, InputConfig.Actions.SELECT },
            { Buttons.X, InputConfig.Actions.BACK },
            { Buttons.BigButton, InputConfig.Actions.EXIT },
			{ Buttons.Start, InputConfig.Actions.MENU },
			{ Buttons.Back, InputConfig.Actions.VIEW}
        };

        // Keyboard Control schema
        public Dictionary<Keys, int> keyboardMap = new Dictionary<Keys, int>
        {
            { Keys.Enter, InputConfig.Actions.SELECT },
            { Keys.Escape, InputConfig.Actions.EXIT },
			{ Keys.Space, InputConfig.Actions.MENU },
            { Keys.Back, InputConfig.Actions.BACK },
            { Keys.W, InputConfig.Actions.UP },
            { Keys.Up, InputConfig.Actions.UP },
            { Keys.A, InputConfig.Actions.LEFT },
            { Keys.Left, InputConfig.Actions.LEFT },
            { Keys.S, InputConfig.Actions.DOWN },
            { Keys.Down, InputConfig.Actions.DOWN },
            { Keys.D, InputConfig.Actions.RIGHT },
            { Keys.Right, InputConfig.Actions.RIGHT }
        };
        
    }
}
