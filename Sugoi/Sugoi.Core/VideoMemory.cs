using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class VideoMemory : SurfacePixel
    {
        private int screenWidth;
        private int screenHeight;
        private Machine machine;

        public Dictionary<string, SurfaceSprite> sprites = new Dictionary<string, SurfaceSprite>(1000);

        /// <summary>
        /// Allocation de la video Memoire
        /// </summary>
        /// <param name="size"></param>

        internal void Start(int size, Machine machine)
        {
            this.machine = machine;
            var screen = machine.Screen;

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

        public SurfaceSprite CreateSprite(string assetName)
        {
            return this.CreateSprite(this.machine.Cartridge.GetAsset<AssetSprite>(assetName));
        }

        public SurfaceTileSheet CreateTileSheet(string assetName)
        {
            return this.CreateTileSheet(this.machine.Cartridge.GetAsset<AssetTileSheet>(assetName));
        }

        public SurfaceFontSheet CreateFontSheet(string assetName)
        {
            var asset = this.machine.Cartridge.GetAsset<AssetFontSheet>(assetName);
            return CreateFontSheet(asset);
        }

        public SurfaceFontSheet CreateFontSheet(AssetFontSheet asset)
        {
            switch (asset.FontType)
            {
                case FontTypes.PolychromeStatic:
                    return CreateFontSheetPolyChromeStatic(asset);
                case FontTypes.MonochromeDynamic:
                    return CreateFontSheetMonoChromeDynamic(asset);
            }

            throw new Exception("The fontType is unknown !");
        }

        public Map[] CreateMapTmx(string assetMapTmxName)
        {
            var cartridge = this.machine.Cartridge;
            return this.CreateMapTmx(cartridge.GetAsset<AssetMapTmx>(assetMapTmxName));
        }

        public Map[] CreateMapTmx(AssetMapTmx asset)
        {
            var videoMemory = this.machine.VideoMemory;
            var maps = new Map[asset.Maps.Count];

            for (int l = 0; l < maps.Length; l++)
            {
                maps[l] = new Map();
                maps[l].Create(asset.Maps[l], videoMemory);
            }

            return maps;
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

        public SurfaceFontSheet CreateFontSheetMonoChromeDynamic(AssetFontSheet asset, params Argb32[] monoChromeDynamicFontColors)
        {
            if (asset.FontType != FontTypes.MonochromeDynamic)
            {
                throw new Exception("The asset is not of type MonoChromeDynamic!");
            }

            if(monoChromeDynamicFontColors.Length == 0)
            {
                monoChromeDynamicFontColors = asset.BankColors;
            }

            var font = new SurfaceFontSheet();

            var address = ReserveEmptySprite(font, asset.Name, asset.Width, asset.Height * monoChromeDynamicFontColors.Length);            
            var addressDestination = address;

            for (int bankNumber = 0; bankNumber < monoChromeDynamicFontColors.Length; bankNumber++)
            {
                var color = monoChromeDynamicFontColors[bankNumber];

                for (int index = 0; index < asset.Pixels.Length; index++)
                {
                    var pixel = asset.Pixels[index];

                    if (pixel.IsTransparent)
                    {
                        this.Pixels[addressDestination] = pixel;
                    }
                    else
                    {
                        this.Pixels[addressDestination] = color;
                    }

                    addressDestination++;
                }
            }

            font.Initialize(this.Pixels, address, asset.FontType, asset.Width, asset.Height * monoChromeDynamicFontColors.Length, asset.TileWidth, asset.TileHeight, asset.MapHeightBank, monoChromeDynamicFontColors.Length);

            return font;
        }

        public SurfaceFontSheet CreateFontSheetPolyChromeStatic(AssetFontSheet asset)
        {
            if (asset.FontType != FontTypes.PolychromeStatic)
            {
                throw new Exception("The asset is not of type PolyChromeStatic!");
            }

            var font = new SurfaceFontSheet();

            var address = ReserveEmptySprite(font, asset.Name, asset.Width, asset.Height);

            font.Initialize(this.Pixels, address, asset.FontType, asset.Width, asset.Height, asset.TileWidth, asset.TileHeight, asset.MapHeightBank, asset.BankCount);
            // Copier les pixels de Asset et les mettre à l'address reservé
            this.Copy(asset.Pixels, 0, 0, font.AddressBank0);

            return font;
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
