using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Sugoi.Core.IO;

namespace Sugoi.Core.IO.Builders
{
    public class Manifest
    {
        public Manifest() 
        {
        }

        ManifestCartridge ManifestCartridge
        {
            get;
            set;
        }

        public string ManifestPath
        {
            get;
            private set;
        }

        public void Read(string manifestPath)
        {
            this.ManifestCartridge = null;
            this.ManifestPath = manifestPath;

            XmlDocument document = new XmlDocument();

            document.Load(manifestPath);

            var rootNode = document.FirstChild;

            if(rootNode.NodeType == XmlNodeType.XmlDeclaration)
            {
                rootNode = rootNode.NextSibling;
            }

            if(rootNode.LocalName != "Cartridge")
            {
                throw new Exception("The first node of the manifest should be a Cartridge Node !");
            }

            var cartridge = new ManifestCartridge(this);

            cartridge.Read((XmlElement)rootNode);

            this.ManifestCartridge = cartridge;
        }

        public string GetAssetFullFilename(string filename)
        {
            string folder = Path.GetDirectoryName(this.ManifestPath);
            return Path.Combine(folder, filename);
        }

        public void Build(string cartridgePath)
        {
            if(this.ManifestCartridge == null)
            {
                throw new Exception("The manifest of the cartridge must be read before the build of cartridge!");
            }

            using (var stream = File.OpenWrite(cartridgePath))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    this.ManifestCartridge.Write(writer);
                }
            }
        }
    }
}
