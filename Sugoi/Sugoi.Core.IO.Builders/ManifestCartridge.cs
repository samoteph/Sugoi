using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sugoi.Core.IO.Builders
{
    public class ManifestCartridge : ManifestNode
    {
        public byte Format
        {
            get;
            set;
        }

        public int Version
        {
            get;
            set;
        }

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

        public int VideoMemorySize
        {
            get;
            set;
        } = 20 * 1000 * 1000; // 20 Mo

        public List<ManifestAsset> Assets
        {
            get;
            private set;
        }

        public Manifest Manifest
        {
            get;
            private set;
        }

        public ManifestCartridge(Manifest manifest)
        {
            this.Manifest = manifest;
        }

        /// <summary>
        /// Lecture
        /// </summary>
        /// <param name="nodeCartridge"></param>

        public override void Read(XmlElement nodeCartridge)
        {
            this.Assets = new List<ManifestAsset>(10);

            this.Format = this.GetByteAttribute(nodeCartridge, "Format", false, 1);
            this.Version = this.GetIntAttribute(nodeCartridge, "Version", false, 1);
            this.Title = this.GetAttribute(nodeCartridge, "Title", true);
            this.Publisher = this.GetAttribute(nodeCartridge, "Publisher", true);

            var descriptionNode = (XmlElement)nodeCartridge.FirstChild;
            
            if(descriptionNode == null)
            {
                throw new Exception("Description node not found in the Cartridge node of the manifest!");
            }

            this.Description = descriptionNode.InnerText;

            var assetsNode = (XmlElement)descriptionNode.NextSibling;

            if(assetsNode == null)
            {
                throw new Exception("Assets node not found in the Cartridge node of the manifest!");
            }

            foreach (var objectNode in assetsNode.ChildNodes)
            {
                var node = objectNode as XmlElement;

                if (node != null)
                {
                    if (node.LocalName == "Asset")
                    {
                        string type = this.GetAttribute(node, "Type", true).ToLower();

                        ManifestAsset asset = null;

                        switch(type)
                        {
                            case "sprite":
                                asset = new ManifestAssetSprite(this);
                                break;
                            case "tilesheet":
                                asset = new ManifestAssetTileSheet(this);
                                break;
                            case "fontsheet":
                                asset = new ManifestAssetFontSheet(this);
                                break;
                            case "maptmx":
                                asset = new ManifestAssetMapTmx(this);
                                break;
                        }

                        if(asset == null)
                        {
                            throw new Exception("The type '" + type + "' is unknown!");
                        }

                        // Lecture du node selon le type
                        asset.Read(node);

                        this.Assets.Add(asset);
                    }
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            // Ecriture du header

            writer.Write(CartridgeFileFormat.HEADER_FILE.ToCharArray());

            writer.Write(this.Format);
            writer.Write(this.Version);

            this.WriteString(writer, this.Title, CartridgeFileFormat.TITLE_LENGTH);
            this.WriteString(writer, this.Publisher, CartridgeFileFormat.PUBLISHER_LENGTH);
            this.WriteString(writer, this.Description, CartridgeFileFormat.DESCRIPTION_LENGTH);

            writer.Write(this.VideoMemorySize);

            foreach(var asset in this.Assets)
            {
                asset.Write(writer);
            }
        }
    }
}
