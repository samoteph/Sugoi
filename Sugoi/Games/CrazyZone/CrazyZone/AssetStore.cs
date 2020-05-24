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

        public static Animator CreateMotherOpenAnimation()
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[2];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("MotherOpen1", 4, 3, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(42, 43, 44, 45, 52, 53, 54, 55, 46, 47, 48, 49);
            frames[0] = new MapAnimationFrame(map, 100);

            map = new Map();
            map.Create("MotherOpen2", 4, 4, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(42, 43, 44, 45, 52, 53, 54, 55, 56, 57, 58, 59, 66, 67, 68, 69);
            frames[1] = new MapAnimationFrame(map, 100);

            animator.Initialize(frames);

            return animator;
        }

        private static Map CreateMotherTiredMap(SurfaceTileSheet tileSheet)
        {
            var map = new Map();
            map.Create("MotherTired1", 6, 3, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(80, 81, 82, 83, 84, 80, 90, 91, 92, 93, 94, 95, 100,101,102,103,104, 105);
            
            return map;
        }

        public static Animator CreateMotherFlyAnimation()
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[3];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("MotherFly1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(60, 61, 70, 71);
            frames[0] = new MapAnimationFrame(map, 20, false, false, 8, -9);

            map = new Map();
            map.Create("MotherFly2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(62, 63, 72, 73);
            frames[1] = new MapAnimationFrame(map, 20, false, false, 8, -9);

            map = new Map();
            map.Create("MotherFly3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(64, 65, 74, 75);
            frames[2] = new MapAnimationFrame(map, 20, false, false, 8, -1);

            animator.Initialize(frames);

            return animator;
        }

        private static Animator CreateOpaWalkAnimation(SurfaceTileSheet tileSheet)
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[2];

            var map = new Map();
            map.Create("OpaWalk1", 2, 1, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(200, 201);
            frames[0] = new MapAnimationFrame(map, 10);

            map = new Map();
            map.Create("OpaWalk2", 2, 1, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(202, 203);
            frames[1] = new MapAnimationFrame(map, 20);

            animator.Initialize(frames);

            return animator;
        }

        private static Animator CreateOpaCursorAnimation(SurfaceTileSheet tileSheet)
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[2]; 

            var map = new Map();
            map.Create("OpaCursor1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(182, 183, 192, 193);

            frames[0] = new MapAnimationFrame(map, 15);

            map = new Map();
            map.Create("OpaCursor2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(184, 185, 194, 195);

            frames[1] = new MapAnimationFrame(map, 15);

            animator.Initialize(frames);

            return animator;
        }

        private static Animator CreateOpaFlightAnimation(SurfaceTileSheet tileSheet)
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[3];

            var map = new Map();
            map.Create("OpaFlight1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(0);
            map[1, 0] = new MapTileDescriptor(1);
            map[0, 1] = new MapTileDescriptor(10);
            map[1, 1] = new MapTileDescriptor(11);
            frames[0] = new MapAnimationFrame(map, 30);

            map = new Map();
            map.Create("OpaFlight2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(2);
            map[1, 0] = new MapTileDescriptor(3);
            map[0, 1] = new MapTileDescriptor(12);
            map[1, 1] = new MapTileDescriptor(13);
            frames[1] = new MapAnimationFrame(map, 30);

            map = new Map();
            map.Create("OpaFlight3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(4);
            map[1, 0] = new MapTileDescriptor(5);
            map[0, 1] = new MapTileDescriptor(14);
            map[1, 1] = new MapTileDescriptor(15);
            frames[2] = new MapAnimationFrame(map, 30);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateKaboomAnimation()
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[2];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Kaboom1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(76, 77, 86, 87);
            frames[0] = new MapAnimationFrame(map, 15);

            map = new Map();
            map.Create("Kaboom2", 3, 3, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(97, 98, 99, 107, 108, 109, 117, 118, 119);
            frames[1] = new MapAnimationFrame(map, 15, false, false, -4, -4);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateBabyAnimation()
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[3];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Baby1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(22, 23, 32, 33);
            frames[0] = new MapAnimationFrame(map, 10);

            map = new Map();
            map.Create("Baby2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(24, 25, 34, 35);
            frames[1] = new MapAnimationFrame(map, 10);

            map = new Map();
            map.Create("Baby3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(26, 27, 36, 37);
            frames[2] = new MapAnimationFrame(map, 10);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateDuckAnimation()
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[3];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Duck1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(6, 7, 16, 17);
            frames[0] = new MapAnimationFrame(map, 10);

            map = new Map();
            map.Create("Duck2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(8, 9, 18, 19);
            frames[1] = new MapAnimationFrame(map, 10);

            map = new Map();
            map.Create("Duck3", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(20, 21, 30, 31);
            frames[2] = new MapAnimationFrame(map, 10);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateFlyAnimation()
        {
            Animator animator = new Animator();
            MapAnimationFrame[] frames = new MapAnimationFrame[2];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Fly1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(28, 29, 38, 39);
            frames[0] = new MapAnimationFrame(map, 3);

            map = new Map();
            map.Create("Fly2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(40, 41, 50, 51);
            frames[1] = new MapAnimationFrame(map, 3);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateDeathStartAnimation()
        {
            Animator animator = new Animator();
            AnimationFrame[] frames = new AnimationFrame[3];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Star1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(78, 79, 88, 89);
            frames[0] = new MapAnimationFrame(map, 1);

            map = new Map();
            map.Create("Star2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(198, 199, 208, 209);
            frames[1] = new MapAnimationFrame(map, 1);

            frames[2] = new TileAnimationFrame(tileSheet, 207, 1, false, false, 4, 4);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateCoin1Animation()
        {
            Animator animator = new Animator();
            AnimationFrame[] frames = new AnimationFrame[2];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Coin1-1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(110, 111, 120, 121);
            frames[0] = new MapAnimationFrame(map, 1);

            map = new Map();
            map.Create("Coin1-2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(112, 113, 122, 123);
            frames[1] = new MapAnimationFrame(map, 1);

            animator.Initialize(frames);

            return animator;
        }

        public static Animator CreateCoin5Animation()
        {
            Animator animator = new Animator();
            AnimationFrame[] frames = new AnimationFrame[2];
            var tileSheet = AssetStore.Tiles;

            var map = new Map();
            map.Create("Coin5-1", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(114, 115, 124, 125);
            frames[0] = new MapAnimationFrame(map, 1);

            map = new Map();
            map.Create("Coin5-2", 2, 2, tileSheet, MapTileDescriptor.HiddenTile);
            map.SetTiles(130, 131, 140, 141);
            frames[1] = new MapAnimationFrame(map, 1);

            animator.Initialize(frames);

            return animator;
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

            OpaCursorAnimator = CreateOpaCursorAnimation(Tiles);
            OpaFlightAnimator = CreateOpaFlightAnimation(Tiles);
            OpaWalkAnimator = CreateOpaWalkAnimation(Tiles);

            MotherTired = CreateMotherTiredMap(Tiles);

            var fontSheet = videoMemory.CreateFontSheet("font");
            Font = CreateFont(fontSheet);

            // Son

            await audio.PreloadAsync("homeSound", 1);
            await audio.PreloadAsync("playSound", 1);
            await audio.PreloadAsync("bombSound", 2);
            await audio.PreloadAsync("ammoSound", 2);
            await audio.PreloadAsync("startSound", 3);
            await audio.PreloadAsync("opaExplosionSound", 1);
            await audio.PreloadAsync("motherExplosionSound", 1);
            await audio.PreloadAsync("monsterExplosionSound", 1);
            await audio.PreloadAsync("selectSound", 1);
            await audio.PreloadAsync("menuSound", 1);
            await audio.PreloadAsync("bombExplosionSound", 1);
            await audio.PreloadAsync("ammoExplosionSound", 1);
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

        public static Animator OpaCursorAnimator
        {
            get;
            private set;
        }

        public static Animator OpaFlightAnimator
        {
            get;
            private set;
        }

        public static Animator OpaWalkAnimator
        {
            get;
            private set;
        }

        public static Map MotherTired
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
