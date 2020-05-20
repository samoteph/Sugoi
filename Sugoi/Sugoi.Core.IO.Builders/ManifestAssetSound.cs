using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.Core.IO.Builders
{
    public class ManifestAssetSound : ManifestAsset
    {
        public ManifestAssetSound(ManifestCartridge manifestCartridge) : base(manifestCartridge)
        {

        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
        }
    }
}
