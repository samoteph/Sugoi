using CrazyZone.Pages;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.IO;
using System.Reflection;

namespace CrazyZone
{
    /// <summary>
    /// Video game for Sugoi Virtual Console
    /// </summary>

    public class CrazyZoneCartridge : ExecutableCartridge
    {
        private Machine machine;
        private Game game;

        public CrazyZoneCartridge()
        {
        }

        public override void Start(Machine machine)
        {
            // Démarrage du jeu
            this.machine = machine;

            var videoMemory = this.machine.VideoMemory;
            var screen = this.machine.Screen;

            AssetStore.Start(videoMemory);

            screen.Font = AssetStore.Font;

            game = new Game();

            game.Start(this.machine);
            game.Navigate(typeof(HomePage));
        }

        public override void Load()
        {
            this.LoadFromResource("CrazyZone.Cartridge.Cartridge.sugoi");
        }
    }
}
