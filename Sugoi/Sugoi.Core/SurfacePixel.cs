using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SurfacePixel
    {
        public Argb32[] Pixels
        {
            get;
            private set;
        }

        public int Address
        {
            get;
            protected set;
        }

        public int Size
        {
            get;
            private set;
        }

        protected void Create(int size)
        {
            this.Pixels = new Argb32[size];
            this.Size = size;
        }

        protected void Initialize(Argb32[] pixels, int address, int size)
        {
            this.Pixels = pixels;
            this.Address = address;
            this.Size = size;
        }

        /// <summary>
        /// Copy a block of memory in another place
        /// </summary>
        /// <param name="relativeAddressSource">Relative to </param>
        /// <param name="addressDestination"></param>
        /// <param name="size"></param>

        public void Copy(int positionSource, int positionDestination, int size)
        {
            var source = positionSource + Address;
            var destination = positionDestination + Address;

            for (int s = 0; s < size; s++)
            {
                this.Pixels[destination++] = this.Pixels[source++];
            }
        }

        public void Copy(SurfacePixel surfaceSource, int positionSource, int positionDestination, int size)
        {
            var source = positionSource + surfaceSource.Address;
            var destination = positionDestination + Address;

            var pixelsSource = surfaceSource.Pixels;

            for (int s = 0; s < size; s++)
            {
                this.Pixels[destination++] = pixelsSource[source++];
            }
        }

        public void Copy(Argb32[] pixelsSource, int addressSource, int positionSource, int positionDestination)
        {
            var source = positionSource + addressSource;
            var destination = positionDestination + Address;
            var size = pixelsSource.Length;

            for (int s = 0; s < size; s++)
            {
                this.Pixels[destination++] = pixelsSource[source++];
            }
        }
    }
}
