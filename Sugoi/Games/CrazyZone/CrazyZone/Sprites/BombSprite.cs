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
        
        private int originalX = 0;
        private int originalY = 0;

        private int frameExploding = 0;

        private int easingFrame;
        private EasingPath path = new EasingPath();

        public BombSprite Create(Machine machine, int scrollWidth)
        {
            this.machine = machine;

            tiles = AssetStore.Tiles;

            isHorizontalFlipped = true;

            this.ScrollWidth = scrollWidth;

            Initialize();

            return this;
        }

        public override bool CanCollide
        {
            get
            {
                return this.IsExploding == false;
            }
        }

        public int Direction
        {
            get;
            set;
        }

        public bool IsExploding
        {
            get;
            private set;
        }

        public bool IsFiring
        {
            get;
            private set;
        }

        public override string TypeName
        {
            get
            {
                return nameof(BombSprite);
            }
        }

        public override void Collide(ISprite collider)
        {
            if (collider.TypeName == nameof(MotherSprite))
            {
                this.IsExploding = true;
            }
            else
            {
                this.IsAlive = false;
            }
        }

        public void Fire(int x, int y, int direction)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            if (IsFiring == true) return;

            IsFiring = true;

            this.Direction = direction;
            this.X = x;
            this.Y = y;

            originalX = X;
            originalY = Y;

            var width = 65 + machine.GetRandomInteger(-5, 5);
            path.Initialize(EasingFunctions.QuadraticEaseOut, EasingFunctions.CircularEaseIn, width, machine.Screen.BoundsClipped.Height + Height, 1,1, 60);
        }

        public override void Initialize()
        {
            this.IsAlive = true;
            this.IsExploding = false;
            this.frameExploding = 0;
            this.IsFiring = false;

            this.Damage = 10;

            this.Width = 8;
            this.Height = 8;

            this.easingFrame = 0;

            this.InitializeCollision(0);
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            if (this.IsFiring)
            {
                if (this.IsExploding == false)
                {
                    // retournement
                    isHorizontalFlipped = Direction == -1 ? false : true;

                    path.GetPosition(easingFrame, out var easingX, out var easingY);

                    if (easingFrame <= path.MaximumFrame)
                    {
                        easingFrame++;

                        X = originalX + easingX * Direction;
                        Y = originalY + easingY;
                    }

                    if (Y > machine.Screen.BoundsClipped.Bottom + Height)
                    {
                        IsAlive = false;
                        this.IsFiring = false;
                    }
                }
                else
                {
                    frameExploding++;

                    if (frameExploding > 5)
                    {
                        this.IsAlive = false;
                        this.IsFiring = false;
                    }
                }
            }

            base.Updated();
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            if (this.IsFiring)
            {
                var screen = this.machine.Screen;

                if (this.IsExploding == false)
                {
                    // - 4 c'est pour centrer la bombe
                    screen.DrawTile(tiles, 96, XScrolled - 4, YScrolled - 4, isHorizontalFlipped, false);
                }
                else
                {
                    // explosion
                    screen.DrawTile(tiles, 204, XScrolled - 4, YScrolled - 4, isHorizontalFlipped, false);
                }
            }
        }
    }
}
