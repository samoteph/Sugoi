using Sugoi.Core;
using Sugoi.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sugoi.Core
{
    public class Game
    {
        /// <summary>
        /// Constructeur
        /// </summary>

        public Game()
        {
        }

        /// <summary>
        /// Machine
        /// </summary>

        public Machine Machine
        {
            get;
            private set;
        }

        public GameNavigation Navigation
        {
            get;
            private set;
        }

        public void Start(Machine machine)
        {
            this.Machine = machine;
            this.Navigation = new GameNavigation(machine);

            this.StartOverride();
        }

        /// <summary>
        /// Here we can override the Start
        /// </summary>

        protected virtual void StartOverride()
        {
        }

        private string currentMusicKey;

        /// <summary>
        /// Jouer la music en loop et laisser la music en cours s'il c'est deja la même qui est jouée 
        /// </summary>
        /// <param name="musicLoop"></param>

        public void PlayLoop(string musicKey)
        {
            if (musicKey != currentMusicKey)
            {
                this.Machine.Audio.Stop(currentMusicKey);

                currentMusicKey = musicKey;

                this.Machine.Audio.PlayLoop(musicKey);
            }
        }
    }
}
