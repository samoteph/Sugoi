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
    }
}
