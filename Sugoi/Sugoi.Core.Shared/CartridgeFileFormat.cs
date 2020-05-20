using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core.Shared
{
    public class CartridgeFileFormat
    {
        public const string HEADER_FILE = "MCP";
        public const int    TITLE_LENGTH = 30;
        public const int    PUBLISHER_LENGTH = 30;
        public const int    DESCRIPTION_LENGTH = 200;

        public const int    ASSET_NAME_LENGTH = 30;
        public const int    MINIMAL_ASSET_HEADER_SIZE = ASSET_NAME_LENGTH + 4; // + 4 c'est la taille du type de l'asset


        public const int BANK_COLORS_COUNT = 10;
        public const int BANK_COLORS_LENGTH = BANK_COLORS_COUNT * 4;
    }

    public enum AssetTypes
    {
        None        = -1,
        Sprite      = 0,
        TileSheet   = 1,
        FontSheet   = 2,
        // Fichier de l'application Tiled
        MapTmx      = 3,
        // Map de base
        Map         = 4,
        Sound       = 5,
        File        = 6,
    }
}
