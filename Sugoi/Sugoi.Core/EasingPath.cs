using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public sealed class EasingPath : ItemPath
    {
        public EasingFunctions EasingX
        {
            get;
            private set;
        }

        public EasingFunctions EasingY
        {
            get;
            private set;
        }

        public EasingPath()
        {
        }

        public void Initialize(EasingFunctions easingX, EasingFunctions easingY, int width, int height, int directionX, int directionY, int maximumFrame)
        {
            this.EasingX = easingX;
            this.EasingY = easingY;
            this.Width = width;
            this.Height = height;
            this.MaximumFrame = maximumFrame;
            this.DirectionX = directionX;
            this.DirectionY = directionY;
        }

        public override void GetPosition(int currentFrame, out int offsetX, out int offsetY)
        {
            // position varie de 0 à 1
            double position;

            if(currentFrame > MaximumFrame)
            {
                position = 1;
            }
            else if( currentFrame <= 0)
            {
                position = 0;
            }
            else
            {
                position = (double)currentFrame / (double)MaximumFrame;
            }

            offsetX = (int)(Easings.Interpolate(position, EasingX) * Width) * DirectionX;
            offsetY = (int)(Easings.Interpolate(position, EasingY) * Height) * DirectionY;
        }

        public double GetEasingFromFrame(EasingFunctions easing, int currentFrame)
        {
            var step = (double)currentFrame / (double)MaximumFrame;
            return Easings.Interpolate(step, easing);
        }
    }
}
