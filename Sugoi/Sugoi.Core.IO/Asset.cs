using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using Sugoi.Core;
using Rgba32 = SixLabors.ImageSharp.PixelFormats.Rgba32;
using Sugoi.Core.Shared;

namespace Sugoi.Core.IO
{
    public abstract class Asset
    {
        public string Name
        {
            get;
            protected set;
        }

        public abstract AssetTypes Type
        {
            get;
        }

        public abstract void Read(BinaryReader reader);

        protected virtual void ReadHeader(BinaryReader reader)
        {
            // FORMAT des Assets
            // 00 : Taille en octet de l'asset
            // 04 : Type de l'asset (Sprite, TileSheet,...)
            // 08 : Nom de l'asset
            // 38 : info complementaire de taille n
            // 38 + n : File

            // normalement la taille et le type de l'asset sont déjà lu
            this.Name = this.ReadString(reader, CartridgeFileFormat.ASSET_NAME_LENGTH);
        }

        protected string ReadString(BinaryReader reader, int maxLength)
        {
            // peut contenir des \0 pour boucher
            var characters = reader.ReadChars(maxLength);

            int index;

            for (index = 0; index < characters.Length; index++)
            {
                if (characters[index] == 0)
                {
                    break;
                }
            }

            return new string(characters, 0, index);
        }
    }
}
