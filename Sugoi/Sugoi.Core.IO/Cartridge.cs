using System;
using System.Collections.Generic;
using System.IO;

namespace Sugoi.Core.IO
{
    public class Cartridge
    {
        Dictionary<string, Asset> assets;

        public Cartridge()
        {
            this.assets = new Dictionary<string, Asset>(100);
            this.Header = new CartridgeHeader();
        }

        public CartridgeHeader Header
        {
            get;
            set;
        }

        public void LoadHeader(string filename)
        {
            using (var fileStream = File.OpenRead(filename))
            {
                this.Header = new CartridgeHeader();
                this.Header.Read(fileStream);
            }
        }

        public void Load(string filename)
        {
            using (var fileStream = File.OpenRead(filename))
            {
                this.Header = new CartridgeHeader();
                this.Header.Read(fileStream);

                //while(fileStream.CanRead)
                //{
                //    // Lecture des Assets
                //    // Ajout dans le dico
                //}
            }
        }

        public void Save(string filename)
        {
        }

        /// <summary>
        /// Integration d'un nouvelle asset dans la cartouche
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>

        public void Import(Asset asset)
        {
            string assetName = asset.Name;

            if(this.assets.ContainsKey(assetName) == true)
            {
                throw new Exception("Cette clé existe déjà");
            }

            this.assets.Add(assetName, asset);
        }

        /// <summary>
        /// Lecture de l'asset
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>

        public T GetAsset<T>(string assetName) where T:Asset
        {
            return (T)this.assets[assetName];
        }
    }
}
