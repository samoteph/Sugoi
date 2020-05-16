using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class BabySprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private bool isHorizontalFlipped;

        private Map[] walkMaps;
        private int walkIndex = 0;
        private int frameWalkAnimation = 0;

        public BabySprite Create(Machine machine, PlayPage page, int x, int y)
        {
            this.machine = machine;

            this.IsAlive = true;

            this.page = page;
            tiles = AssetStore.Tiles;
            walkMaps = AssetStore.BabyMaps;
            walkIndex = 0;

            isHorizontalFlipped = true;

            this.ScrollWidth = page.ScrollWidth;
            this.X = x;
            this.Y = y;

            Initialize();

            return this;
        }

        public override void Initialize()
        {
            this.Width = 16;
            this.Height = 16;

            this.InitializeCollision(3);
        }

        public override void Collide(ISprite sprite)
        {
            this.IsAlive = false;

            this.page.Kabooms.GetSprite()
                .Create(this.machine, this.page)
                .Explode(this.X + 8, this.Y + 8);
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            if (frameWalkAnimation > 10)
            {
                frameWalkAnimation = 0;

                walkIndex++;
                walkIndex = walkIndex % walkMaps.Length;
            }
            else
            {
                frameWalkAnimation++;
            }

            Y++;

            if(Y > screen.BoundsClipped.Bottom + this.Height)
            {
                this.IsAlive = false;
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

            screen.DrawSpriteMap(walkMaps[walkIndex], XScrolled, YScrolled);
        }
    }
}
