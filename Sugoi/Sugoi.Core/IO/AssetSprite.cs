using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.IO
{
    /// <summary>
    /// AssetSprite
    /// </summary>

    public class AssetSprite : AssetImage
    {
        public override AssetType Type
        {
            get
            {
                return AssetType.Sprite;
            }
        }

        public static AssetSprite Import(string assetName, Stream stream)
        {
            AssetSprite sprite = new AssetSprite();
            sprite.Name = assetName;
            sprite.ImportImage(stream);
            return sprite;
        }
    }
}
