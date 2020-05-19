using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class EllipticalPath : ItemPath
    {
        private double rayX;
        private double rayY;

        private double startX;
        private double startY;

        private double totalRadian;
        private double startRadian;

        public EllipticalPath Initialize(double startAngleDegree, double totalDegree, int width, int height, int directionX, int directionY, int maximumFrame)
        {
            totalRadian = (totalDegree * Math.PI) / 180d;
            startRadian = (startAngleDegree * Math.PI) / 180d;

            this.Width = width;
            this.Height = height;

            this.DirectionX = Math.Sign(directionX);
            this.DirectionY = Math.Sign(directionY);
            this.MaximumFrame = maximumFrame;

            this.rayX = width / 2;
            this.rayY = height / 2;

            var rad = startRadian;

            this.startX = Math.Cos(rad) * rayX;
            this.startY = Math.Sin(rad) * rayY;

            return this;
        }

        public override void GetPosition(int currentFrame, out int offsetX, out int offsetY)
        {
            var rad = (totalRadian * (double)currentFrame / (double)MaximumFrame) + startRadian;

            offsetX = (int)((Math.Cos(rad) * rayX) - startX) * DirectionX;
            offsetY = (int)((Math.Sin(rad) * rayY) - startY) * DirectionY;
        }
    }
}
