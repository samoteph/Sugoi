using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class TouchPoint
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id"></param>

        public TouchPoint(int id)
        {
            this.Id = id;
        }

        public int Id
        {
            get;
            private set;
        }

        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }
    }
}
