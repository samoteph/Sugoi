using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class Map
    {
        public MapTileDescriptor[] Tiles
        {
            get;
            private set;
        }

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

        public int MapWidth
        {
            get;
            private set;
        }

        public int MapHeight
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

        public void Create(int mapWidth, int mapHeight, SurfaceTileSheet surfaceTileSheet, MapTileDescriptor defaultTile)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.Tiles = new MapTileDescriptor[mapWidth * mapHeight];
            this.TileSheet = surfaceTileSheet;

            this.Clear(defaultTile);
        }

        public void Initialize(MapTileDescriptor[] tiles, int mapWidth, int mapHeight, SurfaceTileSheet surfaceTileSheet)
        {
            if(tiles.Length != mapWidth * mapHeight)
            {
                throw new Exception("The size of the tiles must be equal mapWidth * mapHeight!");
            }

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

        public void SetTile(int xMap, int yMap, MapTileDescriptor tile)
        {
            var index = xMap + yMap * MapWidth;

            if (index < 0 && index >= Tiles.Length)
            {
                return;
            }

            this.Tiles[index] = tile;
        }
    }
}
