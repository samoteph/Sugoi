using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace StarterGame
{
    public static class AssetStore
    {
        /// <summary>
        /// Creation de la fonte du jeu
        /// </summary>
        /// <param name="fontSheet"></param>
        /// <returns></returns>

        private static Font CreateFont(SurfaceFontSheet fontSheet)
        {
            var font = new Font();

            font.AddCharacters(CharactersGroups.AlphaUpperAndLower);
            font.AddCharacters(CharactersGroups.Numeric);
            font.AddCharacters(".,\"'?!@*#$%: ");
            font.CharacterIndex += 5;
            font.AddCharacters("()+-/=©<>~§"); //~ correspond à la disquette, § au coeur
            font.UnknownTileNumber = font.GetTileNumber('$');

            font.FontSheet = fontSheet;

            return font;
        }

        public static Task StartAsync(VideoMemory videoMemory, Audio audio)
        {
            var fontSheet = videoMemory.CreateFontSheet("font");
            Font = CreateFont(fontSheet);

            videoMemory.Machine.Cartridge.ClearAssets();

            return Task.CompletedTask;
        }

        public static Font Font
        {
            get;
            private set;
        }
    }
}
