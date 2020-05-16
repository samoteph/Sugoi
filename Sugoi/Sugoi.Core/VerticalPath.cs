using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class VerticalPath : ItemPath
    {
        public VerticalPath Initialize(int height, int directionY, int maximumFrame)
        {
            this.Width = 1;
            this.Height = height;
            this.MaximumFrame = maximumFrame;
            this.DirectionY = directionY;

            return this;
        }

        public override void GetPosition(int currentFrame, out int offsetX, out int offsetY)
        {
            offsetX = 0;
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

            offsetY = (int)(position * (double)Height) * DirectionY;
        }
    }
}
