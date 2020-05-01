using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class Gamepad
    {
        bool[] gamePadKeyValues;

        public Gamepad()
        {
        }

        internal void Start()
        {
            gamePadKeyValues = new bool[Enum.GetValues(typeof(GamepadKeys)).Length];
        }

        internal void Stop()
        {

        }

        public GamepadKeys HorizontalController
        {
            get
            {
                if(gamePadKeyValues[(int)GamepadKeys.Right])
                {
                    return GamepadKeys.Right;
                }
                else if(gamePadKeyValues[(int)GamepadKeys.Left])
                {
                    return GamepadKeys.Left;
                }

                return GamepadKeys.None;
            }
        }

        public GamepadKeys VerticalController
        {
            get
            {
                if (gamePadKeyValues[(int)GamepadKeys.Up])
                {
                    return GamepadKeys.Down;
                }
                else if (gamePadKeyValues[(int)GamepadKeys.Down])
                {
                    return GamepadKeys.Up;
                }

                return GamepadKeys.None;
            }
        }

        public bool IsControllerPressed
        {
            get
            {
                return HorizontalController != GamepadKeys.None || VerticalController != GamepadKeys.None;
            }
        }

        public void Press(GamepadKeys key)
        {
            gamePadKeyValues[(int)key] = true;

            switch(key)
            {
                case GamepadKeys.Up:
                    Release(GamepadKeys.Down);
                    break;
                case GamepadKeys.Down:
                    Release(GamepadKeys.Up);
                    break;
                case GamepadKeys.Right:
                    Release(GamepadKeys.Left);
                    break;
                case GamepadKeys.Left:
                    Release(GamepadKeys.Right);
                    break;
            }
        }

        public void Release(GamepadKeys key)
        {
            gamePadKeyValues[(int)key] = false;
        }

        public bool IsPressed(GamepadKeys key)
        {
            return gamePadKeyValues[(int)key];
        }
    }

    public enum GamepadKeys
    {
        None = -1,
        Up,
        Down,
        Right,
        Left,
        ButtonA,
        ButtonB,
        ButtonC,
        ButtonD,
        ButtonStart,
    }
}
