using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class FlySprite : Sprite
    {
        Machine machine;
        PlayPage page;
        SurfaceTileSheet tiles;

        private bool isHorizontalFlipped;

        public override string TypeName
        {
            get
            {
                return nameof(FlySprite);
            }
        }

        public override void Collide(ISprite sprite)
        {
            this.IsAlive = false;
        }

        public override void Draw(int frameExecuted)
        {
        }

        public void Create(Machine machine, PlayPage page)
        {
            this.machine = machine;
            this.page = page;
        }

        public override void Initialize()
        {
        }

        public void Start(int x, int y)
        {

        }
    }
}
