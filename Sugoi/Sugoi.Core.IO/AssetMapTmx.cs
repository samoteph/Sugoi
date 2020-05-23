using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace Sugoi.Core.IO
{
    public class AssetMapTmx : Asset
    {
        public AssetMapTmx(Cartridge cartridge) : base(cartridge)
        {
        }

        public override AssetTypes Type
        {
            get
            {
                return AssetTypes.MapTmx;
            }
        }

        public string AssetTileSheetName
        {
            get;
            private set;
        }

        public List<AssetMap> Maps
        {
            get;
            private set;
        }

        public override Task<bool> ReadAsync(BinaryReader reader)
        {
            this.ReadHeader(reader);

            int count = this.Size - CartridgeFileFormat.MINIMAL_ASSET_HEADER_SIZE - CartridgeFileFormat.ASSET_NAME_LENGTH;

            // Le Import de TMX Map charge le stream jusqu'à la fin
            // on va donc lui proposer un Stream qui ne contient que le fichier TMX

            byte[] data = reader.ReadBytes(count);

            using (MemoryStream memory = new MemoryStream(data))
            {
                this.Maps = Import(this.cartridge, this.AssetTileSheetName, memory);
            }

            return Task.FromResult(true);
        }

        protected override void ReadHeader(BinaryReader reader)
        {
            base.ReadHeader(reader);
            this.AssetTileSheetName = this.ReadString(reader, CartridgeFileFormat.ASSET_NAME_LENGTH);
        }

        public static List<AssetMap> Import(Cartridge cartridge, string assetTileSheetName, Stream stream)
        {
            List<AssetMap> maps = new List<AssetMap>();
            
            TmxMap tmxMap = new TmxMap(stream);

            foreach (var layer in tmxMap.Layers)
            {
                var tiles = new MapTileDescriptor[layer.Tiles.Count];
                
                for (int index = 0; index < layer.Tiles.Count; index++)
                {
                    var tile = layer.Tiles[index];

                    int number = tile.Gid - 1;

                    var isHidden = number == -1;

                    if (isHidden == true)
                    {
                        number = 0;
                    }

                    if (number != 0)
                    {

                    }

                    tiles[index] = new MapTileDescriptor() { number = number, hidden = isHidden, isHorizontalFlipped = tile.HorizontalFlip, isVerticalFlipped = tile.VerticalFlip };
                }

                AssetMap map = AssetMap.Import(
                    cartridge,
                    layer.Name,
                    assetTileSheetName,
                    tmxMap.Width,
                    tmxMap.Height,
                    tiles
                    );

                maps.Add(map);
            }

            return maps;

        }
    }
}
