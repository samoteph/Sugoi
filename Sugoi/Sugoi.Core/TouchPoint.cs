using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public struct TouchPoint
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id"></param>

        public TouchPoint(uint id, int x, int y)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
        }

        public uint Id
        {
            get;
            private set;
        }

        public int X
        {
            get;
           private set;
        }

        public int Y
        {
            get;
            private set;
        }
    }
}
