using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class AmmoSprite : Sprite
    {
        private Machine machine;
        private PlayPage page;

        private SurfaceTileSheet tiles;

        private bool isHorizontalFlipped;

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

        public override string TypeName
        {
            get
            {
                return nameof(AmmoSprite);
            }
        }

        public AmmoSprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;
            this.page = page;

            tiles = AssetStore.Tiles;

            this.Width = 8;
            this.Height = 8;

            this.InitializeCollision(0);

            return this;
        }

        public override void Collide(ISprite collider)
        {
            this.IsAlive = false;

            if (collider.TypeName != nameof(MotherSprite))
            {
                page.AddBonusScore(1);
            }
            else
            {
                this.machine.Audio.Play("ammoExplosionSound");
            }
        }

        public void Fire(int x, int y, int direction)
        {
            if (IsFiring == true) return;

            IsFiring = true;
            this.IsAlive = true;

            this.Direction = direction;
            this.X = x;
            this.Y = y + 4;

            this.machine.Audio.Play("ammoSound");
        }

        public override void Initialize()
        {
            isHorizontalFlipped = true;
            this.Damage = 1;

            this.IsFiring = false;
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            if (this.IsFiring == true)
            {
                // retournement
                isHorizontalFlipped = Direction == -1 ? false : true;
                
                // on avance de 8 toutes les frames
                X += Direction * 8;

                var bounds = this.machine.Screen.BoundsClipped;

                if(X > bounds.Right || X < bounds.X )
                {
                    IsFiring = false;
                    IsAlive = false;
                }
            }

            // on est pas attaché au scroll donc pas besoin de SetScroll mais les collisions se base sur XScrolled et YScrolled;
            XScrolled = X;
            YScrolled = Y;
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            if (IsFiring == true)
            {
                var screen = this.machine.Screen;
                screen.DrawTile(tiles, 188,  X, Y, isHorizontalFlipped, false);

                this.DrawCollisionBox(screen);
            }
        }
    }
}
