using Sugoi.Core;
using System;
using GamepadWindows = Windows.Gaming.Input;

namespace Sugoi.Console.Controls
{
    public class XBoxGamepadManager
    {
        // Les manettes XBOX
        GamepadWindows.Gamepad[] xboxGamepads = new GamepadWindows.Gamepad[2];

        // Les manettes XBOX virtuelle
        Gamepad[] sugoiGamepads = new Gamepad[2];

        public void Start(Machine machine)
        {
            for (int i = 0; i < this.sugoiGamepads.Length; i++)
            {
                this.sugoiGamepads[i] = machine.Gamepads.GetFreeGamepad();
            }
        }

        /// <summary>
        /// Mise à jour des valeurs des manettes
        /// </summary>

        public Gamepad[] GetGamepads()
        {
            // recupération des gamepads XBOX (2 max)
            var gamepads = GamepadWindows.Gamepad.Gamepads;

            try
            {
                if (gamepads.Count > 0)
                {
                    this.xboxGamepads[0] = gamepads[0];

                    if (gamepads.Count > 1)
                    {
                        this.xboxGamepads[1] = gamepads[1];
                    }
                    else
                    {
                        this.xboxGamepads[1] = null;
                    }
                }
                else
                {
                    this.xboxGamepads[0] = null;
                }
            }
            catch (Exception ex)
            {
            }

            // transformation des valeurs XBOX en gamepad Sugoi
            SetSugoiGamepadValue(xboxGamepads[0], sugoiGamepads[0]);
            SetSugoiGamepadValue(xboxGamepads[1], sugoiGamepads[1]);

            return sugoiGamepads;
        }

        /// <summary>
        /// Fixer les valeurs du GamePad Sugoi a partir du Gamepad Windows
        /// </summary>
        /// <param name="gamepadWindows"></param>
        /// <param name="gamepadSugoi"></param>

        private void SetSugoiGamepadValue(GamepadWindows.Gamepad gamepadWindows, Gamepad gamepadSugoi)
        {
            if (gamepadWindows == null)
            {
                return;
            }

            if (gamepadSugoi == null)
            {
                return;
            }

            var gamepadValues = gamepadWindows.GetCurrentReading();

            this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.A, GamepadKeys.ButtonA);
            this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.B, GamepadKeys.ButtonB);
            this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.X, GamepadKeys.ButtonC);
            this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.Y, GamepadKeys.ButtonD);

            this.SetSugoiGamepadThumb(gamepadSugoi, gamepadValues.LeftThumbstickX, GamepadKeys.Left, GamepadKeys.Right);
            this.SetSugoiGamepadThumb(gamepadSugoi, gamepadValues.LeftThumbstickY, GamepadKeys.Down, GamepadKeys.Up);

            if (gamepadSugoi.HorizontalController == GamepadKeys.None && gamepadSugoi.VerticalController == GamepadKeys.None)
            {
                this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.DPadRight, GamepadKeys.Right);
                this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.DPadUp, GamepadKeys.Up);
                this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.DPadLeft, GamepadKeys.Left);
                this.SetSugoiGamepadButton(gamepadSugoi, gamepadValues, GamepadWindows.GamepadButtons.DPadDown, GamepadKeys.Down);
            }
        }
        /// <summary>
        /// Fixe le controller droite/gauche / haut bas
        /// </summary>
        /// <param name="gamepadSugoi"></param>
        /// <param name="thumbStick"></param>
        /// <param name="buttonSugoiMin"></param>
        /// <param name="buttonSugoiMax"></param>

        private void SetSugoiGamepadThumb(Gamepad gamepadSugoi, double thumbStick, GamepadKeys buttonSugoiMin, GamepadKeys buttonSugoiMax)
        {
            if (gamepadSugoi != null)
            {
                if (thumbStick > 0.3)
                {
                    gamepadSugoi.Press(buttonSugoiMax);
                }
                else if (thumbStick < -0.3)
                {
                    gamepadSugoi.Press(buttonSugoiMin);
                }
                else
                {
                    if (gamepadSugoi.IsPressed(buttonSugoiMin))
                    {
                        gamepadSugoi.Release(buttonSugoiMin);
                    }
                    else if (gamepadSugoi.IsPressed(buttonSugoiMax))
                    {
                        gamepadSugoi.Release(buttonSugoiMax);
                    }
                }
            }
        }

        /// <summary>
        /// Fixe les buttons
        /// </summary>
        /// <param name="gamepadValues"></param>
        /// <param name="gamepadSugoi"></param>
        /// <param name="buttonWindows"></param>
        /// <param name="buttonSugoi"></param>

        private void SetSugoiGamepadButton(Gamepad gamepadSugoi, GamepadWindows.GamepadReading gamepadValues, GamepadWindows.GamepadButtons buttonWindows, GamepadKeys buttonSugoi)
        {
            if (gamepadSugoi != null)
            {
                if ((gamepadValues.Buttons & buttonWindows) == buttonWindows)
                {
                    gamepadSugoi.Press(buttonSugoi);
                }
                else if (gamepadSugoi.IsPressed(buttonSugoi))
                {
                    gamepadSugoi.Release(buttonSugoi);
                }
            }
        }
    }
}
