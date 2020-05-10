using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sugoi.Core.IO.Builders
{
    public class ManifestAssetMapTmx : ManifestAsset
    {
        public ManifestAssetMapTmx(ManifestCartridge manifestCartridge) : base(manifestCartridge)
        {

        }

        public string TileSheetName
        {
            get;
            private set;
        }

        public override void Read(XmlElement node)
        {
            base.Read(node);

            this.TileSheetName = this.GetAttribute(node, "TileSheet", false);
        }

        protected override void WriteHeader(BinaryWriter writer, int externalSize)
        {
            base.WriteHeader(writer, externalSize + CartridgeFileFormat.ASSET_NAME_LENGTH);
            this.WriteString(writer, this.TileSheetName, CartridgeFileFormat.ASSET_NAME_LENGTH);
        }
    }
}
