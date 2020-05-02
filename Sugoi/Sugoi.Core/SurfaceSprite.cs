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

        private Rectangle BoundsClipped
        {
            get;
            set;
        }

        private void SetClip(Rectangle? value)
        {
            if (value == null)
            {
                this.BoundsClipped = Bounds;
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

                this.BoundsClipped = rectClipped;

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

            this.HaveClip = (clip == null || clip.Value.IsEmpty == true || clip == Bounds) == false;
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

        public bool HaveClip
        {
            get;
            private set;
        }

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

            this.Bounds = new Rectangle(0, 0, width, height);

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

            if(this.BoundsClipped.Width <= 0 || this.BoundsClipped.Height <= 0)
            {
                return;
            }

            SurfaceRectangle rectScreen = new SurfaceRectangle();

            int destinationAddress = 0;
            int sourceAddress = 0;

            // pas de clipping: La destination ne bouge pas (sauf exception des xscreen/yScreen negatif et la source bouge
            if (HaveClip == false)
            {
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

                // on genere le rectangle affichable de la source. Il prend en compte son clipping à lui (souvent inutile) mais pas celui de l'ecran.
                var rectSprite = surface.GetVisibleRectangle(xSprite, ySprite, widthSprite, heightSprite, false, false);

                if (rectSprite.isVisible == false)
                {
                    return;
                }

                // on genere le rectangle affichable de l'ecran en prenant la taille de celui du rectangle de la source
                rectScreen = this.GetVisibleRectangle(xScreen, yScreen, rectSprite.width, rectSprite.height, isHorizontalFlipped, isVerticalFlipped);

                if (rectScreen.isVisible == false)
                {
                    return;
                }

                sourceAddress = rectSprite.address;

                // On sort de l'ecran mais le rectangle est visible donc on recalcule l'adresse de départ en ajoutant les lignes perdues
                if (yScreen < 0)
                {
                    if (isVerticalFlipped == false)
                    {
                        sourceAddress += (surface.Width * -yScreen);
                    }
                    else
                    {
                        if (heightSprite > BoundsClipped.Height) // cas ou la taille affichable est plus petite que le sprite et que l'on est flipH
                        {
                            var sourceOffsetY = ((yScreen + heightSprite) - BoundsClipped.Height);

                            if (sourceOffsetY >= 0)
                            {
                                sourceAddress += surface.Width * sourceOffsetY;
                            }
                            else
                            {

                            }
                        }
                        // on avance la source du nombre de pixel caché par le bord de l'ecran
                    }
                }
                else if (yScreen + heightSprite >= BoundsClipped.Height)
                {
                    if (isVerticalFlipped == true)
                    {
                        var sourceOffsetY = ((yScreen + heightSprite) - BoundsClipped.Height);
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
                        if (widthSprite > BoundsClipped.Width) // cas ou la taille affichable est plus petite que le sprite et que l'on est flipH
                        {
                            var sourceOffsetX = ((xScreen + widthSprite) - BoundsClipped.Width);

                            if (sourceOffsetX >= 0)
                            {
                                sourceAddress += sourceOffsetX;
                            }
                        }
                        // on avance la source du nombre de pixel caché par le bord de l'ecran
                    }
                    // en mode flip Horizontal on a pas besoin d'avancer car la source address et la derniere pixel de la ligne
                }
                else if (xScreen + widthSprite >= this.BoundsClipped.Width)
                {
                    if (isHorizontalFlipped == true)
                    {
                        var sourceOffsetX = ((xScreen + widthSprite) - BoundsClipped.Width);
                        sourceAddress += sourceOffsetX; // on avance la source du nombre de pixel caché par le bord de l'ecran
                    }
                }

                destinationAddress = rectScreen.address;
            }

            // ** Gestion du clipping : ici le mouvement se fait du coté de la source et la destination (écran) reste fixe **
            else
            {
                // on genere le rectangle affichable de la source (il prend en compte son clipping à lui (souvent inutile) et le clipping de l'écran mais qui bouge en xscreen et yscreen )

                var sourceClipped = new Rectangle(this.BoundsClipped.X - xScreen + xSprite, this.BoundsClipped.Y - yScreen + ySprite, this.BoundsClipped.Width, this.BoundsClipped.Height);
                var rectSprite = surface.GetVisibleRectangle(xSprite, ySprite, widthSprite, heightSprite, false, false, sourceClipped);

                if (rectSprite.isVisible == false)
                {
                    return;
                }

                // on genere le rectangle affichable de l'ecran en prenant la taille de celui du rectangle de la source
                rectScreen = this.GetVisibleRectangle(this.BoundsClipped.X, this.BoundsClipped.Y, rectSprite.width, rectSprite.height, isHorizontalFlipped, isVerticalFlipped);

                if (rectScreen.isVisible == false)
                {
                    return;
                }

                sourceAddress = rectSprite.address;
                destinationAddress = rectScreen.address;

                // compensation sur la destinationAddress
                if (isHorizontalFlipped == false)
                {
                    //if (rectSprite.x == 0)
                    //{
                        if (xScreen + widthSprite > BoundsClipped.Right)
                        {
                            destinationAddress += (this.BoundsClipped.Width - rectSprite.width);
                        }
                        else if (xScreen > BoundsClipped.X)
                        {
                            destinationAddress += xScreen - BoundsClipped.X;
                        }
                    //}
                }
                else
                {
                    if (rectSprite.x == xSprite)
                    {
                        if (xScreen + widthSprite > BoundsClipped.Right)
                        {
                            sourceAddress += ((xScreen + widthSprite) - this.BoundsClipped.Right);
                            destinationAddress += (this.BoundsClipped.Width - rectSprite.width);
                        }
                        else if (xScreen > BoundsClipped.X)
                        {
                            destinationAddress += xScreen - BoundsClipped.X;
                        }
                    }
                    else
                    {
                        if (xScreen < BoundsClipped.X)
                        {
                            if (xScreen + widthSprite > BoundsClipped.Right)
                            {
                                // fn de ligne - distance entre xScreen et le x de la zone de clipping 
                                sourceAddress += ((xScreen + widthSprite) - this.BoundsClipped.Right) - (BoundsClipped.X - xScreen);
                            }
                            else
                            {
                                sourceAddress += -rectSprite.x + xSprite;
                            }
                        }
                    }
                }

                // Vertical

                if (isVerticalFlipped == false)
                {
                    //if (rectSprite.y == 0)
                    //{
                        if (yScreen + heightSprite > BoundsClipped.Bottom)
                        {
                            destinationAddress += (this.BoundsClipped.Height - rectSprite.height) * this.Width;
                        }
                        else if (yScreen > BoundsClipped.Y)
                        {
                            destinationAddress += (yScreen - BoundsClipped.Y) * this.Width;
                        }
                    //}
                }
                else
                {
                    if (rectSprite.y == ySprite)
                    {
                        if (yScreen + heightSprite > BoundsClipped.Bottom)
                        {
                            sourceAddress += ((yScreen + heightSprite) - this.BoundsClipped.Bottom) * surface.Width;
                            destinationAddress += (this.BoundsClipped.Height - rectSprite.height) * this.Width;
                        }
                        else if (yScreen > BoundsClipped.Y)
                        {
                            destinationAddress += (yScreen - BoundsClipped.Y) * this.Width;
                        }
                    }
                    else
                    {
                        if (yScreen < BoundsClipped.Y)
                        {
                            if (yScreen + heightSprite > BoundsClipped.Bottom)
                            {
                                // fn de ligne - distance entre xScreen et le x de la zone de clipping 
                                sourceAddress += (((yScreen + heightSprite) - this.BoundsClipped.Bottom) - (BoundsClipped.Y - yScreen)) * surface.Width;
                            }
                            else
                            {
                                sourceAddress += (-rectSprite.y + ySprite) * surface.Width;
                            }
                        }
                    }
                }
            }

            // Affichage 

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
            catch (Exception ex)
            {

            }

            //if (HaveClip)
            //{
            //    // test : En Green affiche la source qui sera envoyé à destination
            //    DrawRectangle(rectScreen.x, rectScreen.y, rectScreen.width, rectScreen.height, Argb32.Green, false);
            //}
        }

        /// <summary>
        /// Reset les valeurs de 
        /// </summary>

        public void ClearClip()
        {
            this.Clip = null;
        }

        public void DrawTile(SurfaceTileSheet surface, int tileNumber, int xScreen, int yScreen, bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            var rows = tileNumber / surface.TileSheetHeight;
            var columns = tileNumber - (rows * surface.TileSheetWidth);

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
            if (HaveClip == false)
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
                this.DrawRectangle(BoundsClipped.X, BoundsClipped.Y, BoundsClipped.Width, BoundsClipped.Height, color);
            }
        }

        public void SetPixel(int x, int y, Argb32 color)
        {
            try
            {
                var position = this.GetPosition(x, y);
                var pixels = this.Pixels;

                if (position > -1)
                {
                    pixels[this.Address + position] = color;
                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Affichage d'un rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>

        public void DrawRectangle(int x, int y, int width, int height, Argb32 color, bool isFilled = true)
        {
            if (isFilled == false)
            {
                var right = x + width - 1;
                var bottom = y + height -1;

                this.DrawHorizontalLine(x, y, right, color);
                this.DrawVerticalLine(x, y, bottom, color);
                this.DrawVerticalLine(right, y, bottom, color);
                this.DrawHorizontalLine(x, bottom, right, color);
            }
            else
            {
                var rect = this.GetVisibleRectangle(x, y, width, height, false, false);

                if (rect.isVisible == false)
                {
                    return;
                }

                var addressStart = rect.address;
                var stride = rect.stride;
                var heightClipped = rect.height;
                var widthClipped = rect.width;
                var pixels = this.Pixels;

                for (int iy = 0; iy < heightClipped; iy++)
                {
                    for (int ix = 0; ix < widthClipped; ix++)
                    {
                        pixels[addressStart] = color;
                        addressStart++;
                    }

                    addressStart += stride;
                }
            }
        }

        public void DrawHorizontalLine(int x1, int y, int x2, Argb32 color)
        {
            if(x1 > x2)
            {
                var temp = x1;
                x2 = x1;
                x1 = temp;
            }

            var width = (x2 - x1) + 1;

            var rect = this.GetVisibleRectangle(x1, y, width, 1, false, false);

            if(rect.isVisible == false)
            {
                return;
            }

            var widthClipped = rect.width;
            var addressStart = rect.address;
            var pixels = this.Pixels;

            for (int ix = 0; ix < widthClipped; ix++)
            {
                pixels[addressStart] = color;
                addressStart++;
            }
        }

        public void DrawVerticalLine(int x, int y1, int y2, Argb32 color)
        {
            if (y1 > y2)
            {
                var temp = y1;
                y2 = y1;
                y1 = temp;
            }

            var height = (y2 - y1) + 1;

            var rect = this.GetVisibleRectangle(x, y1, 1, height, false, false);

            if (rect.isVisible == false)
            {
                return;
            }

            var heightClipped = rect.height;
            var addressStart = rect.address;
            var pixels = this.Pixels;
            var stride = rect.stride;

            for (int iy = 0; iy < heightClipped; iy++)
            {
                pixels[addressStart] = color;
                addressStart+= stride + 1;
            }
        }

        /// <summary>
        /// Obtenir un rectangle à l'interieur de l'ecran (prend en compte le clipping de la surface + un clipping externe eventuellement)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>

        private SurfaceRectangle GetVisibleRectangle(int x, int y, int width, int height, bool isFlipHorizontal, bool isFlipVertical, Rectangle? externalBoundsClipped = null)
        {
            SurfaceRectangle rectVisibility = new SurfaceRectangle();

            //if (width <= 0 || height <= 0)
            //{
            //    return visibleRect;
            //}

            //if (x < 0 )
            //{
            //    if ((x + width) > 0)
            //    {
            //        width = width + x; // ici x est négatif
            //        x = 0;
            //    }
            //    else
            //    {
            //        return visibleRect;
            //    }
            //}

            //if (y < 0)
            //{
            //    if ((y + height) > 0)
            //    {
            //        height = height + y; // ici y est négatif
            //        y = 0;
            //    }
            //    else
            //    {
            //        return visibleRect;
            //    }
            //}

            ////var widthClipped = Math.Min(width, this.Width - x);
            ////var heightClipped = Math.Min(height, this.Height - y);

            //var widthClipped = Math.Min(width, BoundsClipped.Width - x);
            //var heightClipped = Math.Min(height, BoundsClipped.Height - y);

            int direction;
            int stride;

            var rectRequested = new Rectangle(x, y, width, height); // taille demandé par le client

            rectRequested.Intersect(this.BoundsClipped); // BoundsClipped est deja l'intersection entre clip et l'ecran

            // Rajoute la possibilité d'ajouté un clip externe (par exemple celui d'une destination sur une source)
            if(externalBoundsClipped != null)
            {
                rectRequested.Intersect(externalBoundsClipped.Value);
            }

            // L'interscetion entre la taille demandée et le clipping n'existe pas
            if (rectRequested.Width == 0 || rectRequested.Height == 0)
            {
                return rectVisibility;
            }

            int position = this.GetPosition(rectRequested.X, rectRequested.Y);

            if (isFlipHorizontal == false)
            {
                stride = this.Width - rectRequested.Width;
                direction = 1;
            }
            else
            {
                position += (rectRequested.Width - 1);
                direction = -1;
                stride = this.Width + rectRequested.Width;
            }

            if(isFlipVertical == true)
            {
                position += (this.Width * (rectRequested.Height - 1));

                if (isFlipHorizontal == false)
                {
                    stride = -this.Width - rectRequested.Width;
                }
                else
                {
                    stride = -this.Width + rectRequested.Width;
                }
            }

            if (position == -1)
            {
                return rectVisibility;
            }

            rectVisibility.direction = direction;
            rectVisibility.position = position;
            rectVisibility.address = this.Address + position;
            rectVisibility.stride = stride;
            rectVisibility.width = rectRequested.Width;
            rectVisibility.height = rectRequested.Height;
            rectVisibility.isVisible = true;
            rectVisibility.x = rectRequested.X;
            rectVisibility.y = rectRequested.Y;

            return rectVisibility;
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
            if (x < BoundsClipped.X || x > BoundsClipped.Right)
            {
                return -1;
            }

            if (y < BoundsClipped.Y || y > BoundsClipped.Bottom)
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
        public int x;
        public int y;

        public bool isVisible;
    }
}
