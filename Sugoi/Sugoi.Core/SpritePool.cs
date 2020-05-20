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
        /// Obtenir un sprite disponible (mort). Le sprite est initialisé automatiquement
        /// </summary>

        public TSprite GetFreeSprite()
        {
            var index = this.SearchDeadSpriteIndex(this.CurrentIndex);

            if(index == - 1)
            {
                throw new Exception("The pool '"+ typeof(TSprite).Name + "' is unable to reserve a new sprite! Please set a bigger size when initializing (>" + this.sprites.Length  + ") !");
            }
            else
            {
                this.CurrentIndex = index;
                var sprite = this.sprites[index];

                sprite.Initialize();
                
                return sprite;
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

                //if (sprite.IsAlive)
                //{
                    sprite.ScrollX = scrollX;
                    sprite.ScrollY = scrollY;
                //}
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

        public void Create( Action<TSprite> spriteCreate = null)
        {
            this.CurrentIndex = 0;

            for(int i=0; i<this.sprites.Length;i++)
            {
                var sprite = this.sprites[i];

                sprite.IsAlive = false;

                spriteCreate?.Invoke(sprite);
            }
        }

        public bool CheckCollision<TCollider>(SpritePool<TCollider> colliderPool, CollisionStrategies collisionStrategy) where TCollider : ISprite
        {
            bool haveCollision = false;
            var colliderSprites = colliderPool.sprites;

            for (int i = 0; i < colliderSprites.Length; i++)
            {
                var colliderSprite = colliderSprites[i];

                if (this.CheckCollision(colliderSprite, collisionStrategy) == true)
                {
                    haveCollision = true;
                }
            }

            return haveCollision;
        }

        public bool CheckCollision<TCollider>(TCollider collider, CollisionStrategies collisionStrategy) where TCollider : ISprite
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


                if (collisionStrategy == CollisionStrategies.RectIntersect)
                {
                    var spriteCollisionRect = new Rectangle(sprite.XScrolled + sprite.CollisionBounds.X, sprite.YScrolled + sprite.CollisionBounds.Y, sprite.CollisionBounds.Width, sprite.CollisionBounds.Height);

                    if (spriteCollisionRect.IntersectsWith(colliderCollisionRect) == true)
                    {
                        haveCollision = true;

                        sprite.Collide(collider);
                        collider.Collide(sprite);
                    }
                }
                else
                {
                    // detection rapide entre l'ancienne et la nouvelle position

                    var left1 = sprite.XScrolled + sprite.CollisionBounds.X;
                    var left2 = sprite.OldXScrolled + sprite.CollisionBounds.X;

                    int left;
                    int right;

                    if( left1 < left2)
                    {
                        left = left1;
                        right = (left2 + sprite.CollisionBounds.Width) - left1;
                    }
                    else
                    {
                        left = left2;
                        right = (left1 + sprite.CollisionBounds.Width) - left2;
                    }

                    var top1 = sprite.YScrolled + sprite.CollisionBounds.Y;
                    var top2 = sprite.OldYScrolled + sprite.CollisionBounds.Y;

                    int top;
                    int bottom;

                    if (top1 < top2)
                    {
                        top = top1;
                        bottom = (top2 + sprite.CollisionBounds.Height) - top1;
                    }
                    else
                    {
                        top = top2;
                        bottom = (top1 + sprite.CollisionBounds.Height) - top2;
                    }

                    var spriteCollisionRect = new Rectangle(left, top, right, bottom);

                    if (spriteCollisionRect.IntersectsWith(colliderCollisionRect) == true)
                    {
                        // ici on a peut être une collision on doit faire une collision plus fine 
                        // bresenhame et la cible ? fonctionne si la cible n'est pas en mouvement également
                        // il vaudrait mieux faire un polygon avec l'ancienne forme du sprite et la nouvelle (une sorte de rectangle vu de coté) et testé la collision entre les deux polygones

                        throw new NotImplementedException("Collision CCD not implemented yet!");

                        haveCollision = true;

                        sprite.Collide(collider);
                        collider.Collide(sprite);
                    }

                }
            }

            return haveCollision;
        }
    }

    public enum CollisionStrategies
    {
        RectIntersect,
        ContinousCollisionDetection,
    }
}
