using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core
{
    /// <summary>
    /// Guidance pour integrer une nouvelle consoles
    /// </summary>

    public interface ISugoiConsole
    {
        /// <summary>
        /// Démarrage de la console
        /// </summary>
        /// <param name="cartridge"></param>

        Task StartAsync(Cartridge cartridge);
        
        /// <summary>
        /// Arret de la console
        /// </summary>
        
        void Stop();
        
        /// <summary>
        /// La console est démarrée
        /// </summary>
        
        bool IsStarted
        {
            get;
        }

        /// <summary>
        /// Memoire vidéo
        /// </summary>

        VideoMemory VideoMemory
        {
            get;
        }

        /// <summary>
        /// Ecran
        /// </summary>

        SurfaceSprite Screen
        {
            get;
        }

        Gamepad Gamepad1
        {
            get;
        }

        Gamepad Gamepad2
        {
            get;
        }

        event SugoiInitializedHandler Initialized;

        /// <summary>
        /// Mise à jour d'une frame
        /// </summary>
        event SugoiFrameUpdatedHandler FrameUpdated;

        /// <summary>
        /// Affichage de la frame
        /// </summary>

        event SugoiFrameDrawnHandler FrameDrawn;

        /// <summary>
        /// Permet d'executer un script (en plus de celui chargé dans la cartouche) 
        /// </summary>
        /// <param name="script"></param>

        void ExecuteScript(string script);
    }

    public delegate void SugoiInitializedHandler();
    public delegate void SugoiFrameUpdatedHandler();
    public delegate void SugoiFrameDrawnHandler(int frameExecuted);
}
