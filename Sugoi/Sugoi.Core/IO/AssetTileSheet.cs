using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.IO
{
    public class AssetTileSheet : AssetImage
    {
        public override AssetType Type
        {
            get
            {
                return AssetType.TileSheet;
            }
        }

        public int TileWidth
        {
            get;
            private set;
        }

        public int TileHeight
        {
            get;
            private set;
        }

        public static AssetTileSheet Import(string assetName, Stream stream, int tileWidth, int tileHeight)
        {
            AssetTileSheet tileSheet = new AssetTileSheet();
            tileSheet.Name = assetName;
            tileSheet.ImportImage(stream);

            tileSheet.TileWidth = tileWidth;
            tileSheet.TileHeight = tileHeight;

            return tileSheet;
        }
    }
}
