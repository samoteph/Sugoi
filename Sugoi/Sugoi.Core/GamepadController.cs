using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class GamepadController
    {
        public Machine Machine
        {
            get;
            private set;
        }

        public Gamepad GamepadGlobal
        {
            get;
            private set;
        }

        private List<Gamepad> Gamepads
        {
            get;
            set;
        }

        public void Start(Machine machine)
        {
            this.Gamepads = new List<Gamepad>();
            this.Machine = machine;

            this.GamepadGlobal = new Gamepad();
            this.GamepadGlobal.Start(machine);
        }

        /// <summary>
        /// Ajouter un gamepad
        /// </summary>
        /// <param name="gamepad"></param>

        public void AddGamepad(Gamepad gamepad)
        {
            this.Gamepads.Add(gamepad);
            gamepad.Start(Machine);
        }

        /// <summary>
        /// retirer un gamepad
        /// </summary>
        /// <param name="gamepad"></param>

        public void RemoveGamepad(Gamepad gamepad)
        {
            gamepad.Stop();

            this.Gamepads.Remove(gamepad);
        }
    }
}
