using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.Core.IO
{
    public class AssetFont : AssetImage
    {
        public override AssetType Type
        {
            get
            {
                return AssetType.Font;
            }
        }

        public int TileWidth
        {
            get;
            private set;
        }

        public int TileHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// Nombre de tuile en hauteur dans une bank
        /// </summary>

        public int MapHeightBank
        {
            get;
            private set;
        }

        public int BankCount
        {
            get;
            private set;
        }

        public FontTypes FontType
        {
            get;
            private set;
        }

        /// <summary>
        /// Importation
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="stream"></param>
        /// <param name="fontType"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="mapHeightBank"></param>
        /// <returns></returns>

        public static AssetFont Import(string assetName, Stream stream, int tileWidth, int tileHeight, FontTypes fontType, int mapHeightBank)
        {
            AssetFont font = new AssetFont();
            font.Name = assetName;
            font.ImportImage(stream);

            font.TileWidth = tileWidth;
            font.TileHeight = tileHeight;

            int fontMapHeight = font.Height / tileHeight;

            switch(fontType)
            {
                case FontTypes.PolyChromeStatic:

                    if (mapHeightBank <= 0)
                    {
                        mapHeightBank = fontMapHeight;
                    }
                    font.BankCount = fontMapHeight / mapHeightBank;
                    break;

                case FontTypes.MonoChromeDynamic:
                    // en mono il n'y a qu'une seule bank au départ
                    mapHeightBank = fontMapHeight;
                    font.BankCount = 1;
                    break;
            }
            
            font.MapHeightBank = mapHeightBank;
            font.FontType = fontType;

            return font;
        }
    }

    public enum FontTypes
    {
        PolyChromeStatic,
        MonoChromeDynamic
    }
}
