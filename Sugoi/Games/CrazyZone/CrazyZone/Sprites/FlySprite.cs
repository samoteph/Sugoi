using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class FlySprite : Sprite
    {
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

        public override void Initialize()
        {
        }
    }
}
