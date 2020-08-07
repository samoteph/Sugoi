using SixLabors.ImageSharp.Formats.Tga;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class GameService
    {
        /// <summary>
        /// Constructeur
        /// </summary>

        private GameService()
        {

        }

        /// <summary>
        /// Instance du GameService
        /// </summary>

        public static GameService Instance
        {
            get;
        } = new GameService();

        /// <summary>
        /// Singleton de Game
        /// </summary>

        public Game Game
        {
            get;
            private set;
        }

        /// <summary>
        /// Ajout du singleton Game
        /// </summary>

        public void AddGameSingleton()
        {
            this.Game = (Game)Activator.CreateInstance(typeof(Game));
        }

        /// <summary>
        /// Ajouter le singleton Game
        /// </summary>
        /// <typeparam name="TGame"></typeparam>

        public void AddGameSingleton<TGame>() where TGame : Game
        {
            this.Game = Activator.CreateInstance<TGame>();
        }

        public TGame GetGameSingleton<TGame>() where TGame : Game
        {
            return (TGame)this.Game;
        }
    }
}
