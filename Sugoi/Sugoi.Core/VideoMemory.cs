using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class VideoMemory : SurfacePixel
    {
        int screenWidth;
        int screenHeight;

        public Dictionary<string, SurfaceSprite> sprites = new Dictionary<string, SurfaceSprite>(1000);

        /// <summary>
        /// Allocation de la video Memoire
        /// </summary>
        /// <param name="size"></param>

        internal void Start(int size, Screen screen)
        {
            // seulement pour la creation de sprite de la taille de la fenetre
            this.screenWidth = screen.Width;
            this.screenHeight = screen.Height;

            // creation de la mémoire vidéo
            this.Create(size);
        }

        internal void Stop()
        {
            // ici on libère la mémoire
        }

        /// <summary>
        /// 
        /// </summary>

        private int CurrentAddress
        {
            get;
            set;
        } = 0;

        /// <summary>
        /// Reserver une emplacement memoire et Copier la data selon l'asset
        /// </summary>
        /// <param name="pixels"></param>

        public SurfaceSprite CreateSprite(AssetSprite asset)
        {
            var sprite = CreateEmptySprite(asset.Name, asset.Width, asset.Height);
            // Copier les pixels de Asset et les mettre à l'address reservé
            this.Copy(asset.Pixels, 0, 0, sprite.Address);

            return sprite;
        }

        public SurfaceTileSheet CreateTileSheet(AssetTileSheet asset)
        {
            var tileSheet = new SurfaceTileSheet();
            var address = ReserveEmptySprite(tileSheet, asset.Name, asset.Width, asset.Height);
            
            tileSheet.Initialize(this.Pixels, address, asset.Width, asset.Height, asset.TileWidth, asset.TileHeight);
            
            // Copier les pixels de Asset et les mettre à l'address reservé
            this.Copy(asset.Pixels, 0, 0, tileSheet.Address);

            return tileSheet;
        }

        public SurfaceSprite CreateEmptySprite(string name, int width, int height)
        {
            var sprite = new SurfaceSprite();

            var address = this.ReserveEmptySprite(sprite, name, width, height);
            sprite.Initialize(this.Pixels, address, width, height);

            return sprite;
        }

        /// <summary>
        /// Creer un sprite vide
        /// </summary>
        /// <returns></returns>

        private int ReserveEmptySprite(SurfaceSprite sprite, string name, int width, int height)
        {
            var address = this.CurrentAddress;

            this.sprites.Add(name, sprite);

            // on ne peut pas se servire de sprite.Size car le sprite n'est pas encore initialisé
            int size = width * height;
            this.CurrentAddress += size;

            return address;
        }

        /// <summary>
        /// Creation d'un sprte de la taille de l'écran
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        public SurfaceSprite CreateWindow(string name)
        {
            return this.CreateEmptySprite(name, this.screenWidth, this.screenHeight);
        }

        public T GetSprite<T>(string name) where T : SurfaceSprite
        {
            return (T)sprites[name];
        }
    }
}
