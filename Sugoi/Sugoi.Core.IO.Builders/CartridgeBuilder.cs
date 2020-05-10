using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sugoi.Core.IO.Builders
{
    public class CartridgeBuilder
    {
        /// <summary>
        /// analyse du dossier ou se trouve les assets et le manifest
        /// </summary>
        /// <param name="folder"></param>

        public void Build(string folder)
        {
            string pathManifest = Path.Combine(folder, "Manifest.xml");
            string pathCartridge = Path.Combine(folder, "Cartridge.sugoi");

            if( File.Exists(pathManifest) == false)
            {
                throw new FileNotFoundException(null, pathManifest);
            }

            Manifest manifest = new Manifest();

            manifest.Read(pathManifest);
            manifest.Build(pathCartridge);
        }       
    }
}
