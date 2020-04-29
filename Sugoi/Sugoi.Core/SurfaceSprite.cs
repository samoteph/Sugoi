using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    public class SurfaceSprite : SurfacePixel
    {
        public SurfaceSprite()
        {

        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public int WidthClipped
        {
            get;
            private set;
        }

        public int HeightClipped
        {
            get;
            private set;
        }

        public Rectangle Bounds
        {
            get;
            private set;
        }

        private void SetClip(Rectangle? value)
        {
            if (value == null)
            {
                this.WidthClipped = Width;
                this.HeightClipped = Height;
                clip = value;
            }
            else
            {
                Rectangle rectClipped;

                if (value == null)
                {
                    rectClipped = this.Bounds;
                }
                else
                {
                    rectClipped = value.Value;
                }

                rectClipped.Intersect(new Rectangle(0, 0, Width, Height));

                this.WidthClipped = rectClipped.Width;
                this.HeightClipped = rectClipped.Height;

                if (clip == null)
                {
                    if (rectClipped != Bounds)
                    {
                        clip = rectClipped;
                    }
                }
                // on ne remet pas à null automatiquement
                // C'est à l'utilisateur de se souvenir
                //else
                //{
                //    if (rectClipped == Size)
                //    {
                //        clip = null;
                //    }
                //}
            }
        }

        /// <summary>
        /// Permet de clipper un sprite
        /// </summary>

        public Rectangle? Clip
        {
            get
            {
                return clip;
            }

            set
            {
                this.SetClip(value);
            }
        }

        private Rectangle? clip;

        protected void Create(int width, int height)
        {
            this.Create(width * height);

            this.Width = width;
            this.Height = height;

            this.Bounds = new Rectangle(0, 0, width, height);
            
            this.SetClip(clip);
        }

        public void Initialize(Argb32[] pixels, int address, int width, int height)
        {
            this.Initialize(pixels, address, width * height);
            this.Width = width;
            this.Height = height;
            this.SetClip(clip);
        }

        public void DrawSprite(SurfaceSprite surface, int xScreen, int yScreen)
        {
            this.DrawSprite(surface, xScreen, yScreen, false, false);
        }

        public void DrawSprite(SurfaceSprite surface, int xScreen, int yScreen, bool isHorizontalFlipped, bool isVerticalFlipped, int xSprite = 0, int ySprite = 0, int widthSprite = int.MaxValue, int heightSprite = int.MaxValue )
        {
            if (widthSprite == int.MaxValue)
            {
                widthSprite = surface.Width;
            }

            if (heightSprite == int.MaxValue)
            {
                heightSprite = surface.Height;
            }

            if(this.WidthClipped <= 0 || this.HeightClipped <= 0)
            {
                return;
            }

            // La lecture des pixels commence en négatif (en gros on devrait afficher du transparent)
            // Pour afficher ces pixels qui n'existent pas on compense en poussant la position à l'écran et en diminuant la taille

            if (ySprite < 0)
            {
                yScreen += -ySprite;
                heightSprite += ySprite;
                ySprite = 0;
            }

            if (xSprite < 0)
            {
                xScreen += -xSprite;
                widthSprite += xSprite;
                xSprite = 0;
            }

            // on genere le rectangle affichable de la source
            var rectSprite = surface.GetVisibleRectangle(xSprite, ySprite, widthSprite, heightSprite, false, false);

            if(rectSprite.isVisible == false)
            {
                return;
            }

            // on genere le rectangle affichable de l'ecran en prenant la taille de celui du rectangle de la source
            var rectScreen = this.GetVisibleRectangle(xScreen, yScreen, rectSprite.width, rectSprite.height, isHorizontalFlipped, isVerticalFlipped);

            if (rectScreen.isVisible == false)
            {
                return;
            }

            var sourceAddress = rectSprite.address;

            // On sort de l'ecran mais le rectangle est visible donc on recalcule l'adresse de départ en ajoutant les lignes perdues
            if (yScreen < 0)
            {
                if (isVerticalFlipped == false)
                {
                    sourceAddress += (surface.Width * -yScreen);
                }
                else
                {
                    if (heightSprite > HeightClipped) // cas ou la taille affichable est plus petite que le sprite et que l'on est flipH
                    {
                        var sourceOffsetY = ((yScreen + heightSprite) - this.HeightClipped);

                        if (sourceOffsetY >= 0)
                        {
                            sourceAddress += surface.Width * sourceOffsetY;
                        }
                    }
                    // on avance la source du nombre de pixel caché par le bord de l'ecran
                }
            }
            else if (yScreen + heightSprite >= this.HeightClipped)
            {
                if (isVerticalFlipped == true)
                {
                    var sourceOffsetY = ((yScreen + heightSprite) - this.HeightClipped);
                    sourceAddress += surface.Width * sourceOffsetY; // on avance la source du nombre de pixel caché par le bord de l'ecran
                }
            }

            if (xScreen < 0)
            {
                if (isHorizontalFlipped == false)
                {
                    sourceAddress += -xScreen; // on avance la source du nombre de pixel caché par le bord de l'ecran
                }
                else
                {
                    if (widthSprite > WidthClipped) // cas ou la taille affichable est plus petite que le sprite et que l'on est flipH
                    {
                        var sourceOffsetX = ((xScreen + widthSprite) - this.WidthClipped);

                        if (sourceOffsetX >= 0)
                        {
                            sourceAddress += sourceOffsetX;
                        }
                    }
                    // on avance la source du nombre de pixel caché par le bord de l'ecran
                }
                // en mode flip Horizontal on a pas besoin d'avancer car la source address et la derniere pixel de la ligne
            }
            else if(xScreen + widthSprite >= this.WidthClipped)
            {
                if (isHorizontalFlipped == true)
                {
                    var sourceOffsetX = ((xScreen + widthSprite) - this.WidthClipped);
                    sourceAddress += sourceOffsetX; // on avance la source du nombre de pixel caché par le bord de l'ecran
                }
            }

            var destinationAddress = rectScreen.address;
            
            var strideDestination = rectScreen.stride;
            var strideSource = surface.Width - Math.Min(surface.Width, rectScreen.width);

            var heightScreen = rectScreen.height;
            var widthScreen = rectScreen.width;
            
            var destinationPixels = this.Pixels;
            var sourcePixels = surface.Pixels;

            int destinationDirection = rectScreen.direction;

            try
            {
                for (int iy = 0; iy < heightScreen; iy++)
                {
                    for (int ix = 0; ix < widthScreen; ix++)
                    {
                        var pixel = sourcePixels[sourceAddress];

                        if (pixel.A != 0)
                        {
                            destinationPixels[destinationAddress] = pixel;
                        }

                        sourceAddress++;
                        destinationAddress += destinationDirection;
                    }

                    destinationAddress += strideDestination;
                    sourceAddress += strideSource;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void DrawTile(SurfaceTileSheet surface, int tileNumber, int xScreen, int yScreen, bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            var rows = tileNumber / surface.TileSheetHeight;
            var columns = tileNumber - (rows * tileNumber);

            var ySprite = rows * surface.TileHeight;
            var xSprite = columns * surface.TileWidth;

            this.DrawSprite(surface, xScreen, yScreen, isHorizontalFlipped, isVerticalFlipped, xSprite, ySprite, surface.TileWidth, surface.TileHeight);
        }

        public void DrawSpriteMap(Map map, int xScreen, int yScreen)
        {
            this.DrawSpriteMap(map, xScreen, yScreen, false, false, 0, 0);
        }

        public void DrawSpriteMap(Map map, int xScreen, int yScreen, bool isHorizontalFlipped, bool isVerticalFlipped, int xMap = 0, int yMap = 0, int widthMap = int.MaxValue, int heightMap = int.MaxValue)
        {
            if(xMap < 0)
            {
                xMap = 0;
            }

            if(yMap < 0)
            {
                yMap = 0;
            }

            if(widthMap == int.MaxValue)
            {
                widthMap = map.MapWidth;
            }

            if(heightMap == int.MaxValue)
            {
                heightMap = map.MapHeight;
            }

            if(xMap + widthMap > map.MapWidth)
            {
                widthMap = map.MapWidth - xMap;
            }

            if (yMap + heightMap > map.MapHeight)
            {
                heightMap = map.MapHeight - yMap;
            }

            var tileSheet = map.TileSheet;
            
            var tileWidth = tileSheet.TileWidth;
            var tileHeight = tileSheet.TileHeight;

            var xScreenSart = xScreen;

            var xFlip = 0;
            var yFlip = 0;

            for (int y = yMap; y < heightMap; y++)
            {
                for (int x = xMap; x < widthMap; x++)
                {
                    xFlip = x;
                    yFlip = y;

                    if (isHorizontalFlipped == true)
                    {
                        xFlip = widthMap - 1 - x;
                    }

                    if (isVerticalFlipped == true)
                    {
                        yFlip = heightMap - 1 - y;
                    }

                    var tile = map[xFlip, yFlip];

                    var isHFlip = tile.isHorizontalFlipped;
                    var isVFlip = tile.isVerticalFlipped;

                    if (isHorizontalFlipped == true)
                    {
                        isHFlip = !isHFlip;
                    }

                    if (isVerticalFlipped == true)
                    {
                        isVFlip = !isVFlip;
                    }

                    if (tile.hidden == false)
                    {
                        this.DrawTile(tileSheet, tile.number, xScreen, yScreen, isHFlip, isVFlip);
                    }

                    xScreen += tileWidth;
                }

                xScreen = xScreenSart;
                yScreen += tileHeight;
            }
        }

        public void Clear()
        {
            this.Clear(Argb32.Black);
        }

        public void Clear(Argb32 color)
        {
            if (clip == null)
            {
                var size = this.Size;
                var pixels = this.Pixels;

                var addressStart = this.Address;
                var addressStop = addressStart + size;

                for (int i = addressStart; i < addressStop; i++)
                {
                    pixels[i] = color;
                }
            }
            else
            {
                this.DrawRectangle(0, 0, this.WidthClipped, this.HeightClipped, color);
            }
        }

        public void SetPixel(int x, int y, Argb32 color)
        {
            var position = this.GetPosition(x, y);
            var pixels = this.Pixels;

            if (position > -1)
            {
                pixels[this.Address + position] = color;
            }
        }

        /// <summary>
        /// Affichage d'un rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>

        public void DrawRectangle(int x, int y, int width, int height, Argb32 color)
        {
            var rect = this.GetVisibleRectangle(x, y, width, height, false, false);

            if (rect.isVisible == false)
            {
                return;
            }

            var addressStart = rect.address;
            var stride = rect.stride;
            height = rect.height;
            width = rect.width;
            var pixels = this.Pixels;

            for (int iy = 0; iy < height; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    pixels[addressStart] = color;
                    addressStart++;
                }

                addressStart += stride;
            }
        }

        /// <summary>
        /// Obtenir un rectangle à l'interieur de l'ecran (prend en compte le clipping)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>

        private SurfaceRectangle GetVisibleRectangle(int x, int y, int width, int height, bool isFlipHorizontal, bool isFlipVertical)
        {
            SurfaceRectangle visibleRect = new SurfaceRectangle();

            if (width <= 0 || height <= 0)
            {
                return visibleRect;
            }

            if (x < 0 )
            {
                if ((x + width) > 0)
                {
                    width = width + x; // ici x est négatif
                    x = 0;
                }
                else
                {
                    return visibleRect;
                }
            }

            if (y < 0)
            {
                if ((y + height) > 0)
                {
                    height = height + y; // ici y est négatif
                    y = 0;
                }
                else
                {
                    return visibleRect;
                }
            }

            int direction;
            int stride;

            //var widthClipped = Math.Min(width, this.Width - x);
            //var heightClipped = Math.Min(height, this.Height - y);

            var widthClipped = Math.Min(width, this.WidthClipped - x);
            var heightClipped = Math.Min(height, this.HeightClipped - y);

            int position = this.GetPosition(x, y);

            if (isFlipHorizontal == false)
            {
                stride = this.Width - widthClipped;
                direction = 1;
            }
            else
            {
                position += (widthClipped - 1);
                direction = -1;
                stride = this.Width + widthClipped;
            }

            if(isFlipVertical == true)
            {
                position += (this.Width * (heightClipped - 1));

                if (isFlipHorizontal == false)
                {
                    stride = -this.Width - widthClipped;
                }
                else
                {
                    stride = -this.Width + widthClipped;
                }
            }

            if (position == -1)
            {
                return visibleRect;
            }

            visibleRect.direction = direction;
            visibleRect.position = position;
            visibleRect.address = this.Address + position;
            visibleRect.stride = stride;
            visibleRect.width = widthClipped;
            visibleRect.height = heightClipped;
            visibleRect.isVisible = true;

            return visibleRect;
        }

        public Argb32 GetPixel(int x, int y)
        {
            var position = this.GetPosition(x, y);

            if (position > -1)
            {
                return this.Pixels[Address + position];
            }

            // renvoie une pixel completement transparente 
            return Argb32.Empty;
        }

        /// <summary>
        /// Obtenir une position relative à Address de Pixels. Par 10 pour une address à 1000 represente une address reel à 1010
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>

        private int GetPosition(int x, int y)
        {
            if (x < 0 || x > this.WidthClipped)
            {
                return -1;
            }

            if (y < 0 || y > this.HeightClipped)
            {
                return -1;
            }

            var position = y * this.Width + x;

            return position;
        }

        private int GetAddress(int x, int y)
        {
            var position = this.GetPosition(x, y);

            if(position > -1)
            {
                return position + this.Address;
            }

            return -1;
        }
    }

    public struct SurfaceRectangle
    {
        public int address;
        public int position;
        public int stride;
        public int width;
        public int height;
        public int direction;

        public bool isVisible;
    }
}
