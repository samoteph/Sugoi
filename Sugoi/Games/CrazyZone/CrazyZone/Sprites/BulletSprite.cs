using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class BulletSprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private int framePath = 0;

        private int originalX = 0;
        private int originalY = 0;

        DiagonalPath path = new DiagonalPath();

        public BulletSprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;

            this.page = page;
            tiles = AssetStore.Tiles;

            this.ScrollWidth = page.ScrollWidth;

            this.Width = 8;
            this.Height = 8;

            this.InitializeCollision(3);

            return this;
        }

        public override void Initialize()
        {
            framePath = 0;
        }

        public override string TypeName
        {
            get
            {
                return nameof(BulletSprite);
            }
        }

        /// <summary>
        /// Naissance du bébé
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Fire(int x, int y)
        {
            this.IsAlive = true;

            this.X = x;
            this.Y = y;

            this.originalY = y;
            this.originalX = X;

            var opa = page.Opa;

            int directionY;

            if(y > opa.Y + 20)
            {
                // le bulle est au dessous
                directionY = -1;
            }
            else if(y < opa.Y - 20)
            {
                // le bullet est au dessus
                directionY = 1;
            }
            else
            {
                directionY = 0;
            }

            path.Initialize(300, -1, directionY, 300);
        }

        public override void Collide(ISprite sprite)
        {
            this.IsAlive = false;
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            if (framePath <= path.MaximumFrame)
            {
                path.GetPosition(framePath, out var offsetX, out var offsetY);

                X = originalX + offsetX;
                Y = originalY + offsetY;

                framePath++;
            }
            else
            {
                this.IsAlive = false;
            }

            if (Y < screen.BoundsClipped.Top - this.Height)
            {
                this.IsAlive = false;
            }
            else if (Y > screen.BoundsClipped.Bottom + this.Height)
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

            screen.DrawTile(tiles, 106, XScrolled, YScrolled);
        }
    }
}
