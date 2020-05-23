using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SpriteAnimationFrame : AnimationFrame
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>

        public SpriteAnimationFrame(SurfaceSprite sprite, int frameCount, bool isHorizontalFlipped = false, bool isVerticalFlipped = false, int relativeX = 0, int relativeY = 0) : base(frameCount, isHorizontalFlipped, isVerticalFlipped, relativeX, relativeY)
        {
            this.Sprite = sprite;
        }

        public SurfaceSprite Sprite
        {
            get;
            private set;
        }

        public override int Width => this.Sprite.Width;
        public override int Height => this.Sprite.Height;

        /// <summary>
        /// Draw
        /// </summary>

        public override void Draw(SurfaceSprite screen, int x, int y)
        {
            screen.DrawSprite(Sprite, x + RelativeX, y + RelativeY, IsHorizontalFlipped, IsVerticalFlipped);
        }

        /// <summary>
        /// Affichage avec inversion des valeur Flipped contenu dans la Frame
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isHorizontalFlipped"></param>
        /// <param name="isVerticalFlipped"></param>

        public override void Draw(SurfaceSprite screen, int x, int y, bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            // si isHorizontalFlipped alors on inverse la valeur IsHorizontalFlipped stocké dans la frame
            isHorizontalFlipped = isHorizontalFlipped == true ? !IsHorizontalFlipped : IsHorizontalFlipped;
            isVerticalFlipped = isVerticalFlipped == true ? !IsVerticalFlipped : IsVerticalFlipped;

            screen.DrawSprite(Sprite, x + RelativeX, y + RelativeY, isHorizontalFlipped, isVerticalFlipped);
        }
    }
}
