using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    public static class InputManager
    {
        public static InputState[] InputState = new InputState[4];

        private static PlayerIndex PlayerIndex = PlayerIndex.One;

        public static KeyboardState CurrentKeyboardState
        {
            get { return InputState[0].KeyboardState; }
        }

        public static InputState SinglePlayer
        {
            get { return InputState[0]; }
        }

        public static GamePadState CurrentGamePadState
        {
            get { return InputState[0].GamePadState; }
        }
        public static Dictionary<Keys, Buttons>
            InputMap = new Dictionary<Keys, Buttons>()
                {
                    {Keys.Enter, Buttons.Start},
                    {Keys.Up, Buttons.DPadUp},
                    {Keys.Down, Buttons.DPadDown},
                    {Keys.Left, Buttons.DPadLeft},
                    {Keys.Right, Buttons.DPadRight},
                    {Keys.Space, Buttons.A},
                    {Keys.P, Buttons.Y}
                };


        public static void Setup()
        {
            InputState[0] = new InputState(PlayerIndex.One);
        }


        public static void Update()
        {
            for (var i = 0; i < InputState.Length; i++)
            {
                if (InputState[i] != null)
                    InputState[i].Update();
            }
        }
        public static Buttons KeyToPad(Keys key)
        {
            if (InputMap.ContainsKey(key))
                return InputMap[key];

            return Buttons.A;
        }
    }

    public class InputState
    {
        public KeyboardState KeyboardState { get; private set; }
        public GamePadState GamePadState { get; private set; }

        public KeyboardState PreviousKeyboardState;
        public GamePadState PreviousGamePadState;

        public PlayerIndex PlayerIndex
        {
            get;
            private set;
        }
        public InputState(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            Update();
        }

        public void Update()
        {
            PreviousGamePadState = GamePadState;
            PreviousKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
            GamePadState = GamePad.GetState(PlayerIndex);
        }

        public Boolean IsPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key) || GamePadState.IsButtonDown(InputManager.KeyToPad(key));
        }
        public Boolean EitherIsPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key) || GamePadState.IsButtonDown(InputManager.KeyToPad(key));
        }
        public Boolean WasPressed(Keys key, Buttons button)
        {
            return WasPressed(key) || WasPressed(button);
        }
        public Boolean EitherWasPressed(Keys key)
        {
            return WasPressed(key, InputManager.KeyToPad(key));
        }
        public Boolean EitherUpWasDown(Keys key)
        {
            return UpWasDown(key, InputManager.KeyToPad(key));
        }
        public Boolean EitherDownWasUp(Keys key)
        {
            return DownWasUp(key, InputManager.KeyToPad(key));
        }
        public Boolean UpWasDown(Keys key, Buttons button)
        {
            return UpWasDown(key) || UpWasDown(button);
        }
        public Boolean DownWasUp(Keys key, Buttons button)
        {
            return DownWasUp(key) || DownWasUp(button);
        }
        public Boolean WasPressed(Keys key)
        {
            return PreviousKeyboardState != default(KeyboardState) && PreviousKeyboardState.IsKeyDown(key);
        }
        public Boolean WasPressed(Buttons button)
        {
            return PreviousGamePadState.IsButtonDown(button);
        }

        public Boolean UpWasDown(Keys key)
        {
            return KeyboardState.IsKeyUp(key) && WasPressed(key);
        }
        public Boolean UpWasDown(Buttons button)
        {
            return GamePadState.IsButtonUp(button) && WasPressed(button);
        }
        public Boolean DownWasUp(Keys key)
        {
            return KeyboardState.IsKeyDown(key) && !WasPressed(key);
        }
        public Boolean DownWasUp(Buttons button)
        {
            return GamePadState.IsButtonDown(button) && !WasPressed(button);
        }
    }
}
