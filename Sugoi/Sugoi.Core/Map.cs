using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    [DebuggerDisplay("Map '{Name}'")]
    public class Map
    {
        public MapTileDescriptor[] Tiles
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Récupération sans control 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>

        public MapTileDescriptor this[int x, int y]
        {
            get
            {
                return Tiles[x + (y * this.MapWidth)];
            }

            set
            {
                Tiles[x + (y * this.MapWidth)] = value;
            }
        }

        /// <summary>
        /// Largeur exprimée en tuile
        /// </summary>

        public int MapWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// Largeur exprimée en pixel
        /// </summary>

        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Hauteur exprimé en tuile
        /// </summary>

        public int MapHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// Hauteur exprimée en pixels
        /// </summary>

        public int Height
        {
            get;
            private set;
        }

        public SurfaceTileSheet TileSheet
        {
            get;
            set;
        }

        public void Clear(MapTileDescriptor tile)
        {
            for(int i=0; i<this.Tiles.Length;i++)
            {
                this.Tiles[i] = tile;
            }
        }

        public void Create(AssetMap map, VideoMemory videoMemory)
        {
            var surfaceTileSheet = videoMemory.GetSprite<SurfaceTileSheet>(map.AssetTileSheetName);

            this.Name = map.Name;
            this.MapWidth = map.MapWidth;
            this.MapHeight = map.MapHeight;
            this.Tiles = new MapTileDescriptor[map.MapWidth * map.MapHeight];

            for(int index=0; index < this.Tiles.Length; index++)
            {
                this.Tiles[index] = map.Tiles[index];
            }

            this.TileSheet = surfaceTileSheet;

            this.Width = map.MapWidth * surfaceTileSheet.TileWidth;
            this.Height = map.MapHeight * surfaceTileSheet.TileHeight;
        }

        public void Create(string name, int mapWidth, int mapHeight, SurfaceTileSheet surfaceTileSheet, MapTileDescriptor defaultTile)
        {
            this.Name = name;
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.Tiles = new MapTileDescriptor[mapWidth * mapHeight];
            this.TileSheet = surfaceTileSheet;

            this.Width = mapWidth * surfaceTileSheet.TileWidth;
            this.Height = mapHeight * surfaceTileSheet.TileHeight;

            this.Clear(defaultTile);
        }

        public void Initialize(string name, MapTileDescriptor[] tiles, int mapWidth, int mapHeight, SurfaceTileSheet surfaceTileSheet)
        {
            if(tiles.Length != mapWidth * mapHeight)
            {
                throw new Exception("The size of the tiles must be equal mapWidth * mapHeight!");
            }

            this.Name = name;

            this.TileSheet = surfaceTileSheet;
            this.Tiles = tiles;
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
        }

        public MapTileDescriptor? GetTile(int xMap, int yMap)
        {
            var index = xMap + yMap * MapWidth;

            if (index < 0 && index >= Tiles.Length)
            {
                return null;
            }

            return this.Tiles[index];
        }

        /// <summary>
        /// Affectation managée de tile 
        /// </summary>
        /// <param name="xMap"></param>
        /// <param name="yMap"></param>
        /// <param name="tile"></param>

        public void SetTile(int xMap, int yMap, MapTileDescriptor tile)
        {
            var index = xMap + yMap * MapWidth;

            if (index < 0 || index >= Tiles.Length)
            {
                return;
            }

            this.Tiles[index] = tile;
        }

        public void SetTiles(params int[] numbers)
        {
            var length = numbers.Length;

            if(Tiles.Length < numbers.Length)
            {
                length = Tiles.Length;
            }

            for(int i=0; i < numbers.Length; i++)
            {
                this.Tiles[i] = new MapTileDescriptor(numbers[i]);
            }
        }
    }
}
