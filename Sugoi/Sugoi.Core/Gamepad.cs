using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    public class Gamepad
    {
        bool[] gamePadKeyValues;
        Machine machine;

        public Gamepad()
        {
        }

        internal void Start(Machine machine)
        {
            this.machine = machine;
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

        public bool IsPressed(GamepadKeys key)
        {
            return gamePadKeyValues[(int)key];
        }

        public bool IsButtonsPressed()
        {
            if(IsPressed(GamepadKeys.ButtonA))
            {
                return true;
            }
            else if(IsPressed(GamepadKeys.ButtonB))
            {
                return true;
            }
            else if(IsPressed(GamepadKeys.ButtonC))
            {
                return true;
            }
            else if(IsPressed(GamepadKeys.ButtonD))
            {
                return true;
            }
            else if(IsPressed(GamepadKeys.ButtonStart))
            {
                return true;
            }

            return false;
        }


        public void Release(GamepadKeys key)
        {
            gamePadKeyValues[(int)key] = false;
        }

        public bool IsRelease(GamepadKeys key)
        {
            return gamePadKeyValues[(int)key];
        }

        public bool IsRelease()
        {
            for (int i = 0; i < gamePadKeyValues.Length; i++)
            {
                if (gamePadKeyValues[i] == true)
                {
                    return false;
                }
            }

            return true;
        }

        public void WaitForRelease(Action completed = null)
        {
            Debug.WriteLine("WaitForRelease");

            this.machine.PrepareWaiting( () =>
            {
                return this.IsRelease() == false;
            },
            completed
            );
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
