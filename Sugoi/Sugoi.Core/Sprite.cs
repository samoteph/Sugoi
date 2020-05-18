using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    public abstract class Sprite : ISprite
    {
        public bool IsAlive
        {
            get;
            set;
        }
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public int Width
        {
            get;
            protected set;
        }

        public int Height
        {
            get;
            protected set;
        }

        public int XScrolled
        {
            get
            {
                return xScrolled;
            }

            private set
            {
                if (value != xScrolled)
                {
                    if (OldXScrolled == int.MinValue)
                    {
                        OldXScrolled = value;
                    }
                    else
                    {
                        OldXScrolled = xScrolled;
                    }

                    xScrolled = value;
                }
            }
        }

        private int xScrolled;

        public int OldXScrolled
        {
            get;
            private set;
        } = int.MinValue;

        public int YScrolled
        {
            get
            {
                return yScrolled;
            }

            private set
            {
                if (value != yScrolled)
                {
                    if (OldYScrolled == int.MinValue)
                    {
                        OldYScrolled = value;
                    }
                    else
                    {
                        OldYScrolled = yScrolled;
                    }

                    yScrolled = value;
                }
            }
        }

        private int yScrolled;

        public int OldYScrolled
        {
            get;
            private set;
        } = int.MinValue;

        public int ScrollX
        {
            get;
            set;
        }

        public int ScrollY
        {
            get;
            set;
        }

        public int ScrollWidth
        {
            get;
            set;
        }

        public int Damage
        {
            get;
            set;
        }

        public int ScrollHeight
        {
            get;
            set;
        }

        public abstract string TypeName
        {
            get;
        }

        public void SetPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// A effectué après changement de X,Y,ScrollWidth,ScrollHeight,ScrollX,ScrollY pour recalculer XScrolled et YScrolled
        /// </summary>

        public void UpdatePositionScrolled()
        {
            this.UpdateXScrolled();
            this.UpdateYScrolled();
        }

        public void UpdateXScrolled()
        {
            this.XScrolled = this.UpdatePositionScrolled(this.X, this.Width, this.ScrollX, this.ScrollWidth);
        }

        public void UpdateYScrolled()
        {
            this.YScrolled = this.UpdatePositionScrolled(this.Y, this.Height, this.ScrollY, this.ScrollHeight);
        }

        /// <summary>
        /// Calcule XScrolled en prenant en compte x, le scrolling et la taille du scroll (une Map.Width normalement)
        /// Il est capable de calculer des scrolling negatif ou depassant la map en les enroulant (infinite scroll) 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scrollPosition"></param>
        /// <param name="scrollSize"></param>

        private int UpdatePositionScrolled(int position, int size, int scrollPosition, int scrollSize)
        {
            if(scrollSize <= 0)
            {
                return position;
            }

            var xScrolled = scrollPosition % scrollSize + position;

            if (xScrolled < -size)
            {
                xScrolled = scrollSize + xScrolled;
            }
            else if (xScrolled > scrollSize - size)
            {
                xScrolled = xScrolled - scrollSize;
            }

            return xScrolled;
        }

        public Rectangle CollisionBounds
        {
            get;
            private set;
        }

        public void InitializeCollision(int margin)
        {
            this.OldXScrolled = int.MinValue;
            this.OldYScrolled = int.MinValue;

            this.CollisionBounds = new Rectangle(margin, margin, this.Width - margin * 2, this.Height - margin * 2);
        }

        public void InitializeCollision(int marginLeft, int marginTop, int marginRight, int marginBottom)
        {
            this.OldXScrolled = int.MinValue;
            this.OldYScrolled = int.MinValue;

            this.CollisionBounds = new Rectangle(marginLeft, marginTop, this.Width - (marginLeft + marginRight), this.Height - (marginTop + marginBottom));
        }

        /// <summary>
        /// Il autorise les collisions
        /// </summary>

        public virtual bool CanCollide
        {
            get
            {
                return true;
            }
        }

        public abstract void Collide(ISprite sprite);

        public abstract void Draw(int frameExecuted);

        public abstract void Initialize();

        public virtual void Updating()
        {
        }

        public virtual void Updated()
        {
            this.UpdatePositionScrolled();
        }
    }
}
