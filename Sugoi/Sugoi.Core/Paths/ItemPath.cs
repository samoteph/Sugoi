using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public abstract class ItemPath : IItemPath
    {
        public int Width
        {
            get;
            protected set;
        }

        public int Height
        {
            get;
            protected set;
        }

        public int DirectionX
        {
            get;
            protected set;
        } = 1;

        public int DirectionY
        {
            get;
            protected set;
        } = 1;

        public int MaximumFrame
        {
            get;
            protected set;
        }

        public abstract void GetPosition(int currentFrame, out int offsetX, out int offsetY);
    }
}
