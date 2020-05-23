using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public abstract class AnimationFrame
    {
        public AnimationFrame(int frameCount, bool isHorizontalFlipped = false, bool isVerticalFlipped = false, int relativeX = 0, int relativeY = 0 )
        {
            this.FrameCount = frameCount;
            this.RelativeX = relativeX;
            this.RelativeY = relativeY;
        }

        public bool IsHorizontalFlipped
        {
            get;
            private set;
        }

        public bool IsVerticalFlipped
        {
            get;
            private set;
        }

        public int RelativeY
        {
            get;
            private set;
        }

        public int RelativeX
        {
            get;
            private set;
        }

        public int FrameCount
        {
            get;
            private set;
        }

        public abstract int Width
        {
            get;
        }

        public abstract int Height
        {
            get;
        }

        public abstract void Draw(SurfaceSprite screen, int x, int y);
        public abstract void Draw(SurfaceSprite screen, int x, int y, bool isHorizontalFlipped, bool isVerticalFlipped);
    }
}
