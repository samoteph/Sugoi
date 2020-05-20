using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.Core.IO
{
    /// <summary>
    /// AssetSprite
    /// </summary>

    public class AssetSprite : AssetImage
    {
        public AssetSprite(Cartridge cartridge) : base(cartridge)
        {
        }

        public override AssetTypes Type
        {
            get
            {
                return AssetTypes.Sprite;
            }
        }

        public static AssetSprite Import(Cartridge cartridge, string assetName, Stream stream)
        {
            AssetSprite sprite = new AssetSprite(cartridge);
            sprite.Name = assetName;
            sprite.ImportImage(stream);
            return sprite;
        }
    }
}
