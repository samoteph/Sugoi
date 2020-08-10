using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Sugoi.Core
{
    public class GamepadPool
    {
        private List<Gamepad> gamepads;
        private Machine machine;

        public GamepadPool(int size)
        {
            this.gamepads = new List<Gamepad>(size);
        }

        /// <summary>
        /// Démarre le pool
        /// </summary>
        /// <param name="machine"></param>

        internal void Start(Machine machine)
        {
            this.machine = machine;
        }

        /// <summary>
        /// Obtenir un sprite disponible (mort). Le sprite est initialisé automatiquement
        /// </summary>

        public void AddGamepad(Gamepad gamepad)
        {
            gamepads.Add(gamepad);
            gamepad.Start(machine);
        }

        public void RemoveGamepad(Gamepad gamepad)
        {
            gamepads.Remove(gamepad);
            gamepad.Stop();
        }

        /// <summary>
        /// Renvoie un mix de tous les gamepads. permet de gerer un gamepadGlobal qui prend tous les périphériques en compte comme un seul. 
        /// </summary>
        /// <param name="gamepad"></param>

        public int GetGamepadGlobalValue()
        {
            int value = 0;

            for (int i = 0; i < this.gamepads.Count; i++)
            {
                var gamepad = this.gamepads[i];

                var gamepadValue = gamepad.GetValue();
                value = value | gamepadValue;
            }

            return value;
        }

        /// <summary>
        /// Permet d'obtenir le gamepad dont la clé est pressé. le func peut renvoyé true pour arreter la recherche
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public void FindGamepadByKeyPressed(GamepadKeys key, Func<Gamepad, bool> gamepadPressed)
        {
            if(gamepadPressed == null)
            {
                return;
            }

            for (int i = 0; i < this.gamepads.Count; i++)
            {
                var gamepad = this.gamepads[i];

                if (gamepad.IsPressed(key))
                {
                    bool mustContinue = gamepadPressed(gamepad);
                    
                    if(mustContinue == false)
                    {
                        return;
                    }
                }
            }
        }

        internal void Stop()
        {
        }
    }
}
