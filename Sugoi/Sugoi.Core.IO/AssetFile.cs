using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core.IO
{
    public class AssetFile : Asset
    {
        public AssetFile(Cartridge cartridge) : base(cartridge)
        {
        }

        public override AssetTypes Type
        {
            get
            {
                return AssetTypes.File;
            }
        }

        public override Task<bool> ReadAsync(BinaryReader reader)
        {
            base.ReadHeader(reader);

            int count = this.Size - CartridgeFileFormat.MINIMAL_ASSET_HEADER_SIZE;

            if(this.cartridge.ExportFileAsyncCallback == null)
            {
                throw new Exception("Please set a callback to ExportFileAsyncCallback!");
            }

            Debug.WriteLine("ExportFile=" + this.Name);

            return this.cartridge.ExportFileAsyncCallback.Invoke(this.Name, reader, count);
        }
    }
}
