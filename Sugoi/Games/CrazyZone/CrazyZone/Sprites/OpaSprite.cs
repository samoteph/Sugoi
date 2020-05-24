using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrazyZone.Sprites
{
    public sealed class OpaSprite : Sprite
    {
        private Machine machine;
        private SurfaceTileSheet tiles;

        private Animator flightAnimator;
        private Animator walkAnimator;

        private Animator deathStarAnimator;
        private int frameDeathStar;
        private int deathStarIndex;

        private int frameDeathStarThresold0;
        private int frameDeathStarThresold1;


        private bool isOpaHorizontalFlipped;

        private Rectangle rectScroll;

        private int frameAmmo;
        private int frameBomb;

        EasingPath pathDeathStart = new EasingPath();

        PlayPage page;

        public OpaSprite(Machine machine, PlayPage page)
        {
            this.machine = machine;

            this.page = page;

            pathDeathStart.Initialize(EasingFunctions.CircularEaseOut, EasingFunctions.Linear, 40, 1, 1, 1, 100);

            frameDeathStarThresold0 = (int)((double)pathDeathStart.MaximumFrame * 0.4d);
            frameDeathStarThresold1 = (int)((double)pathDeathStart.MaximumFrame * 0.8d);

            tiles = AssetStore.Tiles;
            flightAnimator = AssetStore.OpaFlightAnimator;
            walkAnimator = AssetStore.OpaWalkAnimator;
            
            deathStarAnimator = AssetStore.CreateDeathStartAnimation();
            deathStarAnimator.AnimationType = AnimationTypes.Manual;

            isOpaHorizontalFlipped = true;

            Initialize();
        }

        public override string TypeName
        {
            get
            {
                return nameof(OpaSprite);
            }
        }

        public override bool CanCollide
        {
            get
            {
                return this.IsDying == false;
            }
        }

        /// <summary>
        /// Juste avant de mourir
        /// </summary>

        public bool IsDying
        {
            get;
            private set;
        }
        public bool CanFireAmmo
        {
            get;
            private set;
        }

        public bool CanFireBomb
        {
            get;
            private set;
        }

        public bool IsMoving
        {
            get;
            private set;
        }

        public float Speed
        {
            get;
            private set;
        }

        public int Direction
        {
            get;
            private set;
        }

        public bool IsWalking
        {
            get;
            private set;
        }
        public override void Initialize() 
        {
            this.IsAlive = true;
            this.IsDying = false;

            CanFireAmmo = true;
            CanFireBomb = true;

            var screen = this.machine.Screen;

            this.X = (screen.BoundsClipped.Width - 16) / 2;
            this.Y = (screen.BoundsClipped.Height - 16) / 2;

            Direction = 1;
            isOpaHorizontalFlipped = true;

            Speed = 1;
            IsMoving = true;
            IsWalking = false;

            frameAmmo = 0;
            frameBomb = 0;
            frameDeathStar = 0;

            var widthScroll = (screen.BoundsClipped.Width * 1) / 3;
            var xScroll = (screen.BoundsClipped.Width - widthScroll) / 2;

            this.rectScroll = new Rectangle(xScroll, screen.BoundsClipped.Y, widthScroll, screen.BoundsClipped.Height);

            this.Width = flightAnimator.Width;
            this.Height = flightAnimator.Height;

            deathStarIndex = 0;

            this.flightAnimator.Start();
            this.walkAnimator.Start();
            this.deathStarAnimator.Start();

            this.InitializeCollision(3);
        }

        public override void Updated()
        {
            if(this.IsAlive == false)
            {
                return;
            }

            if (this.IsDying == false)
            {
                var gamepad = machine.Gamepad;

                switch (gamepad.HorizontalController)
                {
                    case GamepadKeys.Right:
                        Direction = 1;
                        Speed = 1;
                        this.IsMoving = true;
                        break;
                    case GamepadKeys.Left:
                        Direction = -1;
                        Speed = -1;
                        this.IsMoving = true;
                        break;
                    default:
                        this.IsMoving = false;
                        Speed = 0.5f * Direction;
                        break;
                }

                switch (gamepad.VerticalController)
                {
                    case GamepadKeys.Up:
                        Y += 2;
                        break;
                    case GamepadKeys.Down:
                        Y -= 2;
                        break;
                }

                // on attends un peu de pouvoir tirer à nouveau
                if (CanFireAmmo == false)
                {
                    if (frameAmmo > 5)
                    {
                        CanFireAmmo = true;
                    }
                    else
                    {
                        frameAmmo++;
                    }
                }

                // on attends un peu de pouvoir tirer à nouveau
                if (CanFireBomb == false)
                {
                    if (frameBomb > 40)
                    {
                        CanFireBomb = true;
                    }
                    else
                    {
                        frameBomb++;
                    }
                }

                // On tire !
                if (gamepad.IsPressed(GamepadKeys.ButtonA))
                {
                    if (CanFireAmmo)
                    {
                        frameAmmo = 0;
                        CanFireAmmo = false;

                        page.Ammos.GetFreeSprite().Fire(X, Y, Direction);
                    }
                }
                else if (gamepad.IsPressed(GamepadKeys.ButtonB))
                {
                    if (CanFireBomb)
                    {
                        // On largue une bombe !
                        frameBomb = 0;
                        CanFireBomb = false;

                        page.Bombs.GetFreeSprite().Fire(X + (Width / 2) + (int)page.ScrollX, Y + 8, Direction);
                    }
                }

                /// Bornes
                if (X + Speed >= rectScroll.Right)
                {
                    X = rectScroll.Right;

                    if (this.IsMoving)
                    {
                        Speed = 2;
                    }
                }
                else if (X + Speed <= rectScroll.X)
                {
                    X = rectScroll.X;

                    if (this.IsMoving)
                    {
                        Speed = -2;
                    }
                }
                else
                {
                    X += (int)Speed;
                }

                if (Y <= (rectScroll.Y - 1))
                {
                    Y = -1;
                }
                else if (Y >= rectScroll.Bottom - 16)
                {
                    Y = rectScroll.Bottom - 16;
                    IsWalking = true;
                }
                else if (IsWalking == true)
                {
                    IsWalking = false;
                }

                // on marche

                if (IsWalking)
                {
                    if (Direction != 0)
                    {
                        walkAnimator.Speed = 2;
                    }
                    else
                    {
                        walkAnimator.Speed = 1;
                    }
                }

                // retournement
                if (IsMoving == true)
                {
                    isOpaHorizontalFlipped = Direction == -1 ? false : true;
                }

                // battement des ailes : varie selon que l'on bouge ou non
                if (IsMoving == false)
                {
                    flightAnimator.Speed = 1;
                }
                else
                {
                    flightAnimator.Speed = 2;
                }

                flightAnimator.Update();
                walkAnimator.Update();
            }

            // on est pas attaché au scroll donc pas besoin de SetScroll mais les collisions se base sur XScrolled et YScrolled;
            XScrolled = X;
            YScrolled = Y;
        }

        public override void Draw(int frameExecuted)
        {
            if(this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            if (this.IsDying == true)
            {
                if (frameDeathStar <= pathDeathStart.MaximumFrame)
                {
                    // seuil de changement d'apparence selon l'avancement dans le temps
                    if(frameDeathStar < frameDeathStarThresold0)
                    {
                        deathStarAnimator.SetFrame(0);
                    }
                    else if(frameDeathStar < frameDeathStarThresold1 )
                    {
                        deathStarAnimator.SetFrame(1);
                    }
                    else
                    {
                        deathStarAnimator.SetFrame(2);
                    }

                    // explosion avant la mort
                    pathDeathStart.GetPosition(frameDeathStar, out var offsetX, out var offsetY);

                    double currentOffsetX;

                    for (double i = 0; i < 3; i++)
                    {
                        currentOffsetX = (double)offsetX * (1d + (i * 0.5d));

                        // Horizontal / Vertical

                        double step = (Math.PI * 2) / 16;

                        for(double a = 0; a < 16; a++)
                        {
                            var x = X + (int)(currentOffsetX * Math.Cos(a * step));
                            var y = Y + (int)(currentOffsetX * Math.Sin(a * step));

                            deathStarAnimator.Draw(screen, x, y);
                        }
                    }
                }
                else if( frameDeathStar > pathDeathStart.MaximumFrame + 10)
                {
                    this.IsDying = false;
                    this.IsAlive = false;
                    // GameOver
                    page.GameOver();
                }

                frameDeathStar += frameExecuted;
            }
            else
            {
                flightAnimator.Draw(screen, X, Y, isOpaHorizontalFlipped, false);

                if (IsWalking)
                {
                    walkAnimator.Draw(screen, X, Y + 8, isOpaHorizontalFlipped, false);
                }
            }
        }

        public override void Collide(ISprite collider)
        {
            // Explosion de fin avant le game over
            if (collider.TypeName != nameof(CoinSprite))
            {
                this.IsDying = true;
                this.machine.Audio.Play("opaExplosionSound");
            }
        }
    }
}
