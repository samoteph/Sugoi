using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SurfaceFontSheet : SurfaceTileSheet
    {
        public int BankCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Nombre de tuile dans une bank
        /// </summary>

        public int TileHeightBank
        {
            get;
            private set;
        }

        public int TileSizeBank
        {
            get;
            private set;
        }

        /// <summary>
        /// Numero de la bank en cours
        /// </summary>
        public int BankNumber
        {
            get;
            private set;
        } = 0;

        public int AddressBank0
        {
            get;
            private set;
        }

        public FontTypes FontType
        {
            get;
            private set;
        }

        protected void Create(FontTypes fontType, int width, int height, int tileWidth, int tileHeight, int tileHeightBank, int bankCount)
        {
            this.Create(width, height, tileWidth, tileHeight);

            this.BankCount = bankCount;
            this.TileHeightBank = tileHeightBank;
            this.TileSizeBank = (tileHeightBank * tileHeight) * width;
            this.AddressBank0 = this.Address;
            this.FontType = FontType;
        }

        public void Initialize(Argb32[] pixels, int address, FontTypes fontType, int width, int height, int tileWidth, int tileHeight, int tileHeightBank, int bankCount)
        {
            this.Initialize(pixels, address, width, height, tileWidth, tileHeight);

            this.BankCount = bankCount;
            this.TileHeightBank = tileHeightBank;
            this.TileSizeBank = (tileHeightBank * tileHeight) * width;
            this.AddressBank0 = this.Address;
            this.FontType = fontType;
        }

        /// <summary>
        /// Change l'adress de la surface Font selon la Bank
        /// </summary>
        /// <param name="bank"></param>

        public void SetBank(int bank)
        {
            if(bank >= 0 && bank < this.BankCount)
            {
                this.Address = this.AddressBank0 + bank * TileSizeBank;
            }
        }
    }
}
