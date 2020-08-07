using Sugoi.Core;
using Sugoi.Core.IO;
using System.Threading.Tasks;

namespace CrazyZone
{
    /// <summary>
    /// Video game for Sugoi Virtual Console
    /// </summary>

    public class CrazyZoneCartridge : ExecutableCartridge
    {
        private Machine machine;
        private CrazyZoneGame game;

        public CrazyZoneCartridge()
        {
            GameService.Instance.AddGameSingleton<CrazyZoneGame>();
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

            game = GameService.Instance.GetGameSingleton<CrazyZoneGame>();

            game.Start(this.machine);
        }

        public override Task LoadAsync()
        {
            return this.LoadFromResourceAsync("CrazyZone.Cartridge.Cartridge.sugoi");
        }

        //public override Task LoadHeaderAsync()
        //{
        //    return Task.Run(() =>
        //    {
        //        using (var stream = GetStreamResource("CrazyZone.Cartridge.Cartridge.sugoi"))
        //        {
        //            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
        //            {
        //                this.LoadHeader(reader);
        //            }
        //        }
        //    });
        //}
    }
}
