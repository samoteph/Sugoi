using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class DuckSprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private Map[] walkMaps;
        private int walkIndex = 0;
        private int frameWalkAnimation = 0;
        private double framePath = 0;

        private int frameBullet;

        private int originalX = 0;
        private int originalY = 0;

        static GroupPath path = new GroupPath();

        static DuckSprite()
        {
            path.AddPath(new HorizontalPath().Initialize(100, -1, 100));

            path.AddPath(new EllipticalPath().Initialize(0, 180, 25, 25, 1, 1, 25));
            path.AddPath(new EllipticalPath().Initialize(0, 180, 25, 25, 1, -1, 25));

            path.AddPath(new HorizontalPath().Initialize(100, -1, 100));

            path.AddPath(new EllipticalPath().Initialize(0, 180, 25, 25, 1, 1, 25));
            path.AddPath(new EllipticalPath().Initialize(0, 180, 25, 25, 1, -1, 25));

            path.AddPath(new HorizontalPath().Initialize(100, -1, 100));
            path.AddPath(new EllipticalPath().Initialize(0, 180, 25, 25, 1, 1, 25));

            path.AddPath(new VerticalPath().Initialize(300, -1, 300));
        }

        public DuckSprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;

            this.page = page;
            tiles = AssetStore.Tiles;
            walkMaps = AssetStore.DuckMaps;
            walkIndex = 0;

            this.ScrollWidth = page.ScrollWidth;

            this.Width = 16;
            this.Height = 16;

            this.InitializeCollision(3);

            return this;
        }

        public override void Initialize()
        {
            framePath = 0;
            frameBullet = 0;
        }

        public override string TypeName
        {
            get
            {
                return nameof(DuckSprite);
            }
        }

        /// <summary>
        /// Naissance du bébé
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Born(int y)
        {
            this.IsAlive = true;

            this.X = (int)page.ScrollX + this.machine.Screen.BoundsClipped.Width;
            this.Y = y;

            this.originalY = y;
            this.originalX = X;
        }

        public override void Collide(ISprite sprite)
        {
            this.IsAlive = false;

            this.page.Kabooms.GetFreeSprite()
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

            if (framePath <= path.MaximumFrame)
            {
                path.GetPosition((int)framePath, out var offsetX, out var offsetY);

                X = originalX + offsetX;
                Y = originalY + offsetY;

                framePath+=0.5;
            }
            else
            {
                this.IsAlive = false;
            }

            if (Y < screen.BoundsClipped.Top - this.Height)
            {
                this.IsAlive = false;
            }

            if (frameBullet > 60 * 3)
            {
                frameBullet = 0;
                page.Bullets.GetFreeSprite().Fire(X, Y);
            }
            else
            {
                frameBullet++;
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
