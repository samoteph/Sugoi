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
        private int framePath = 0;

        private int originalX = 0;
        private int originalY = 0;

        private static int directionThresold1;
        private static int directionThresold2;
        private static int directionThresold3;
        private static int directionThresold4;

        static GroupPath path = new GroupPath();

        static BabySprite()
        {
            path.AddPath(new VerticalPath().Initialize(20, 1, 100));

            path.AddPath(new EllipticalPath().Initialize(0, 90, 50, 50, -1, 1, 50));

            path.AddPath(new HorizontalPath().Initialize(100, 1, 100));

            path.AddPath(new EllipticalPath().Initialize(90, -180, 50, 50, 1, 1, 100));

            directionThresold1 = path.MaximumFrame - 50; // correspond à la moitié des frames de l'ellipticalPath

            path.AddPath(new HorizontalPath().Initialize(100, -1, 100));
            path.AddPath(new EllipticalPath().Initialize(90, 180, 50, 50, 1, 1, 100));

            directionThresold2 = path.MaximumFrame - 50; // correspond à la moitié des frames de l'ellipticalPath

            path.AddPath(new HorizontalPath().Initialize(100, 1, 100));
            path.AddPath(new EllipticalPath().Initialize(90, -180, 50, 50, 1, 1, 100));

            directionThresold3 = path.MaximumFrame - 50; // correspond à la moitié des frames de l'ellipticalPath

            path.AddPath(new HorizontalPath().Initialize(100, -1, 100));
            path.AddPath(new EllipticalPath().Initialize(90, 180, 50, 50, 1, 1, 100));
        
            directionThresold4 = path.MaximumFrame - 50; // correspond à la moitié des frames de l'ellipticalPath  
        }

        public BabySprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;

            this.page = page;
            tiles = AssetStore.Tiles;
            walkMaps = AssetStore.BabyMaps;
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

            // pas besoin
            //isHorizontalFlipped = true;
        }

        public override string TypeName
        {
            get
            {
                return nameof(BabySprite);
            }
        }

        /// <summary>
        /// Naissance du bébé
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Born(int x, int y)
        {
            this.IsAlive = true;

            this.X = x;
            this.Y = y;

            this.originalY = y;
            this.originalX = x;
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
                path.GetPosition(framePath, out var offsetX, out var offsetY);

                X = originalX + offsetX;
                Y = originalY + offsetY;

                if(framePath < directionThresold1 )
                {
                    isHorizontalFlipped = true;
                }
                else if( framePath < directionThresold2)
                {
                    isHorizontalFlipped = false;
                }
                else if (framePath < directionThresold3)
                {
                    isHorizontalFlipped = true;
                }
                else
                {
                    isHorizontalFlipped = false;
                }

                framePath++;
            }
            else
            {
                this.IsAlive = false;
            }

            if(Y < screen.BoundsClipped.Top - this.Height)
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

            screen.DrawSpriteMap(walkMaps[walkIndex], XScrolled, YScrolled, isHorizontalFlipped, false);
        }
    }
}
