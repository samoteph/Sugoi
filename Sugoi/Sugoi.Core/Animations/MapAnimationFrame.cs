using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class MapAnimationFrame : AnimationFrame
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>

        public MapAnimationFrame(Map map, int frameCount, bool isHorizontalFlipped = false, bool isVerticalFlipped = false, int relativeX = 0, int relativeY = 0) : base(frameCount, isHorizontalFlipped, isVerticalFlipped, relativeX, relativeY)
        {
            this.Map = map;
        }

        public Map Map
        {
            get;
            private set;
        }

        public override int Width => this.Map.Width;
        public override int Height => this.Map.Height;

        /// <summary>
        /// Draw
        /// </summary>

        public override void Draw(SurfaceSprite screen, int x, int y)
        {
            screen.DrawSpriteMap(Map, x + RelativeX, y + RelativeY, IsHorizontalFlipped, IsVerticalFlipped);
        }

        public override void Draw(SurfaceSprite screen, int x, int y, bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            // si isHorizontalFlipped alors on inverse la valeur IsHorizontalFlipped stocké dans la frame
            isHorizontalFlipped = isHorizontalFlipped == true ? !IsHorizontalFlipped : IsHorizontalFlipped;
            isVerticalFlipped = isVerticalFlipped == true ? !IsVerticalFlipped : IsVerticalFlipped;

            screen.DrawSpriteMap(Map, x + RelativeX, y + RelativeY, isHorizontalFlipped, isVerticalFlipped);
        }
    }
}
