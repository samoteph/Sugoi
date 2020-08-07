using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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
            get
            {
                return this.BoundsClipped.Width;
            }
        }

        public int HeightClipped
        {
            get
            {
                return this.BoundsClipped.Height;
            }
        }

        public Rectangle Bounds
        {
            get;
            private set;
        }

        public Rectangle BoundsClipped
        {
            get;
            private set;
        }

        private Rectangle? oldClip;

        public void ReverseClip()
        {
            this.SetClip(oldClip);
        }

        public void ResetClip()
        {
            this.SetClip(null);
        }

        public void SetClip(Rectangle? value)
        {
            if (value == null)
            {
                this.BoundsClipped = Bounds;
                oldClip = clip;
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
                    // clip == null et le nouveau clipping fait la taille de la Surface -> on le laisse à null
                    if (rectClipped != Bounds)
                    {
                        oldClip = clip;
                        clip = rectClipped;
                    }
                }
                else
                {
                    oldClip = value;
                    clip = value;
                }
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
                            // Peut-on virer ce if ? et laisser seulement le else
                            // s'il n'arrive jamais dans le point d'arret c'est le cas
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

            //try
            //{
                for (int iy = 0; iy < heightScreen; iy++)
                {
                    for (int ix = 0; ix < widthScreen; ix++)
                    {
                        destinationPixels[destinationAddress].AlphaBlend(sourcePixels[sourceAddress], this.Opacity);

                        // sinon c'est invisible

                        sourceAddress++;
                        destinationAddress += destinationDirection;
                    }

                    destinationAddress += strideDestination;
                    sourceAddress += strideSource;
                }
            //}
            //catch (Exception ex)
            //{

            //}

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
            this.SetClip(null);
        }

        public void DrawTile(SurfaceTileSheet surface, int tileNumber, int xScreen, int yScreen, bool isHorizontalFlipped = false, bool isVerticalFlipped = false)
        {
            var rows = tileNumber / surface.TileSheetWidth;
            var columns = tileNumber - (rows * surface.TileSheetWidth);

            var ySprite = rows * surface.TileHeight;
            var xSprite = columns * surface.TileWidth;

            this.DrawSprite(surface, xScreen, yScreen, isHorizontalFlipped, isVerticalFlipped, xSprite, ySprite, surface.TileWidth, surface.TileHeight);
        }

        public void DrawSpriteMap(Map map, int xScreen, int yScreen)
        {
            this.DrawSpriteMap(map, xScreen, yScreen, false, false, 0, 0);
        }

        private void DrawInfiniteSpriteMap(Map map, int xScreen, int yScreen, bool isHorizontalFlipped, bool isVerticalFlipped, int xMap = 0, int yMap = 0, int widthMap = int.MaxValue, int heightMap = int.MaxValue)
        {
            if (xMap < 0)
            {
                xMap = 0;
            }

            if (yMap < 0)
            {
                yMap = 0;
            }

            if (widthMap == int.MaxValue)
            {
                widthMap = map.MapWidth;
            }

            if (heightMap == int.MaxValue)
            {
                heightMap = map.MapHeight;
            }

            var tileWidth = map.TileSheet.TileWidth;
            var tileHeight = map.TileSheet.TileHeight;

            var xMapClipped = xMap + ((BoundsClipped.X - xScreen) / tileWidth);
            var yMapClipped = yMap + ((BoundsClipped.Y - yScreen) / tileHeight);

            // gestion de la longueur
            int widthMapClipped = BoundsClipped.Width / tileWidth;
            var offsetX = (BoundsClipped.X - xScreen) % tileWidth;

            if (offsetX < 0)
            {
                offsetX = tileWidth + offsetX;
                xMapClipped = map.MapWidth + xMapClipped - 1;
            }

            //Debug.WriteLine("OffsetX=" + offsetX);

            int size = BoundsClipped.Width + offsetX;

            if(BoundsClipped.Width % tileWidth > 0)
            {
                widthMapClipped++;
            }

            if (size % tileWidth > 0)
            {
                widthMapClipped++;            
            }

            // gestion de la hauteur
            int heightMapClipped = BoundsClipped.Height / tileHeight;
            var offsetY = (BoundsClipped.Y - yScreen) % tileHeight;

            if (offsetY < 0)
            {
                offsetY = tileHeight + offsetY;
                yMapClipped = map.MapHeight + yMapClipped - 1;
            }

            size = BoundsClipped.Height + offsetY;

            if (BoundsClipped.Height % tileHeight > 0)
            {
                heightMapClipped++;
            }

            if (size % tileHeight > 0)
            {
                heightMapClipped++;
            }

            // fin du clipping début de l'affichage
            var tileSheet = map.TileSheet;

            // l'offsetX permet le scrolling lorsque xScreen est négatif (mais juste sur les 8 premiers pixels négatifs)
            var xPixel = BoundsClipped.X - offsetX;

            // l'offsetY permet le scrolling lorsque yScreen est négatif (mais juste sur les 8 premiers pixels négatifs)
            var yPixel = BoundsClipped.Y - offsetY;

            // lancement de l'affichage
            var xPixelSart = xPixel;

            var xTile = 0;
            var yTile = 0;

            //Debug.WriteLine(widthMapClipped);

            for (int y = 0; y < heightMapClipped; y++)
            {
                for (int x = 0; x < widthMapClipped; x++)
                {
                    xTile = x + xMapClipped;
                    yTile = y + yMapClipped;

                    if (isHorizontalFlipped == true)
                    {
                        xTile = map.MapWidth - 1 - xTile;
                    }

                    if (isVerticalFlipped == true)
                    {
                        yTile = map.MapHeight - 1 - yTile;
                    }

                    // les tiles sont forcement dans la map
                    xTile = xTile % map.MapWidth;
                    yTile = yTile % map.MapHeight;

                    if(xTile < 0)
                    {
                        xTile = map.MapWidth + xTile;
                    }

                    if (yTile < 0)
                    {
                        yTile = map.MapHeight + yTile;
                    }

                    var tile = map[xTile, yTile];

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
                        this.DrawTile(tileSheet, tile.number, xPixel, yPixel, isHFlip, isVFlip);
                    }

                    xPixel += tileWidth;
                }

                xPixel = xPixelSart;
                yPixel += tileHeight;
            }
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

            var tileWidth = map.TileSheet.TileWidth;
            var tileHeight = map.TileSheet.TileHeight;

            // les flip sont pris en compte plus tard
            var rectScreen = this.GetVisibleRectangle(xScreen, yScreen, map.Width, map.Height, false, false);

            if (rectScreen.isVisible == false)
            {
                return;
            }

            // on recalcule xMap et yMap et la taille selon le clipping

            var xMapClipped = xMap + ((rectScreen.x - xScreen) / tileWidth);
            var yMapClipped = yMap + ((rectScreen.y - yScreen) / tileHeight);

            // gestion de la longueur

            var offsetX = (BoundsClipped.X - xScreen) % tileWidth;
            
            int widthMapClipped;
            int size;

            if (offsetX >= 0)
            {
                size = offsetX + rectScreen.width;
            }
            else
            {
                size = rectScreen.width;
            }

            widthMapClipped = size / tileWidth;

            if(size % tileWidth > 0)
            {
                widthMapClipped++;
            }

            if (xMapClipped + widthMapClipped > map.MapWidth)
            {
                widthMapClipped = map.MapWidth - xMapClipped;
            }

            // gestion de la hauteur

            var offsetY = (BoundsClipped.Y - yScreen) % tileHeight;

            int heightMapClipped;

            if (offsetY >= 0)
            {
                size = offsetY + rectScreen.height;
            }
            else
            {
                size = rectScreen.height;
            }

            heightMapClipped = size / tileHeight;

            if (size % tileHeight > 0)
            {
                heightMapClipped++;
            }

            if (yMapClipped + heightMapClipped > map.MapHeight)
            {
                heightMapClipped = map.MapHeight - yMapClipped;
            }

            // fin du clipping début de l'affichage
            var tileSheet = map.TileSheet;

            // l'offsetX permet le scrolling lorsque xScreen est négatif (mais juste sur les 8 premiers pixels négatifs)
            var xPixel = rectScreen.x;

            if (xScreen < BoundsClipped.X)
            {
                xPixel -= offsetX;
            }

            // l'offsetY permet le scrolling lorsque yScreen est négatif (mais juste sur les 8 premiers pixels négatifs)
            var yPixel = rectScreen.y;

            if (yScreen < BoundsClipped.Y)
            {
                yPixel -= offsetY;
            }

            // lancement de l'affichage
            var xPixelSart = xPixel;

            var xTile = 0;
            var yTile = 0;

            for (int y = 0; y < heightMapClipped; y++)
            {
                for (int x = 0; x < widthMapClipped; x++)
                {
                    xTile = x + xMapClipped;
                    yTile = y + yMapClipped;

                    if (isHorizontalFlipped == true)
                    {
                        xTile = map.MapWidth - 1 - xTile;
                    }

                    if (isVerticalFlipped == true)
                    {
                        yTile = map.MapHeight - 1 - yTile;
                    }

                    var tile = map[xTile, yTile];

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
                        this.DrawTile(tileSheet, tile.number, xPixel, yPixel, isHFlip, isVFlip);
                    }

                    xPixel += tileWidth;
                }

                xPixel = xPixelSart;
                yPixel += tileHeight;
            }
        }

        /// <summary>
        /// Permet de savoir si l'on atteint un bord ou non
        /// </summary>
        /// <param name="map"></param>
        /// <param name="scrollX"></param>
        /// <param name="scrollY"></param>
        /// <param name="xScreen"></param>
        /// <param name="yScreen"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>

        public bool CanScrollMap(Map map, int scrollX, int scrollY, int xScreen = 0, int yScreen = 0, int width = int.MaxValue, int height = int.MaxValue)
        {
            if (scrollX > 0)
            {
                return false;
            }

            if (scrollY > 0)
            {
                return false;
            }

            if (width == int.MaxValue)
            {
                width = this.Width - xScreen;
            }

            if (height == int.MaxValue)
            {
                height = this.Height - yScreen;
            }

            var currentClip = this.BoundsClipped;
            currentClip.Intersect(new Rectangle(xScreen, yScreen, width, height));

            var tileWidth = map.TileSheet.TileWidth;
            var scrollWidth = (currentClip.Width / tileWidth) * tileWidth;
            var offsetWidth = scrollWidth % tileWidth;

            if (offsetWidth > 0)
            {
                scrollWidth += tileWidth;
            }

            if (scrollX < -(map.Width - scrollWidth))
            {
                return false;
            }

            var tileHeight = map.TileSheet.TileHeight;
            var scrollHeight = (currentClip.Height /tileHeight) * tileHeight;
            var offsetHeight = scrollHeight % tileHeight;

            if(offsetHeight > 0)
            {
                scrollHeight += tileHeight;
            }

            if (scrollY < -(map.Height - scrollHeight))
            {
                return false;
            }

            return true;
        }

        public Font Font
        {
            get;
            set;
        }

        public void DrawScrollMap(Map map, bool isInfinite, int scrollX, int scrollY,  int xScreen = 0, int yScreen = 0, int width = int.MaxValue, int height = int.MaxValue, bool isHorizontalFlip = false, bool isVerticalFlip = false, int xMap = 0, int yMap = 0)
        {
            var clip = this.Clip;

            if(width == int.MaxValue)
            {
                width = map.Width;
            }

            if(height == int.MaxValue)
            {
                height = map.Height;
            }

            var currentClip = this.BoundsClipped;    
            currentClip.Intersect(new Rectangle(xScreen, yScreen, width, height));

            this.SetClip(currentClip);

            if (isInfinite == false)
            {
                this.DrawSpriteMap(map, xScreen + scrollX, yScreen + scrollY, isHorizontalFlip, isVerticalFlip, xMap, yMap, int.MaxValue, int.MaxValue);
            }
            else
            {
                this.DrawInfiniteSpriteMap(map, xScreen + scrollX, yScreen + scrollY, isHorizontalFlip, isVerticalFlip, xMap, yMap, int.MaxValue, int.MaxValue);
            }

            this.SetClip(clip);
        }

        public void DrawText(char glyph, int xScreen = 0, int yScreen = 0, int bank = 0)
        {
            var font = this.Font;

            int tileWidth = font.FontSheet.TileWidth;

            font.FontSheet.SetBank(bank);

            var tileNumber = font.GetTileNumber(glyph);
            this.DrawTile(font.FontSheet, tileNumber, xScreen, yScreen);
        }

        public void DrawText(string text, int xScreen = 0, int yScreen = 0, int bank = 0)
        {
            var font = this.Font;

            if (text == null || text.Length == 0)
            {
                return;
            }

            if(font == null)
            {
                throw new Exception("You must define a font!");
            }

            int tileWidth = font.FontSheet.TileWidth;

            font.FontSheet.SetBank(bank);

            for (int x = 0; x < text.Length; x++)
            {
                var tileNumber = font.GetTileNumber(text[x]);
                this.DrawTile(font.FontSheet, tileNumber, xScreen + x * tileWidth, yScreen);
            }
        }

        private char[] integerString = new char[10];

        /// <summary>
        /// Affichage d'un integer
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="xScreen"></param>
        /// <param name="yScreen"></param>
        /// <param name="bank"></param>

        public void DrawText(int integer, int xScreen = 0, int yScreen = 0, int bank = 0)
        {
            int index = 0;
            var font = this.Font;

            int sign = Math.Sign(integer);
            integer = Math.Abs(integer);

            while (true)
            {
                var mask = ((integer / 10) * 10);

                int digit = integer - mask;

                integerString[index] = (char)('0' + digit);

                index++;

                integer = integer / 10;

                if (integer == 0)
                {
                    break;
                }
            }

            if (sign == -1)
            {
                integerString[index] = '-';
                index++;
            }

            int tileWidth = font.FontSheet.TileWidth;
            int x = 0;

            for(int i=index -1; i >= 0; i--)
            {
                var tileNumber = font.GetTileNumber(integerString[i]);
                this.DrawTile(font.FontSheet, tileNumber, xScreen + x, yScreen);
                x += tileWidth;
            }
        }

        public void Clear()
        {
            this.Clear(Argb32.Black);
        }

        public void Clear(Argb32 color)
        {
            if(Opacity == 0 || color.A == 0)
            {
                return;
            }

            if (HaveClip == false)
            {
                var size = this.Size;
                var pixels = this.Pixels;

                var addressStart = this.Address;
                var addressStop = addressStart + size;

                if (Opacity == 1)
                {
                    for (int i = addressStart; i < addressStop; i++)
                    {
                        pixels[i] = color;
                    }
                }
                else
                {
                    for (int i = addressStart; i < addressStop; i++)
                    {
                        pixels[i].AlphaBlend(color, Opacity);
                    }
                }
            }
            else
            {
                this.DrawRectangle(BoundsClipped.X, BoundsClipped.Y, BoundsClipped.Width, BoundsClipped.Height, color);
            }
        }

        /// <summary>
        /// Opacity
        /// </summary>

        public double Opacity
        {
            get;
            set;
        } = 1.0;

        public void SetPixel(int x, int y, Argb32 color)
        {
            var position = this.GetPosition(x, y);
            var pixels = this.Pixels;

            if (position > -1)
            {
                pixels[this.Address + position].AlphaBlend(color, this.Opacity);
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
            if(Opacity == 0 || color.A == 0)
            {
                return;
            }

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
                        pixels[addressStart].AlphaBlend(color, this.Opacity);
                        addressStart++;
                    }

                    addressStart += stride;
                }
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Argb32 color)
        {
            if(Opacity == 0 || color.A == 0)
            {
                return;
            }

            if(y1 == y2)
            {
                this.DrawHorizontalLine(x1, y1, x2, color);
                return;
            }

            if(x1 == x2)
            {
                this.DrawVerticalLine(x1, y1, y2, color);
                return;
            }

            int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
            int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            while(true)
            {
                this.SetPixel(x1, y1, color);
                if (x1 == x2 && y1 == y2) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x1 += sx; }
                if (e2 < dy) { err += dx; y1 += sy; }
            }
        }

        public void DrawHorizontalLine(int x1, int y, int x2, Argb32 color)
        {
            if(Opacity == 0 || color.A == 0)
            {
                return;
            }

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
                pixels[addressStart].AlphaBlend(color, Opacity);
                addressStart++;
            }
        }

        public void DrawVerticalLine(int x, int y1, int y2, Argb32 color)
        {
            if (Opacity == 0 || color.A == 0)
            {
                return;
            }

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
                pixels[addressStart].AlphaBlend(color, Opacity);
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

        public void DrawAnimator(Animator animator, int x, int y, bool isHorizontalFlipped = false, bool isVerticalFlipped = false)
        {
            animator.Draw(this, x, y, isHorizontalFlipped, isVerticalFlipped);
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
            if (x < BoundsClipped.X || x >= BoundsClipped.Right)
            {
                return -1;
            }

            if (y < BoundsClipped.Y || y >= BoundsClipped.Bottom)
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
