using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class SpritePool<TSprite> where TSprite : ISprite
    {
        public TSprite[] sprites;

        public SpritePool(int size)
        {
            this.sprites = (TSprite[])Array.CreateInstance(typeof(TSprite), size);
            
            for(int i=0; i< size; i++)
            {
                this.sprites[i] = Activator.CreateInstance<TSprite>();
            }
            
            this.CurrentIndex = 0;
        }

        public int CurrentIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Obtenir un sprite disponible (mort)
        /// </summary>

        public TSprite GetSprite()
        {
            var index = this.SearchDeadSpriteIndex(this.CurrentIndex);

            if(index == - 1)
            {
                throw new Exception("The pool is unable to reserve a new sprite! Please set a bigger size when initializing!");
            }
            else
            {
                this.CurrentIndex = index;
                return this.sprites[index];
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

                if (sprite.IsAlive == false)
                {
                    return index;
                }
            }

            // tout est pris !
            return -1;
        }

        public void SetScroll(int scrollX, int scrollY)
        {
            for (int i = 0; i < this.sprites.Length; i++)
            {
                var sprite = this.sprites[i];

                if (sprite.IsAlive)
                {
                    sprite.ScrollX = scrollX;
                    sprite.ScrollY = scrollY;
                }
            }
        }

        public void Updated()
        {
            for(int i=0; i<this.sprites.Length;i++)
            {
                var sprite = this.sprites[i];

                if (sprite.IsAlive)
                {
                    sprite.Updated();
                }
            }
        }

        public void Updating()
        {
            for (int i = 0; i < this.sprites.Length; i++)
            {
                var sprite = this.sprites[i];

                if (sprite.IsAlive)
                {
                    sprite.Updating();
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

        public void Reset()
        {
            this.CurrentIndex = 0;

            for(int i=0; i<this.sprites.Length;i++)
            {
                var sprite = this.sprites[i];

                sprite.IsAlive = false;
            }
        }

        public bool CheckCollision<TCollider>(SpritePool<TCollider> colliderPool) where TCollider : ISprite
        {
            bool haveCollision = false;
            var colliderSprites = colliderPool.sprites;

            for (int i = 0; i < colliderSprites.Length; i++)
            {
                var colliderSprite = colliderSprites[i];

                if (this.CheckCollision(colliderSprite) == true)
                {
                    haveCollision = true;
                }
            }

            return haveCollision;
        }

        public bool CheckCollision<TCollider>(TCollider collider) where TCollider : ISprite
        {
            if(collider == null || collider.IsAlive == false || collider.CanCollide == false)
            {
                return false;
            }

            bool haveCollision = false;
            var colliderCollisionRect = new Rectangle(collider.XScrolled + collider.CollisionBounds.X, collider.YScrolled + collider.CollisionBounds.Y, collider.CollisionBounds.Width, collider.CollisionBounds.Height);

            for (int i = 0; i < this.sprites.Length; i++)
            {
                var sprite = this.sprites[i];

                if(sprite.IsAlive == false || sprite.CanCollide == false)
                {
                    continue;
                }

                var spriteCollisionRect = new Rectangle(sprite.XScrolled + sprite.CollisionBounds.X, sprite.YScrolled + sprite.CollisionBounds.Y, sprite.CollisionBounds.Width, sprite.CollisionBounds.Height);

                if(spriteCollisionRect.IntersectsWith(colliderCollisionRect) == true )
                {
                    haveCollision = true;

                    sprite.Collide(collider);
                    collider.Collide(sprite);
                }
            }

            return haveCollision;
        }
    }
}
