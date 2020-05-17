using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class KaboomSprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private Map[] kaboomMaps;
        private int indexKaboom = 0;
        private int frameKaboom = 0;
        private int frameMaxKaboom = 15;

        private int[] animationKaboomIndex = { 0, 1, 0 };

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

            tiles = AssetStore.Tiles;
            kaboomMaps = AssetStore.KaboomMaps;

            this.ScrollWidth = page.ScrollWidth;
            this.Width = 8;
            this.Height = 8;

            Initialize();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xCenter">Center of the explode</param>
        /// <param name="yCenter">Center of the explode</param>

        public void Explode(int xCenter, int yCenter, bool isSlow = false)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            if(this.IsExploding == true)
            {
                return;
            }

            if(isSlow == false)
            {
                frameMaxKaboom = 8;
            }
            else
            {
                frameMaxKaboom = 15;
            }

            this.IsExploding = true;

            this.xCenter = xCenter;
            this.yCenter = yCenter;
        }

        public override void Initialize()
        {
            this.IsAlive = true;
            this.indexKaboom = 0;
            IsExploding = false;
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var index = animationKaboomIndex[indexKaboom];
            
            Width = kaboomMaps[index].Width;
            Height = kaboomMaps[index].Height;

            X = xCenter - (Width / 2);
            Y = yCenter - (Height / 2); 

            frameKaboom++;

            if (frameKaboom > frameMaxKaboom)
            {
                frameKaboom = 0;
                indexKaboom++;

                if (indexKaboom == 3)
                {
                    this.IsAlive = false;
                    return;
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

            var screen = this.machine.Screen;

            var index = animationKaboomIndex[indexKaboom];
            screen.DrawSpriteMap(kaboomMaps[index], XScrolled, YScrolled);
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
