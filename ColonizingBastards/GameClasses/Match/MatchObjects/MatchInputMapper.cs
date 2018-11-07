using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.GameClasses.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.Match.MatchObjects
{
    class MatchInputMapper : InputMapper
    {
	    public bool ListenToKeyboard { get; set; }
		public int KeyboardControlSchema { get; set; } 

	    public MatchInputMapper(bool listenToKeyboard, int keyboardControlSchema) : base()
	    {
		    this.ListenToKeyboard = listenToKeyboard;
		    if (keyboardControlSchema >= 1 && keyboardControlSchema <= KEYBOARD_SCHEME_NUMBER)
			    this.KeyboardControlSchema = keyboardControlSchema;
			
	    }

        public override ActionSet MapInputs(InputState inputs)
        {
            ActionSet res = new ActionSet();

			if (ListenToKeyboard && (KeyboardControlSchema >= 1 && KeyboardControlSchema <= KEYBOARD_SCHEME_NUMBER))
			{
				Dictionary<Keys, int> dict = KEYBOARD_DICTIONARIES[KeyboardControlSchema - 1];
				foreach (Keys key in inputs.KeyboardKeys)
				{
					if (dict.ContainsKey(key))
						res.actions.Add(dict[key]);
				}

				Vector2 dir = KEYBOARD_DIRECTION(KEYBOARD_DIRECTION_CONTROLS[KeyboardControlSchema - 1], inputs.KeyboardKeys);
				if (dir.X != 0 || dir.Y != 0)
				{
					res.axisActions.Add(new Tuple<int, Vector2>(InputConfig.Actions.MOVE_DIRECTION, dir));
					res.axisActions.Add(new Tuple<int, Vector2>(InputConfig.Actions.LOOK_DIRECTION, dir));
				}

			}

			foreach (Buttons b in inputs.ButtonsDown)
            {
                if (buttonMap.ContainsKey(b))
                    res.actions.Add(buttonMap[b]);
            }

            foreach (Tuple <InputState.AxisInputs, Vector2> i in inputs.axisInputs)
            {
                if (axisMap.ContainsKey(i.Item1))
                    res.axisActions.Add(new Tuple<int, Vector2>(axisMap[i.Item1], i.Item2));
            }

            return res;

        }

		// Controls for Gamepads

		public Dictionary<Buttons, int> buttonMap = new Dictionary<Buttons, int>
		{
			{ Buttons.A, (int) InputConfig.Actions.JUMP },
			{ Buttons.X, (int) InputConfig.Actions.HIT },
			{ Buttons.B, (int) InputConfig.Actions.TRAP },
			{ Buttons.Y, (int) InputConfig.Actions.COLLECT },
			{ Buttons.RightShoulder, (int)  InputConfig.Actions.SHOOT},
            { Buttons.Start, (int)  InputConfig.Actions.MENU}
        };

		public Dictionary<InputState.AxisInputs, int> axisMap = new Dictionary<InputState.AxisInputs, int>
		{
			{ InputState.AxisInputs.Mouse, (int) InputConfig.Actions.MOUSE_POSITION },
			{ InputState.AxisInputs.ThumbstickLeft, (int) InputConfig.Actions.MOVE_DIRECTION },
			{ InputState.AxisInputs.ThumbstickRight, (int) InputConfig.Actions.LOOK_DIRECTION }
		};

		// Keyboard Control schemas (static!), same for every player
		public const int KEYBOARD_SCHEME_NUMBER = 2;

		public static readonly Dictionary<Keys, int> KEYBOARD_ACTION_SCHEMA_1 = new Dictionary<Keys, int>
	    {
			{ Keys.Space, (int) InputConfig.Actions.JUMP },
			{ Keys.E, (int) InputConfig.Actions.HIT },
			{ Keys.LeftShift, (int) InputConfig.Actions.SHOOT },
			{ Keys.Q, (int) InputConfig.Actions.COLLECT }
		};

		public static readonly Dictionary<Keys, int> KEYBOARD_ACTION_SCHEMA_2 = new Dictionary<Keys, int>
		{
			{ Keys.Back, (int) InputConfig.Actions.JUMP },
			{ Keys.OemPipe, (int) InputConfig.Actions.HIT },
			{ Keys.Enter, (int) InputConfig.Actions.SHOOT },
			{ Keys.Oem8, (int) InputConfig.Actions.COLLECT }
		};

		public static readonly Dictionary<Keys, int>[] KEYBOARD_DICTIONARIES = new Dictionary<Keys, int>[] { KEYBOARD_ACTION_SCHEMA_1, KEYBOARD_ACTION_SCHEMA_2 };

		
		public static readonly Keys[] KEYBOARD_DIRECTION_SCHEMA_1 = { Keys.W, Keys.A, Keys.S, Keys.D };
		public static readonly Keys[] KEYBOARD_DIRECTION_SCHEMA_2 = { Keys.Up, Keys.Left, Keys.Down, Keys.Right };
		public static readonly Keys[][] KEYBOARD_DIRECTION_CONTROLS = new Keys[][] { KEYBOARD_DIRECTION_SCHEMA_1, KEYBOARD_DIRECTION_SCHEMA_2 };

		// KeysFourDirections is [UP, LEFT, DOWN, RIGHT], e.g. W, A, S, D
		public static Vector2 KEYBOARD_DIRECTION(Keys[] keysFourDirections, List<Keys> keysPressed)
	    {
		    float x = 0;
		    float y = 0;

		    foreach (Keys key in keysPressed)
		    {
			    if (key == keysFourDirections[0])
				    y += 1.0f;
				if (key == keysFourDirections[1])
					x -= 1.0f;
				if (key == keysFourDirections[2])
					y -= 1.0f;
				if (key == keysFourDirections[3])
					x += 1.0f;
			}

		    return new Vector2(x, y);
	    }


	}

}
