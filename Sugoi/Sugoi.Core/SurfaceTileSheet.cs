using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SurfaceTileSheet : SurfaceSprite
    {
        /// <summary>
        /// width de la tile en pixel
        /// </summary>

        public int TileWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// height de la tile en pixel
        /// </summary>

        public int TileHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// Width en nombre de tile
        /// </summary>

        public int TileSheetWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// Height en nombre de tile
        /// </summary>

        public int TileSheetHeight
        {
            get;
            private set;
        }

        protected void Create(int width, int height, int tileWidth, int tileHeight)
        {
            this.Create(width,height);
            this.SetDimension(tileWidth, tileHeight);
        }

        public void Initialize(Argb32[] pixels, int address, int width, int height, int tileWidth, int tileHeight)
        {
            this.Initialize(pixels, address, width, height);
            this.SetDimension(tileWidth, tileHeight);
        }

        /// <summary>
        /// Determine le nombre de taille 
        /// </summary>

        private void SetDimension(int tileWidth, int tileHeight)
        {
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;

            if(this.Width % tileWidth != 0)
            {
                throw new Exception("The width of the image of the TileSheet must be a multiple of the tileWidth");
            }

            if (this.Height % tileHeight != 0)
            {
                throw new Exception("The height of the image of the TileSheet must be a multiple of the tileHeight");
            }

            var columns = this.Width / tileWidth;
            var rows = this.Height / tileHeight;

            this.TileSheetHeight = rows;
            this.TileSheetWidth = columns;
        }
    }
}
