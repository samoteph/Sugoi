using System;
using System.IO;

namespace Sugoi.Core.IO
{
    public class CartridgeHeader
    {
        public const string HEADER_FILE = "MCP";
        public const int TITLE_LENGTH = 15;

        /// <summary>
        /// Format du fichier (1 correspond à la version 1 du format fichier)
        /// </summary>

        public byte Format
        {
            get;
            set;
        } = 1;

        public int Version
        {
            get;
            set;
        } = 0;

        public string GameTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// Taille de la VideoMemoory En octets
        /// </summary>

        public int VideoMemorySize
        {
            get;
            private set;
        } = 1000 * 1000 * 20; // 20 mo par defaut de VideoMemory 

        public void Read(Stream stream)
        {
            using( var reader = new BinaryReader(stream) )
            {
                var headerFile = new string(reader.ReadChars(HEADER_FILE.Length));

                if(headerFile != HEADER_FILE)
                {
                    throw new Exception("this file is not a compatible package for this console!");
                }

                Format = reader.ReadByte();
                Version = reader.ReadInt32();
                GameTitle = new string(reader.ReadChars(TITLE_LENGTH));
                VideoMemorySize = reader.ReadInt32();
            }
        }
    }
}
