using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sugoi.Core.IO.Builders
{
    public class ManifestAssetFontSheet : ManifestAssetTileSheet
    {
        public ManifestAssetFontSheet(ManifestCartridge manifestCartridge) : base(manifestCartridge)
        {

        }

        public FontTypes FontType
        {
            get;
            private set;
        }

        /// <summary>
        /// Nombre de tuile en hauteur composant la fonte (-1 si c'est la taille de la map)
        /// </summary>

        public int MapHeightBank
        {
            get;
            private set;
        }

        public uint[] BankColors
        {
            get;
            private set;
        }

        public override void Read(XmlElement node)
        {
            base.Read(node);

            this.FontType = this.GetEnumAttribute<FontTypes>(node, "FontType", true, FontTypes.None);
        
            if(this.FontType == FontTypes.None)
            {
                throw new Exception("The FontType attribute of the node '" + this.Name  +"' is not in a correct format!");
            }

            this.MapHeightBank = this.GetIntAttribute(node, "MapHeightBank", false);

            this.ReadBankColors(node);
        }

        private void ReadBankColors(XmlElement node)
        {
            var bankColorsString = this.GetAttribute(node, "BankColors", false);

            // 10 couleur max pour les banks monochromeDynamic
            this.BankColors = new uint[CartridgeFileFormat.BANK_COLORS_COUNT];

            if (bankColorsString != null)
            {
                string[] colors = bankColorsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                int index = 0;

                foreach (var color in colors)
                {
                    if (color.StartsWith("#"))
                    {
                        string argbString = color.Substring(1);
                        // bourre avec des F. Ainsi FF00FF devient FFFF00FF
                        argbString = argbString.PadLeft(8, 'F');

                        if (uint.TryParse(argbString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                        {
                            // trop de couleurs !
                            if (index >= this.BankColors.Length)
                            {
                                throw new Exception("Only " + this.BankColors.Length + " BankColors are allowed!");
                            }

                            this.BankColors[index] = result;
                            index++;
                        }
                        else
                        {
                            throw new Exception("This color '" + color + "' is not a valide color. The color must be exprimed in Hexadecimal number. (ie #FF00FF or #80FF00FF");
                        }
                    }
                    else
                    {
                        throw new Exception("This color '" + color + "' is not a valide color. The color must start with a # symbole and must be exprimed in Hexadecimal number. (ie #FF00FF or #80FF00FF");
                    }
                }
            }
        }

        protected override void WriteHeader(BinaryWriter writer, int externalSize)
        {
            base.WriteHeader(writer, externalSize + 4 + 4 + this.BankColors.Length * 4);
            
            writer.Write((int)this.FontType);
            writer.Write(this.MapHeightBank);

            // ecriture des 10 couleurs des fonts mono (par simplicité on les ecrit même en Polychrome)
            byte[] bankColorsArray = new byte[CartridgeFileFormat.BANK_COLORS_LENGTH];
            Buffer.BlockCopy(this.BankColors, 0, bankColorsArray, 0, CartridgeFileFormat.BANK_COLORS_LENGTH);

            writer.Write(bankColorsArray);
        }
    }

    public enum FontTypes
    {
        None = -1,
        PolychromeStatic,
        MonochromeStatic
    }
}
