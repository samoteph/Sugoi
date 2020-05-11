using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Pages
{
    public interface IPage
    {
        void Initialize();
        void Updating(); 
        void Updated();
        void Draw(int frameExecuted);
    }
}
