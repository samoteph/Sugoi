using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class TileAnimationFrame : AnimationFrame
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>

        public TileAnimationFrame(SurfaceTileSheet tiles, int tileNumber, int frameCount, bool isHorizontalFlipped = false, bool isVerticalFlipped = false, int relativeX = 0, int relativeY = 0) : base(frameCount, isHorizontalFlipped, isVerticalFlipped, relativeX, relativeY)
        {
            this.TileNumber = tileNumber;
            this.Tiles = tiles;
        }

        public override int Width => this.Tiles.TileWidth;
        public override int Height => this.Tiles.TileHeight;

        public int TileNumber
        {
            get;
            private set;
        }

        public SurfaceTileSheet Tiles
        {
            get;
            private set;
        }

        /// <summary>
        /// Draw
        /// </summary>

        public override void Draw(SurfaceSprite screen, int x, int y)
        {
            screen.DrawTile(Tiles, TileNumber, x + RelativeX, y + RelativeY, IsHorizontalFlipped, IsVerticalFlipped);
        }

        public override void Draw(SurfaceSprite screen, int x, int y, bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            // si isHorizontalFlipped alors on inverse la valeur IsHorizontalFlipped stocké dans la frame
            isHorizontalFlipped = isHorizontalFlipped == true ? !IsHorizontalFlipped : IsHorizontalFlipped;
            isVerticalFlipped = isVerticalFlipped == true ? !IsVerticalFlipped : IsVerticalFlipped;

            screen.DrawTile(Tiles, TileNumber, x + RelativeX, y + RelativeY, isHorizontalFlipped, isVerticalFlipped);
        }
    }
}
