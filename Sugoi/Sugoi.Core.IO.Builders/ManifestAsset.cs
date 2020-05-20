using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sugoi.Core.IO.Builders
{
    public abstract class ManifestAsset : ManifestNode
    {
        public ManifestCartridge ManifestCartridge
        {
            get;
            private set;
        }

        public Manifest Manifest
        {
            get
            {
                return this.ManifestCartridge?.Manifest;
            }
        }

        public ManifestAsset(ManifestCartridge manifestCartridge)
        {
            this.ManifestCartridge = manifestCartridge;
        }

        public string Name
        {
            get;
            set;
        }

        public string Filename
        {
            get;
            set;
        }

        public AssetTypes Type
        {
            get;
            set;
        }

        public override void Read(XmlElement node)
        {
            this.Name = this.GetAttribute(node, "Name", true);
            this.Filename = this.GetAttribute(node, "Filename", true);
            this.Type = this.GetEnumAttribute(node, "Type", true, AssetTypes.None);

            if(this.Type == AssetTypes.None)
            {
                throw new Exception("The type of the name '" + this.Name + "' is not valid!");
            }
        }

        public override void Write(BinaryWriter writer)
        {
            // ajoute le path au nom de l'asset
            string fullFileName = Manifest.GetAssetFullFilename(this.Filename);
            byte[] fileArray = File.ReadAllBytes(fullFileName);
            
            this.WriteHeader(writer, fileArray.Length);

            // ecriture du fichier
            writer.Write(fileArray);

        }

        /// <summary>
        /// Ecriture du fichier externe dans la cartouche avec les infos complementaire (nom, ...)
        /// </summary>
        /// <param name="writer"></param>

        protected virtual void WriteHeader(BinaryWriter writer, int externalSize)
        {
            // FORMAT des Assets
            // 00 : Taille en octet de l'asset
            // 04 : Type de l'asset (Sprite, TileSheet,...)
            // 08 : Nom de l'asset
            // 38 : info complementaire de taille n
            // 38 + n : File


            // ecriture de la taille de l'asset (nom + fichier)
            int size = externalSize + CartridgeFileFormat.MINIMAL_ASSET_HEADER_SIZE;
            writer.Write(size); // 4 c'est la taille du type de l'asset

            // Type de l'asset
            int assetType = (int)this.Type;
            writer.Write(assetType);

            // ecriture du nom
            this.WriteString(writer, this.Name, CartridgeFileFormat.ASSET_NAME_LENGTH);
        }
    }
}
