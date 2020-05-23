using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class FlySprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private bool isHorizontalFlipped;

        private Animator flyAnimator;
        private int flyIndex = 0;
        private int frameFlyAnimation = 0;
        private double framePath = 0;

        private int frameBullet;

        private int originalX = 0;
        private int originalY = 0;

        static GroupPath path = new GroupPath();
        static int frameThresold1;

        static FlySprite()
        {
            path.AddPath(new HorizontalPath().Initialize(150, 1, 75));

            frameThresold1 = path.MaximumFrame + (75 / 2);

            path.AddPath(new EllipticalPath().Initialize(180, 180, 100, 100, 1, 1, 75));

            path.AddPath(new VerticalPath().Initialize(200, 1, 100));
        }

        public FlySprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;

            this.page = page;
            tiles = AssetStore.Tiles;
            flyAnimator = AssetStore.CreateFlyAnimation();
            flyIndex = 0;

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
            isHorizontalFlipped = true;
        }

        public override string TypeName
        {
            get
            {
                return nameof(FlySprite);
            }
        }

        /// <summary>
        /// Naissance du bébé
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Born(int offsetX, int y)
        {
            this.IsAlive = true;

            this.X = (int)page.ScrollX + this.machine.Screen.BoundsClipped.X - Width - offsetX;
            this.Y = y;

            this.originalY = y;
            this.originalX = X;

            this.flyAnimator.Start();
        }

        public override void Collide(ISprite sprite)
        {
            this.IsAlive = false;

            this.page.Kabooms.GetFreeSprite()
                .Explode(this.X + 8, this.Y + 8);

            this.page.AddHitSmallMonster(this.X, this.Y);
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            flyAnimator.Update();

            if (framePath <= path.MaximumFrame)
            {
                path.GetPosition((int)framePath, out var offsetX, out var offsetY);

                X = originalX + offsetX;
                Y = originalY + offsetY;

                framePath+=0.5;

                if(framePath < frameThresold1 )
                {
                    isHorizontalFlipped = true;
                }
                else
                {
                    isHorizontalFlipped = false;
                }
            }
            else
            {
                this.IsAlive = false;
            }

            if (Y < screen.BoundsClipped.Top - this.Height)
            {
                this.IsAlive = false;
            }

            // XScrolled est calculé ici
            base.Updated();

            if (frameBullet > 60 * 2)
            {
                frameBullet = 0;
                page.Bullets.GetFreeSprite().Fire(X, Y);
            }
            else
            {
                frameBullet++;
            }
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            flyAnimator.Draw(screen, XScrolled, YScrolled, isHorizontalFlipped, false) ;

            this.DrawCollisionBox(screen);
        }
    }
}
