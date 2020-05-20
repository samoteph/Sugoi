using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.Core.IO
{
    /// <summary>
    /// Les fontes peuvent être PolyChromeStatic ou MonochromeDynamic
    /// PolychromeStatic-> utilisé tel quelle. Elles peuvent contenir plusieurs fontes à la suite (bank). Pour changer rapidement de fonte on change de bank. Pour calculer le nombre de bank on utilise MapHeightBank qui donne le nombre de tuile en hauteur constituant la fonte. 
    /// MonochomeDynamic -> La fonte est généré dynamiquement selon les couleurs fournis. Toute pixel non transparente est remplacé par une des couleurs en paramètres. S'il y a plusieurs couleurs, le même système de bank que les Polychromestatic est mise en place. 
    /// </summary>

    public class AssetFontSheet : AssetTileSheet
    {
        public AssetFontSheet(Cartridge cartridge) : base(cartridge)
        {
        }

        public override AssetTypes Type
        {
            get
            {
                return AssetTypes.FontSheet;
            }
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

        public Argb32[] BankColors
        {
            get;
            private set;
        }

        protected override void ReadHeader(BinaryReader reader)
        {
            // FORMAT des Assets
            // 00 : Taille en octet de l'asset
            // 04 : Type de l'asset (Sprite, TileSheet,...)
            // 08 : Nom de l'asset
            // 38 : info complementaire de taille n
            // 38 + n : File

            base.ReadHeader(reader);

            // lecture du type de fonte
            this.FontType = (FontTypes)reader.ReadInt32();
            this.MapHeightBank = reader.ReadInt32();

            // Chargement des couleurs
            this.ReadBankColors(reader); 

            // Calcul des banks
            PrepareBank(this, this.TileWidth, this.TileHeight, this.FontType, this.MapHeightBank);
        }

        private void ReadBankColors(BinaryReader reader)
        {
            byte[] bankColorsArray = reader.ReadBytes(CartridgeFileFormat.BANK_COLORS_LENGTH);
            uint[] bankColors = new uint[CartridgeFileFormat.BANK_COLORS_LENGTH];

            Buffer.BlockCopy(bankColorsArray, 0, bankColors, 0, bankColors.Length);

            // nombre de couleur dispo
            int countColor = 0;

            for (countColor = 0; countColor < bankColors.Length; countColor++)
            {
                if (bankColors[countColor] == 0)
                {
                    break;
                }
            }

            if (countColor > 0)
            {
                this.BankColors = new Argb32[countColor];

                for (int i = 0; i < BankColors.Length; i++)
                {
                    this.BankColors[i] = new Argb32(bankColors[i]);
                }
            }
        }

        private static void PrepareBank(AssetFontSheet font, int tileWidth, int tileHeight, FontTypes fontType, int mapHeightBank)
        {
            font.TileWidth = tileWidth;
            font.TileHeight = tileHeight;

            int fontMapHeight = font.Height / tileHeight;

            switch (fontType)
            {
                case FontTypes.PolychromeStatic:

                    if (mapHeightBank <= 0)
                    {
                        mapHeightBank = fontMapHeight;
                    }
                    font.BankCount = fontMapHeight / mapHeightBank;
                    break;

                case FontTypes.MonochromeDynamic:
                    // en mono il n'y a qu'une seule bank au départ
                    mapHeightBank = fontMapHeight;
                    font.BankCount = 1;
                    break;
            }

            font.MapHeightBank = mapHeightBank;
            font.FontType = fontType;
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

    public static AssetFontSheet Import(Cartridge cartridge, string assetName, Stream stream, int tileWidth, int tileHeight, FontTypes fontType, int mapHeightBank)
        {
            AssetFontSheet font = new AssetFontSheet(cartridge);
            font.Name = assetName;
            font.ImportImage(stream);

            font.TileWidth = tileWidth;
            font.TileHeight = tileHeight;

            PrepareBank(font, tileWidth, tileHeight, fontType, mapHeightBank);

            return font;
        }
    }

    public enum FontTypes
    {
        PolychromeStatic,
        MonochromeDynamic
    }
}
