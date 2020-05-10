using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sugoi.Core.IO.Builders
{
    public class ManifestAssetTileSheet : ManifestAsset
    {
        public ManifestAssetTileSheet(ManifestCartridge manifestCartridge) : base(manifestCartridge)
        {

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

        public override void Read(XmlElement node)
        {
            base.Read(node);
            this.TileWidth = this.GetIntAttribute(node, "TileWidth", true, -1);
            this.TileHeight = this.GetIntAttribute(node, "TileHeight", true, -1);

            if(TileWidth <= 0 || TileHeight <= 0)
            {
                throw new Exception("The tilesheet '" + this.Name + "' must have a correct tile size (TimeWidth/TileHeight");
            }
        }

        protected override void WriteHeader(BinaryWriter writer, int externalSize)
        {
            // ecriture de l'entete de base
            base.WriteHeader(writer, externalSize + 8);
            // ecriture des infos complemetaire
            writer.Write(this.TileWidth);
            writer.Write(this.TileHeight);
        }
    }
}
