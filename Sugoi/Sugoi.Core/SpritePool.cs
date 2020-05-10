using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SpritePool
    {
        public Sprite[] sprites;

        public void Initialize(int size)
        {
            this.sprites = new Sprite[size];
        }
    }

    public struct Sprite
    {
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public SugoiInitializedHandler Initialize;
        public SugoiFrameUpdatedHandler UpdateCallback;
        public SugoiFrameDrawnHandler DrawCallback;
    }
}
