using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrazyZone.Sprites
{
    public class KaboomSprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private Animator kaboomAnimator;

        private int xCenter = 0;
        private int yCenter = 0;

        public bool IsExploding
        {
            get;
            private set;
        }

        public override string TypeName
        {
            get
            {
                return nameof(KaboomSprite);
            }
        }

        public KaboomSprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;
            this.page = page;

            tiles = AssetStore.Tiles;

            kaboomAnimator = AssetStore.CreateKaboomAnimation();

            this.Width = 8;
            this.Height = 8;

            kaboomAnimator.Stop();

            Width = kaboomAnimator.Width;
            Height = kaboomAnimator.Height;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xCenter">Center of the explode</param>
        /// <param name="yCenter">Center of the explode</param>

        public void Explode(int xCenter, int yCenter, bool isSlow = false)
        {
            if(this.IsExploding == true)
            {
                return;
            }

            this.IsExploding = true;

            if (isSlow == false)
            {
                kaboomAnimator.Speed = 2;
                this.machine.Audio.Play("monsterExplosionSound");
            }
            else
            {
                kaboomAnimator.Speed = 1;
                this.machine.Audio.Play("motherExplosionSound");
            }

            this.xCenter = xCenter;
            this.yCenter = yCenter;

            // jouer trois frames avant la fin
            this.kaboomAnimator.Start(3, (parameter) =>
            {
                KaboomSprite sprite = (KaboomSprite)parameter;

                sprite.IsExploding = false;
                sprite.IsAlive = false;
            },
            this
            );
        }

        public override void Initialize()
        {
            IsExploding = false;
            this.IsAlive = true;
        }

        public override void Updated()
        {
            if (this.IsExploding == false)
            {
                return;
            }

            X = xCenter - (Width / 2);
            Y = yCenter - (Height / 2);

            kaboomAnimator.Update();

            this.SetScroll(page);
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsExploding == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            kaboomAnimator.Draw(screen, XScrolled, YScrolled);
        }

        public override bool CanCollide
        {
            get
            {
                return false;
            }
        }


        public override void Collide(ISprite sprite)
        {
            // pas de collision autorisé
        }
    }
}
