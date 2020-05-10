using Sugoi.Core.Shared;
using System;
using System.IO;

namespace Sugoi.Core.IO
{
    public class CartridgeHeader
    {
        /// <summary>
        /// Format de la cartouche (1 correspond à la version 1 du format fichier)
        /// </summary>

        public int Format
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// Version du jeu
        /// </summary>

        public int Version
        {
            get;
            set;
        } = 0;

        public string Title
        {
            get;
            set;
        }

        public string Publisher
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Taille de la VideoMemoory En octets
        /// </summary>

        public int VideoMemorySize
        {
            get;
            set;
        } = 1000 * 1000 * 20; // 20 mo par defaut de VideoMemory 

        public void Read(BinaryReader reader)
        {
            var headerFile = new string(reader.ReadChars(CartridgeFileFormat.HEADER_FILE.Length));

            if (headerFile != CartridgeFileFormat.HEADER_FILE)
            {
                throw new Exception("this file is not a compatible package for this console!");
            }

            Format = reader.ReadByte();
            Version = reader.ReadInt32();

            Title = ReadString(reader, CartridgeFileFormat.TITLE_LENGTH); 
            Publisher = ReadString(reader, CartridgeFileFormat.PUBLISHER_LENGTH);  
            Description = ReadString(reader, CartridgeFileFormat.DESCRIPTION_LENGTH);

            VideoMemorySize = reader.ReadInt32();
        }

        protected string ReadString(BinaryReader reader, int maxLength)
        {
            // peut contenir des \0 pour boucher
            var characters = reader.ReadChars(maxLength);

            int index;
            
            for(index=0; index < characters.Length; index++)
            {
                if(characters[index] == 0)
                {
                    break;
                }
            }

            return new string(characters, 0, index);
        }
    }
}
