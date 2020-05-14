using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public interface ISprite
    {
        bool IsAlive
        {
            get;
            set;
        }

        int X
        {
            get;
            set;
        }

        int Y
        {
            get;
            set;
        }

        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        int XScrolled
        {
            get;
        }

        int YScrolled
        {
            get;
        }

        int ScrollX
        {
            get;
            set;
        }

        int ScrollY
        {
            get;
            set;
        }

        int ScrollWidth
        {
            get;
            set;
        }

        int ScrollHeight
        {
            get;
            set;
        }

        Rectangle CollisionBounds
        {
            get;
        }

        bool CanCollide
        {
            get;
        }

        void Initialize();

        void Updating();
        void Updated();

        void Draw(int frameExecuted);
        void Collide(ISprite sprite);
    }
}
