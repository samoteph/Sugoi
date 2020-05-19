using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class DiagonalPath : ItemPath
    {
        public DiagonalPath Initialize(int size, int directionX, int directionY, int maximumFrame)
        {
            this.Height = size;
            this.Width = size;
            this.MaximumFrame = maximumFrame;

            this.DirectionX = Math.Sign(directionX);
            this.DirectionY = Math.Sign(directionY);

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

            offsetX = (int)(position * (double)Width) * DirectionX;
            offsetY = (int)(position * (double)Height) * DirectionY;
        }
    }
}