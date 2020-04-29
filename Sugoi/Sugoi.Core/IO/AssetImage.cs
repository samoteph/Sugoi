using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.IO
{
    public abstract class AssetImage : Asset
    {
        public Core.Argb32[] Pixels
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Lecture à partir du package
        /// </summary>
        /// <param name="stream"></param>

        public override void ReadPackage(Stream stream)
        {
        }

        /// <summary>
        /// Importation de l'image transformation en data
        /// </summary>
        /// <param name="stream"></param>

        protected void ImportImage(Stream streamImage)
        {
            Rgba32[] array = null;

            using (Image<Rgba32> image = Image.Load<Rgba32>(streamImage))
            {
                array = image.GetPixelSpan().ToArray();
                this.Width = image.Width;
                this.Height = image.Height;
            }

            this.Pixels = new Core.Argb32[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                var pixelFrom = array[i];
                var pixelTo = new Core.Argb32(pixelFrom.R, pixelFrom.G, pixelFrom.B, pixelFrom.A);
                this.Pixels[i] = pixelTo;
            }
        }
    }
}
