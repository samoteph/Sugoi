using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using Sugoi.Core;
using Rgba32 = SixLabors.ImageSharp.PixelFormats.Rgba32;

namespace Sugoi.IO
{
    public abstract class Asset
    {
        public string Name
        {
            get;
            protected set;
        }

        public virtual AssetType Type
        {
            get;
        }

        public abstract void ReadPackage(Stream stream);
    }

    public enum AssetType
    {
        Sprite,
        Map,
        TileSheet,
        Sound,
        File
    }
}
