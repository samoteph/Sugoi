using StarterGame.Pages;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StarterGame
{
    /// <summary>
    /// Video game for Sugoi Virtual Console
    /// </summary>

    public class MyCartridge : ExecutableCartridge
    {
        private Machine machine;
        private Game game;

        public MyCartridge()
        {
            // Creation du Singleton du jeu
            GameService.Instance.AddGameSingleton<MyGame>();
        }

        public override async Task StartAsync(Machine machine)
        {
            // Démarrage du jeu
            this.machine = machine;

            var videoMemory = this.machine.VideoMemory;
            var screen = this.machine.Screen;
            var audio = this.machine.Audio;

            await AssetStore.StartAsync(videoMemory, audio);

            screen.Font = AssetStore.Font;

            game = GameService.Instance.GetGameSingleton<MyGame>();
            game.Start(this.machine);
        }

        public override Task LoadAsync()
        {
            return this.LoadFromResourceAsync("StarterGame.Cartridge.Cartridge.sugoi");
            //return this.LoadEmptyCartridgeAsync();
        }
    }
}
