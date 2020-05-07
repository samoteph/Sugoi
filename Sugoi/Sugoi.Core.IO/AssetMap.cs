using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TiledSharp;

namespace Sugoi.Core.IO
{
    public class AssetMap : Asset
    {
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

        public MapTileDescriptor[] Tiles
        {
            get;
            private set;
        }

        public string AssetTileSheetName
        {
            get;
            private set;
        }

        public override void ReadPackage(Stream stream)
        {
        }

        public static List<AssetMap> Import(string assetTileSheetName, Stream stream)
        {
            List<AssetMap> maps = new List<AssetMap>();
            TmxMap tmxMap = new TmxMap(stream);

            foreach(var layer in tmxMap.Layers)
            {
                AssetMap map = new AssetMap();

                map.Name = layer.Name;
                map.Tiles = new MapTileDescriptor[layer.Tiles.Count];

                map.MapWidth = tmxMap.Width;
                map.MapHeight = tmxMap.Height;

                map.AssetTileSheetName = assetTileSheetName;

                for(int index = 0; index < layer.Tiles.Count; index++)
                {
                    var tile = layer.Tiles[index];

                    int number = tile.Gid - 1;

                    var isHidden = number == -1;

                    if (isHidden == true)
                    {
                        number = 0;
                    }

                    if(number != 0)
                    {

                    }

                    map.Tiles[index] = new MapTileDescriptor() { number = number, hidden = isHidden, isHorizontalFlipped = tile.HorizontalFlip, isVerticalFlipped = tile.VerticalFlip };
                }

                maps.Add(map);
            }

            return maps;

        }

        /// <summary>
        /// uint -> bit 31 = flip horizontal
        /// uint -> bit 30 = flip vertical
        /// le reste number data
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="encodedTilesNumber"></param>
        /// <returns></returns>

        public static AssetMap Import(string assetName, string assetTileSheetName, int mapWidth, int mapHeight, uint[] encodedTilesNumber)
        {
            AssetMap map = new AssetMap();
            map.Name = assetName;

            map.Tiles = new MapTileDescriptor[encodedTilesNumber.Length];

            for(int index = 0; index < encodedTilesNumber.Length; index++)
            {
                var encodedTileNumber = encodedTilesNumber[index];

                map.Tiles[index] = CreateTile(encodedTileNumber);        
             }

            map.MapWidth = mapWidth;
            map.MapHeight = mapHeight;

            map.AssetTileSheetName = assetTileSheetName;

            return map;
        }

        private static MapTileDescriptor CreateTile(uint encodedTileNumber)
        {
            int number = (int)(encodedTileNumber & 0x1FFFFFFF) -1;

            var isHidden = number == -1;

            if(isHidden == true)
            {
                number = 0;
            }

            var isHorizontalFlip = (encodedTileNumber & 0x80000000) == 0x80000000;
            var isVerticalFlip = (encodedTileNumber & 0x40000000) == 0x40000000;

            var tile = new MapTileDescriptor() { number = number, hidden = isHidden, isHorizontalFlipped = isHorizontalFlip, isVerticalFlipped = isVerticalFlip };

            return tile;
        }
    }
}
