using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public struct AmmoSprite : ISprite
    {
        private Machine machine;
        private SurfaceTileSheet tiles;

        private bool isOpaHorizontalFlipped;

        public AmmoSprite(Machine machine) : this()
        {
            this.machine = machine;

            tiles = AssetStore.Tiles;
 
            isOpaHorizontalFlipped = true;

            Initialize();
        }

        public bool IsAlive
        {
            get;
            set;
        }
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

        public int Direction
        {
            get;
            set;
        }

        public bool IsFiring
        {
            get;
            private set;
        }

        public void Fire(int x, int y, int direction)
        {
            if (IsFiring == true) return;

            IsFiring = true;

            this.Direction = direction;
            this.X = x;
            this.Y = y + 4;
        }

        public void Initialize()
        {
            this.IsAlive = true;
        }

        public void Update()
        {
            if (this.IsFiring == true)
            {
                // retournement
                isOpaHorizontalFlipped = Direction == -1 ? false : true;
                
                // on avance de 8 toutes les frames
                X += Direction * 8;

                var bounds = this.machine.Screen.BoundsClipped;

                if(X > bounds.Right || X < bounds.X )
                {
                    IsFiring = false;
                    IsAlive = false;
                }
            }
        }

        public void Draw(int frameExecuted)
        {
            if (IsFiring == true)
            {
                var screen = this.machine.Screen;
                screen.DrawTile(tiles, 188,  X, Y, isOpaHorizontalFlipped, false);
            }
        }
    }
}
