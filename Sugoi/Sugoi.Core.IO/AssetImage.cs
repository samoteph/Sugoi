using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core.IO
{
    public abstract class AssetImage : Asset
    {
        public AssetImage(Cartridge cartridge) : base(cartridge)
        {
        }

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

        public override Task<bool> ReadAsync(BinaryReader reader)
        {
            this.ReadHeader(reader);
            this.ImportImage(reader.BaseStream);

            return Task.FromResult(true);
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
                if(image.TryGetSinglePixelSpan(out var span))
                {
                    array = span.ToArray();
                }
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
