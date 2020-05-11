using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone
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
            font.AddCharacters(".,\"'?!@*#$%&: ");
            font.CharacterIndex += 4;
            font.AddCharacters("()+-/=©<>");
            font.UnknownTileNumber = font.GetTileNumber('$');

            font.FontSheet = fontSheet;

            return font;
        }

        private static Map[] CreateOpaCursorAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("OpaCursor1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(182, 183, 192, 193);
            maps[0] = map;

            map = new Map();
            map.Create("OpaCursor2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(184, 185, 194, 195);
            maps[1] = map;

            return maps;
        }

        private static Map[] CreateOpaFlightAnimation(SurfaceTileSheet tileSheet)
        {
            var maps = new Map[3];

            var map = new Map();
            map.Create("OpaFlight1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(0);
            map[1, 0] = new MapTileDescriptor(1);
            map[0, 1] = new MapTileDescriptor(10);
            map[1, 1] = new MapTileDescriptor(11);
            maps[0] = map;

            map = new Map();
            map.Create("OpaFlight2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(2);
            map[1, 0] = new MapTileDescriptor(3);
            map[0, 1] = new MapTileDescriptor(12);
            map[1, 1] = new MapTileDescriptor(13);
            maps[1] = map;

            map = new Map();
            map.Create("OpaFlight3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(4);
            map[1, 0] = new MapTileDescriptor(5);
            map[0, 1] = new MapTileDescriptor(14);
            map[1, 1] = new MapTileDescriptor(15);
            maps[2] = map;

            return maps;
        }

        public static void Start(VideoMemory videoMemory)
        {
            Title = videoMemory.CreateSprite("title");
            Tiles = videoMemory.CreateTileSheet("tiles");
            ParallaxMaps = videoMemory.CreateMapTmx("map");

            OpaCursorMaps = CreateOpaCursorAnimation(Tiles);
            OpaFlightMaps = CreateOpaFlightAnimation(Tiles);

            var fontSheet = videoMemory.CreateFontSheet("font");
            Font = CreateFont(fontSheet);
        }

        public static SurfaceSprite Title
        {
            get;
            private set;
        }

        public static SurfaceTileSheet Tiles
        {
            get;
            private set;
        }

        public static Map[] ParallaxMaps
        {
            get;
            private set;
        }

        public static Map[] OpaCursorMaps
        {
            get;
            private set;
        }

        public static Map[] OpaFlightMaps
        {
            get;
            private set;
        }

        public static Font Font
        {
            get;
            private set;
        }
    }
}
