using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class EllipticalPath : ItemPath
    {
        private double rayX;
        private double rayY;

        private double totalRadian;
        private double startRadian;

        public EllipticalPath Initialize(double startAngleDegree, double totalDegree, int width, int height, int directionX, int directionY, int maximumFrame)
        {
            totalRadian = (totalDegree * Math.PI) / 180d;
            startRadian = (startAngleDegree * Math.PI) / 180d;

            this.Width = width;
            this.Height = height;
            this.DirectionX = directionX;
            this.DirectionY = directionY;
            this.MaximumFrame = maximumFrame;

            this.rayX = width / 2;
            this.rayY = height / 2;

            return this;
        }

        public override void GetPosition(int currentFrame, out int offsetX, out int offsetY)
        {
            var rad = (totalRadian * (double)currentFrame / (double)MaximumFrame) + startRadian;

            offsetX = (int)((Math.Cos(rad) * rayX) - rayX) * DirectionX;
            offsetY = (int)((Math.Sin(rad) * rayY)) * DirectionY;
        }
    }
}
