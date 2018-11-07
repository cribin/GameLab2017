using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Windows.Media.Audio;
using ColonizingBastards.Base.Objects;

namespace ColonizingBastards.Base.PlayerInteraction
{
	class Player : CharacterController
	{

		protected InputMapper inputMapper;
        public readonly PlayerIndex playerIndex;

	    private KeyboardState oldKeyboardState;
	    private GamePadState oldGamePadState;

        public ActionSet CurrentActions { get; protected set; }

	    private Scene.Scene scene;

		public Player(PlayerIndex index, InputMapper mapper, Scene.Scene scene = null) : base()
		{
            this.playerIndex = index;
            this.inputMapper = mapper;

            oldKeyboardState = new KeyboardState();
            oldGamePadState = new GamePadState();

		    this.scene = scene;
		}

        public PlayerIndex GetPlayerIndex()
        {
            return playerIndex;
        }

        protected ActionSet GetActions()
        {
            InputState inputs = new InputState();
            
            // Mouse
            if (Mouse.GetState() != null)
                inputs.axisInputs.Add(new Tuple<InputState.AxisInputs, Vector2>(
                    InputState.AxisInputs.Mouse,
                    new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));

			// Keyboard
	        if (Keyboard.GetState() != null)
	        {
		        KeyboardState state = Keyboard.GetState();

		        inputs.KeyboardKeys.AddRange(state.GetPressedKeys());
	        }

            // Gamepad
            GamePadCapabilities gCap = GamePad.GetCapabilities(playerIndex);
            if (gCap.IsConnected)
            {
                GamePadState state = GamePad.GetState(playerIndex);

                // Thumbsticks
                if (gCap.HasLeftXThumbStick && gCap.HasLeftYThumbStick)
                    inputs.axisInputs.Add(new Tuple<InputState.AxisInputs, Vector2>(
                        InputState.AxisInputs.ThumbstickLeft,
                        new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y)));

                if (gCap.HasRightXThumbStick && gCap.HasRightYThumbStick)
                    inputs.axisInputs.Add(new Tuple<InputState.AxisInputs, Vector2>(
                        InputState.AxisInputs.ThumbstickRight,
                        new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y)));

                // Buttons
                if (gCap.HasAButton)
                    if (state.IsButtonDown(Buttons.A))
                        inputs.ButtonsDown.Add(Buttons.A);

                if (gCap.HasBButton)
                    if (state.IsButtonDown(Buttons.B) && oldGamePadState.IsButtonUp(Buttons.B))
                        inputs.ButtonsDown.Add(Buttons.B);

                if (gCap.HasXButton)
					if (state.IsButtonDown(Buttons.X) && oldGamePadState.IsButtonUp(Buttons.X))
						inputs.ButtonsDown.Add(Buttons.X);

                if (gCap.HasYButton)
					if (state.IsButtonDown(Buttons.Y))
						inputs.ButtonsDown.Add(Buttons.Y);

				if (gCap.HasLeftShoulderButton)
					if (state.IsButtonDown(Buttons.LeftShoulder))
						inputs.ButtonsDown.Add(Buttons.LeftShoulder);

				if (gCap.HasRightShoulderButton)
					if (state.IsButtonDown(Buttons.RightShoulder))
						inputs.ButtonsDown.Add(Buttons.RightShoulder);

				if (gCap.HasLeftTrigger)
					if (state.IsButtonDown(Buttons.LeftTrigger))
						inputs.ButtonsDown.Add(Buttons.LeftTrigger);

				if (gCap.HasRightTrigger)
					if (state.IsButtonDown(Buttons.RightTrigger))
						inputs.ButtonsDown.Add(Buttons.RightTrigger);

				// Menu Buttons
				if (gCap.HasStartButton)
					if (state.IsButtonDown(Buttons.Start) && oldGamePadState.IsButtonUp(Buttons.Start))
						inputs.ButtonsDown.Add(Buttons.Start);

				if (gCap.HasBackButton)
					if (state.IsButtonDown(Buttons.Back))
						inputs.ButtonsDown.Add(Buttons.Back);

				if (gCap.HasBigButton)
					if (state.IsButtonDown(Buttons.BigButton))
						inputs.ButtonsDown.Add(Buttons.BigButton);

                oldGamePadState = state;
            }

            CurrentActions = inputMapper.MapInputs(inputs);
            return  new ActionSet(CurrentActions);
        }

        // Returns a clone of the actions updated in "getActions"
        public ActionSet GetCurrentActionSet()
        {
            return new ActionSet(CurrentActions);
        }

		public override void Update(GameTime gameTime)
		{
			ActionSet actions = GetActions();

		    if (scene != null && scene.MatchPaused) return;

			foreach (Character character in PossesedCharacters)
			{
				character.Update(gameTime, actions);
			}
		}

		public override void Possess(Character character)
		{
			this.PossesedCharacters.Add(character);
			character.AddPossession(this);
		}
	}
}
