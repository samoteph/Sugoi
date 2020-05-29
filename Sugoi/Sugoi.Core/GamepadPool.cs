using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Sugoi.Core
{
    public class GamepadPool
    {
        private Gamepad[] gamepads;
        private Machine machine;

        public GamepadPool(int size)
        {
            this.gamepads = new Gamepad[size];

            for (int i = 0; i < size; i++)
            {
                this.gamepads[i] = (Gamepad)Activator.CreateInstance(typeof(Gamepad));
            }

            this.CurrentIndex = 0;
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
        /// Index du gamepad
        /// </summary>

        public int CurrentIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Obtenir un sprite disponible (mort). Le sprite est initialisé automatiquement
        /// </summary>

        public Gamepad GetFreeGamepad()
        {
            var index = this.SearchFreeGamepadIndex(this.CurrentIndex);

            if (index == -1)
            {
                throw new Exception("The pool Gamepad is unable to reserve a new Gamepad! Please set a bigger size when initializing (>" + this.gamepads.Length + ") !");
            }
            else
            {
                this.CurrentIndex = index;
                var gamepad = this.gamepads[index];

                gamepad.Start(machine);

                return gamepad;
            }
        }

        private int SearchFreeGamepadIndex(int startIndex)
        {
            var length = this.gamepads.Length;
            var endIndex = length + startIndex;

            for (int i = startIndex; i < endIndex; i++)
            {
                var index = i % length;

                var gamepad = this.gamepads[index];

                if (gamepad.IsFree == true)
                {
                    gamepad.IsFree = false;
                    return index;
                }
            }

            // tout est pris !
            return -1;
        }

        /// <summary>
        /// compte le nombre de gamepad free
        /// </summary>
        /// <returns></returns>

        public int GetFreeCount()
        {
            int count = 0;

            for (int i = 0; i < this.gamepads.Length; i++)
            {
                var gamepad = this.gamepads[i];

                if (gamepad.IsFree == true)
                {
                    count++;
                }
            }

            return count;
        }

        public void ForeachReserved(Action<Gamepad> action)
        {
            if (action == null)
            {
                return;
            }

            for (int i = 0; i < this.gamepads.Length; i++)
            {
                var gamepad = this.gamepads[i];

                if (gamepad.IsFree == false)
                {
                    action.Invoke(gamepad);
                }
            }
        }

        /// <summary>
        /// Renvoie un mix de tous les gamepads. permet de gerer un gamepadGlobal qui prend tous les périphériques en compte comme un seul. 
        /// </summary>
        /// <param name="gamepad"></param>

        public int GetGamepadGlobalValue()
        {
            int value = 0;

            for (int i = 0; i < this.gamepads.Length; i++)
            {
                var gamepad = this.gamepads[i];

                if (gamepad.IsFree == false)
                {
                    var gamepadValue = gamepad.GetValue();

                    value = value | gamepadValue;
                }
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

            for (int i = 0; i < this.gamepads.Length; i++)
            {
                var gamepad = this.gamepads[i];

                if (gamepad.IsFree == false)
                {
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
        }

        internal void Stop()
        {
        }
    }
}
