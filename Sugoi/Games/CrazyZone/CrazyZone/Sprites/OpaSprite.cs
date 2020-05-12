using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public struct OpaSprite : ISprite
    {
        private Machine machine;
        private SurfaceTileSheet tiles;

        private Map[] flightMaps;
        private int opaFlightIndex;

        private Map[] walkMaps;
        private int opaWalkIndex;

        private bool isOpaHorizontalFlipped;

        private Rectangle rectScroll;

        private AmmoSprite ammoSprite;

        public OpaSprite(Machine machine) : this()
        {
            this.machine = machine;

            tiles = AssetStore.Tiles;
            flightMaps = AssetStore.OpaFlightMaps;
            walkMaps = AssetStore.OpaWalkMaps;

            isOpaHorizontalFlipped = true;
            ammoSprite = new AmmoSprite(machine);

            Initialize();
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

        public bool IsWalking
        {
            get;
            private set;
        }

        public void Initialize() 
        {
            this.IsAlive = true;

            var screen = this.machine.Screen;

            this.X = (screen.BoundsClipped.Width - 16) / 2;
            this.Y = (screen.BoundsClipped.Height - 16) / 2;

            Direction = 1;
            Speed = 1;
            this.IsMoving = true;

            opaFlightIndex = 0;
            opaWalkIndex = 0;

            var widthScroll = (screen.BoundsClipped.Width * 1) / 3;
            var xScroll = (screen.BoundsClipped.Width - widthScroll) / 2;

            this.rectScroll = new Rectangle(xScroll, screen.BoundsClipped.Y, widthScroll, screen.BoundsClipped.Height);

            ammoSprite.Initialize();
        }

        public void Update()
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
                    Y+=2;
                    break;
                case GamepadKeys.Down:
                    Y-=2;
                    break;
            }

            if(gamepad.IsPressed(GamepadKeys.ButtonA))
            {
                ammoSprite.Fire(X, Y, Direction);
            }

            if( X + Speed >= rectScroll.Right )
            {
                X = rectScroll.Right;
                
                if (this.IsMoving)
                {
                    Speed = 2;
                }
            }
            else if(X + Speed <= rectScroll.X)
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

            if(Y <= (rectScroll.Y - 1))
            {
                Y = -1;
            }
            else if(Y >= rectScroll.Bottom - 16)
            {
                Y = rectScroll.Bottom - 16;
                IsWalking = true;
            }
            else if(IsWalking == true)
            {
                IsWalking = false;
            }

            if(IsWalking)
            {
                if (Direction != 0)
                {
                    opaWalkIndex = (this.machine.Frame % 20) > 10 ? 0 : 1;
                }
                else
                {
                    opaWalkIndex = (this.machine.Frame % 10) > 5 ? 0 : 1;
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
                var frameIndex = this.machine.Frame % 30;

                if (frameIndex > 20) opaFlightIndex = 2;
                else if (frameIndex > 10) opaFlightIndex = 1;
                else opaFlightIndex = 0;
            }
            else
            {
                var frameIndex = this.machine.Frame % 15;

                if (frameIndex > 10) opaFlightIndex = 2;
                else if (frameIndex > 5) opaFlightIndex = 1;
                else opaFlightIndex = 0;
            }

            ammoSprite.Update();
        } 

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;

            ammoSprite.Draw(frameExecuted);
            screen.DrawSpriteMap(flightMaps[opaFlightIndex], X, Y, isOpaHorizontalFlipped, false);

            if(IsWalking)
            {
                screen.DrawSpriteMap(walkMaps[opaWalkIndex], X, Y + 8, isOpaHorizontalFlipped, false);
            }
        }
    }
}
