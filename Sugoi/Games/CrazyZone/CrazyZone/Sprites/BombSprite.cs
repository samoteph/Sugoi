using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrazyZone.Sprites
{
    public class BombSprite : Sprite
    {
        private Machine machine;
        private SurfaceTileSheet tiles;

        private bool isHorizontalFlipped;
        
        private double easingCounter = 0;
        private double stepCounter = 0;
        private int originalX = 0;

        public BombSprite Create(Machine machine, int scrollWidth)
        {
            this.machine = machine;

            tiles = AssetStore.Tiles;

            isHorizontalFlipped = true;

            stepCounter = 1d / (double)machine.Screen.BoundsClipped.Height;

            this.ScrollWidth = scrollWidth;

            Initialize();

            return this;
        }

        public int Direction
        {
            get;
            set;
        }

        public override void Collide(ISprite collider)
        {
            this.IsAlive = false;
        }

        public void Fire(int x, int y, int direction)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            this.Direction = direction;
            this.X = x;
            this.Y = y + 4;
            originalX = x;
        }

        public override void Initialize()
        {
            this.IsAlive = true;

            this.Width = 8;
            this.Height = 8;

            this.CreateCollisionBounds(0);
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            // retournement
            isHorizontalFlipped = Direction == -1 ? false : true;

            // on avance de 8 toutes les frames
            easingCounter += stepCounter;
                
            if(easingCounter < 1)
            {
                X = originalX; // originalX + (int)(Easings.CircularEaseOut(easingCounter) * 20d);  
            }
                
            Y++;

            if (Y > machine.Screen.BoundsClipped.Bottom + 8)
            {
                IsAlive = false;
            }

            base.Updated();
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;
            screen.DrawTile(tiles, 96, XScrolled, YScrolled, isHorizontalFlipped, false);

            Debug.WriteLine("X=" + X + " XScrolled=" + XScrolled);
        }
    }
}
