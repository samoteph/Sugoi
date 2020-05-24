using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public interface IScrollView
    {
        float ScrollX
        {
            get;
        }

        float ScrollY
        {
            get;
        }

        int ScrollWidth
        {
            get;
        }

        int ScrollHeight
        {
            get;
        }    
    }
}
