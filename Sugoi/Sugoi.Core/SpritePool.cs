using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SpritePool
    {
        public ISprite[] sprites;

        public void Initialize(int size)
        {
            this.sprites = new ISprite[size];
            this.CurrentIndex = 0;
        }

        public int CurrentIndex
        {
            get;
            private set;
        }

        public void ReserveSprite(ISprite sprite)
        {
            var index = this.SearchDeadSpriteIndex(this.CurrentIndex);

            if(index == - 1)
            {
                throw new Exception("The pool is unable to reserve a new sprite! Please set a bigger size when initializing!");
            }
            else
            {
                sprite.Initialize();
                sprite.IsAlive = true;

                this.sprites[index] = sprite;
            }
        }

        private int SearchDeadSpriteIndex(int startIndex)
        {
            var length = this.sprites.Length;
            var endIndex = length + startIndex;
            
            for(int i=startIndex; i< endIndex; i++)
            {
                var index = i % length;

                var sprite = this.sprites[index];

                if (sprite == null || sprite.IsAlive == false)
                {
                    return index;
                }
            }

            // tout est pris !
            return -1;
        }

        public void Update()
        {
            for(int i=0; i<this.sprites.Length;i++)
            {
                var sprite = this.sprites[i];

                if (sprite.IsAlive)
                {
                    sprite.Update();
                }
            }
        }

        public void Draw(int frameExecuted)
        {
            for (int i = 0; i < this.sprites.Length; i++)
            {
                var sprite = this.sprites[i];

                if (sprite.IsAlive)
                {
                    sprite.Draw(frameExecuted);
                }
            }
        }
    }

    public interface ISprite
    {
        bool IsAlive
        {
            get;
            set;
        }

        int X
        {
            get;
            set;
        }

        int Y
        {
            get;
            set;
        }

        void Initialize();

        void Update();

        void Draw(int frameExecuted);
    }
}
