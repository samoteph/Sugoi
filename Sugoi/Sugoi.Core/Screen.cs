using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class Screen : SurfaceSprite
    {
        internal void Start(int width, int height)
        {
            this.Create(width, height);
        }

        internal void Stop()
        {
        }

        /// <summary>
        /// Obtenir la position relative à l'ecran du device (qui affiche la Sugoi / SlateView pour UWP par exemple)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Point GetScreenPoint(int xDevice, int yDevice, uint deviceWidth, uint deviceHeight)
        {
            if(deviceWidth == 0 ||  deviceHeight == 0)
            {
                return Point.Empty;
            }

            var xScreen = (int)((xDevice * this.Width) / deviceWidth);
            var yScreen = (int)((yDevice * this.Height) / deviceHeight);

            return new Point(xScreen, yScreen);
        }
    }
}
