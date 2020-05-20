using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        private static Map[] CreateMotherOpenAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("MotherOpen1", 4, 3, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(42, 43, 44, 45, 52, 53, 54, 55, 46, 47, 48, 49);
            maps[0] = map;

            map = new Map();
            map.Create("MotherOpen2", 4, 4, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(42, 43, 44, 45, 52, 53, 54, 55, 56, 57, 58, 59, 66, 67, 68, 69);
            maps[1] = map;

            return maps;
        }

        private static Map CreateMotherTiredMap(SurfaceTileSheet tileSheet)
        {
            var map = new Map();
            map.Create("MotherTired1", 6, 3, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(80, 81, 82, 83, 84, 80, 90, 91, 92, 93, 94, 95, 100,101,102,103,104, 105);
            
            return map;
        }

        private static Map[] CreateMotherFlyAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[3];

            var map = new Map();
            map.Create("MotherFly1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(60, 61, 70, 71);
            maps[0] = map;

            map = new Map();
            map.Create("MotherFly2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(62, 63, 72, 73);
            maps[1] = map;

            map = new Map();
            map.Create("MotherFly3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(64, 65, 74, 75);
            maps[2] = map;

            return maps;
        }

        private static Map[] CreateOpaWalkAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("OpaWalk1", 2, 1, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(200, 201);
            maps[0] = map;

            map = new Map();
            map.Create("OpaWalk2", 2, 1, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(202, 203);
            maps[1] = map;

            return maps;
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

        private static Map[] CreateKaboomAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("Kaboom1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(76, 77, 86, 87);
            maps[0] = map;

            map = new Map();
            map.Create("Kaboom2", 3, 3, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(97, 98, 99, 107, 108, 109, 117, 118, 119);
            maps[1] = map;

            return maps;
        }

        private static Map[] CreateBabyAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[3];

            var map = new Map();
            map.Create("Baby1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(22, 23, 32, 33);
            maps[0] = map;

            map = new Map();
            map.Create("Baby2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(24, 25, 34, 35);
            maps[1] = map;

            map = new Map();
            map.Create("Baby3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(26, 27, 36, 37);
            maps[2] = map;

            return maps;
        }

        private static Map[] CreateDuckAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[3];

            var map = new Map();
            map.Create("Duck1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(6, 7, 16, 17);
            maps[0] = map;

            map = new Map();
            map.Create("Duck2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(8, 9, 18, 19);
            maps[1] = map;

            map = new Map();
            map.Create("Duck3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(20, 21, 30, 31);
            maps[2] = map;

            return maps;
        }

        private static Map[] CreateFlyAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("Fly1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(28, 29, 38, 39);
            maps[0] = map;

            map = new Map();
            map.Create("Fly2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(40, 41, 50, 51);
            maps[1] = map;

            return maps;
        }

        private static Map[] CreateDeathStartAnimation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[3];

            var map = new Map();
            map.Create("Star1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(78, 79, 88, 89);
            maps[0] = map;

            map = new Map();
            map.Create("Star2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(198, 199, 208, 209);
            maps[1] = map;

            map = new Map();
            map.Create("Star3", 1, 1, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(207);
            maps[2] = map;

            return maps;
        }

        private static Map[] CreateCoin1Animation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("Coin1-1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(110, 111, 120, 121);
            maps[0] = map;

            map = new Map();
            map.Create("Coin1-2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(112, 113, 122, 123);
            maps[1] = map;

            return maps;
        }

        private static Map[] CreateCoin5Animation(SurfaceTileSheet tileSheet)
        {
            Map[] maps = new Map[2];

            var map = new Map();
            map.Create("Coin5-1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(114, 115, 124, 125);
            maps[0] = map;

            map = new Map();
            map.Create("Coin5-2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(130, 131, 140, 141);
            maps[1] = map;

            return maps;
        }

        /// <summary>
        /// Lancement de l'AssetStore 
        /// </summary>
        /// <param name="videoMemory"></param>
        /// <param name="audio"></param>
        /// <returns></returns>

        public static async Task StartAsync(VideoMemory videoMemory, Audio audio)
        {
            Title = videoMemory.CreateSprite("title");
            Tiles = videoMemory.CreateTileSheet("tiles");
            ParallaxMaps = videoMemory.CreateMapTmx("map");

            OpaCursorMaps = CreateOpaCursorAnimation(Tiles);
            OpaFlightMaps = CreateOpaFlightAnimation(Tiles);
            OpaWalkMaps = CreateOpaWalkAnimation(Tiles);

            MotherFlyMaps = CreateMotherFlyAnimation(Tiles);
            MotherOpenMaps = CreateMotherOpenAnimation(Tiles);

            MotherTired = CreateMotherTiredMap(Tiles);

            KaboomMaps = CreateKaboomAnimation(Tiles);

            BabyMaps = CreateBabyAnimation(Tiles);

            DeathStarMaps = CreateDeathStartAnimation(Tiles);

            DuckMaps = CreateDuckAnimation(Tiles);

            FlyMaps = CreateFlyAnimation(Tiles);

            Coin1Maps = CreateCoin1Animation(Tiles);
            Coin5Maps = CreateCoin5Animation(Tiles);

            var fontSheet = videoMemory.CreateFontSheet("font");
            Font = CreateFont(fontSheet);

            // Son
            await audio.PreloadAsync("homeSound", 1);
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

        public static Map[] OpaWalkMaps
        {
            get;
            private set;
        }

        public static Map[] MotherOpenMaps
        {
            get;
            private set;
        }

        public static Map[] MotherFlyMaps
        {
            get;
            private set;
        }

        public static Map MotherTired
        {
            get;
            private set;
        }

        public static Map[] KaboomMaps
        {
            get;
            private set;
        }

        public static Map[] BabyMaps
        {
            get;
            private set;
        }

        public static Map[] DeathStarMaps
        {
            get;
            private set;
        }

        public static Map[] DuckMaps
        {
            get;
            private set;
        }

        public static Map[] FlyMaps
        {
            get;
            private set;
        }

        public static Map[] Coin1Maps
        {
            get;
            private set;
        }

        public static Map[] Coin5Maps
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
