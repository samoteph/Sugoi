using Sugoi.Core.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core.IO
{
    public abstract class Cartridge
    {
        Dictionary<string, Asset> assets;

        public bool IsLoaded
        {
            get;
            private set;
        } = false;

        /// <summary>
        /// Ecriture de fichier externe
        /// </summary>

        public ExportFileAsyncHandler ExportFileAsyncCallback
        {
            get;
            set;
        }

        public Cartridge()
        {
            this.assets = new Dictionary<string, Asset>(100);
            this.Header = new CartridgeHeader();
        }

        public CartridgeHeader Header
        {
            get;
            private set;
        }

        public void LoadHeader(BinaryReader reader)
        {
            this.Header = new CartridgeHeader();
            this.Header.Read(reader);
        }

        public abstract Task LoadAsync();
        public abstract Task LoadHeaderAsync();

        /// <summary>
        /// Chargement de la cartridge qui doit se trouver en ressource embedded
        /// "CrazyZone.Cartridge.Cartridge.sugoi" -> Projet.Repertoire.Nom de fichier
        /// </summary>

        protected async Task LoadFromResourceAsync(string resourceName)
        {
            using (var stream = GetStreamResource(resourceName))
            {
                await this.LoadAsync(stream);
            }
        }

        protected Stream GetStreamResource(string resourceName)
        {
            var assembly = this.GetType().Assembly;

            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Chargement à partir d'un flux
        /// </summary>
        /// <param name="stream"></param>

        protected virtual async Task LoadAsync(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                this.LoadHeader(reader);

                while (stream.Position != stream.Length)
                {
                    var assetSize = reader.ReadInt32();
                    var assetType = (AssetTypes)reader.ReadInt32();

                    Asset asset = null;

                    switch(assetType)
                    {
                        default:
                            throw new Exception("Unknow assetType '" + assetType + "' !");

                        case AssetTypes.Sprite:
                            asset = new AssetSprite(this);
                            break;

                        case AssetTypes.TileSheet:
                            asset = new AssetTileSheet(this);
                            break;

                        case AssetTypes.FontSheet:
                            asset = new AssetFontSheet(this);
                            break;

                        case AssetTypes.MapTmx:
                            // MapTmx contient une liste d'AssetMap
                            asset = new AssetMapTmx(this);
                            break;

                        case AssetTypes.Sound:
                            asset = new AssetFile(this);
                            break;
                    }

                    asset.Size = assetSize;

                    await asset.ReadAsync(reader);
                    this.assets.Add(asset.Name, asset);
                }
            }

            this.IsLoaded = true;
        }

        /// <summary>
        /// Integration d'un nouvelle asset dans la cartouche
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>

        public void Import(Asset asset)
        {
            string assetName = asset.Name;

            if (this.assets.ContainsKey(assetName) == true)
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

        public T GetAsset<T>(string assetName) where T : Asset
        {
            return (T)this.assets[assetName];
        }

        public void ClearAssets()
        {
            this.assets.Clear();
        }

        public virtual void Stop()
        {

        }
    }

    public delegate Task<bool> ExportFileAsyncHandler(string name, BinaryReader stream, int count);
}
