using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
                    return GamepadKeys.Up;
                }
                else if (gamePadKeyValues[(int)GamepadKeys.Down])
                {
                    return GamepadKeys.Down;
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

        public bool IsButtonsPressed
        {
            get
            {
                if (IsPressed(GamepadKeys.ButtonA))
                {
                    return true;
                }
                else if (IsPressed(GamepadKeys.ButtonB))
                {
                    return true;
                }
                else if (IsPressed(GamepadKeys.ButtonC))
                {
                    return true;
                }
                else if (IsPressed(GamepadKeys.ButtonD))
                {
                    return true;
                }
                else if (IsPressed(GamepadKeys.ButtonStart))
                {
                    return true;
                }

                return false;
            }
        }

        public void Release()
        {
            for (int i = 0; i < gamePadKeyValues.Length; i++)
            {
                gamePadKeyValues[i] = false;
            }
        }

        public void Release(GamepadKeys key)
        {
            gamePadKeyValues[(int)key] = false;
        }

        /// <summary>
        /// One key is released
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public bool IsRelease(GamepadKeys key)
        {
            return gamePadKeyValues[(int)key] == false;
        }

        /// <summary>
        /// All keys are released
        /// </summary>
        /// <returns></returns>

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

        public void WaitForRelease(GamepadKeys gamepadKey, Action completed = null)
        {
            this.machine.PrepareWaiting(() =>
            {
                return this.IsRelease(gamepadKey) == false;
            },
            completed
            );
        }

        public void WaitForRelease(GamepadKeys gamepadKey, int maximumFrame, Action completed = null)
        {
            frameWaitCount = 0;
            this.machine.PrepareWaiting(() =>
            {
                if (frameWaitCount < maximumFrame)
                {
                    frameWaitCount++;
                    return this.IsRelease(gamepadKey) == false;
                }

                // on attend plus
                return false;
            },
            completed
            );
        }

        public void WaitForRelease(Action completed = null)   
        {
            this.machine.PrepareWaiting( () =>
            {
                return this.IsRelease() == false;
            },
            completed
            );
        }

        private int frameWaitCount = 0;

        /// <summary>
        /// Attent que les touche se relache avec un temps maximum
        /// </summary>
        /// <param name="maximumFrame"></param>
        /// <param name="completed"></param>

        public void WaitForRelease(int maximumFrame, Action completed = null)
        {
            frameWaitCount = 0;
            this.machine.PrepareWaiting(() =>
            {
                if (frameWaitCount < maximumFrame)
                {
                    frameWaitCount++;
                    return this.IsRelease() == false;
                }

                // on attend plus
                return false;
            },
            completed
            );
        }

        /// <summary>
        /// Obtenir la valeur en bit
        /// </summary>
        /// <returns></returns>

        public int GetValue()
        {
            if(gamePadKeyValues == null)
            {
                return 0;
            }

            int value = 0;

            for (int i = 0; i < gamePadKeyValues.Length; i++)
            {
                if (gamePadKeyValues[i] == true)
                {
                    value += (1 << i);
                }
            }

            return value;
        }

        /// <summary>
        /// Set a value
        /// </summary>
        /// <param name="value1"></param>

        public void SetValue(int value1)
        {
            if (gamePadKeyValues == null)
            {
                return;
            }

            for (int i = 0; i < gamePadKeyValues.Length; i++)
            {
                bool isPressed = (value1 & 0x1) == 0x1;
                gamePadKeyValues[i] = isPressed;

                value1 = value1 >> 1;
            }
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
