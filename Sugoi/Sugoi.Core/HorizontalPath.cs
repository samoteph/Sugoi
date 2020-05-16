using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class HorizontalPath : ItemPath
    {
        public HorizontalPath Initialize(int width, int directionX, int maximumFrame)
        {
            this.Height = 1;
            this.Width = width;
            this.MaximumFrame = maximumFrame;

            this.DirectionX = directionX;

            return this;
        }

        public override void GetPosition(int currentFrame, out int offsetX, out int offsetY)
        {
            offsetY = 0;
            double position;

            if (currentFrame > MaximumFrame)
            {
                position = 1;
            }
            else if (currentFrame <= 0)
            {
                position = 0;
            }
            else
            {
                position = (double)currentFrame / (double)MaximumFrame;
            }

            offsetX= (int)(position * (double)Width) * DirectionX;
        }
    }
}
