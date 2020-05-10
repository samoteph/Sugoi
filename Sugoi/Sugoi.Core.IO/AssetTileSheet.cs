using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.Core.IO
{
    public class AssetTileSheet : AssetImage
    {
        public override AssetTypes Type
        {
            get
            {
                return AssetTypes.TileSheet;
            }
        }

        public int TileWidth
        {
            get;
            protected set;
        }

        public int TileHeight
        {
            get;
            protected set;
        }

        protected override void ReadHeader(BinaryReader reader)
        {
            // lecture du nom ici
            base.ReadHeader(reader);

            this.TileWidth = reader.ReadInt32();
            this.TileHeight = reader.ReadInt32();
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
