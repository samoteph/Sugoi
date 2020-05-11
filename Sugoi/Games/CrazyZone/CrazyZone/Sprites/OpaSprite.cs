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

        private bool isOpaHorizontalFlipped;
        private int opaFlightIndex;
        private Rectangle rectScroll;

        private AmmoSprite ammoSprite;

        public OpaSprite(Machine machine) : this()
        {
            this.machine = machine;

            tiles = AssetStore.Tiles;
            flightMaps = AssetStore.OpaFlightMaps;

            isOpaHorizontalFlipped = true;

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

        public void Initialize() 
        {
            this.IsAlive = true;

            var screen = this.machine.Screen;

            this.X = (screen.BoundsClipped.Width - 16) / 2;
            this.Y = (screen.BoundsClipped.Height - 16) / 2;

            Direction = 1;
            Speed = 1;
            this.IsMoving = true;

            var widthScroll = (screen.BoundsClipped.Width * 1) / 3;
            var xScroll = (screen.BoundsClipped.Width - widthScroll) / 2;

            this.rectScroll = new Rectangle(xScroll, screen.BoundsClipped.Y, widthScroll, screen.BoundsClipped.Height);

            ammoSprite = new AmmoSprite(machine);
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
                    Y++;
                    break;
                case GamepadKeys.Down:
                    Y--;
                    break;
            }

            if(gamepad.IsPressed(GamepadKeys.ButtonA))
            {
                ammoSprite.Fire(X, Y, Direction);
            }

            if( X + Speed > rectScroll.Right )
            {
                X = rectScroll.Right;
            }
            else if(X + Speed < rectScroll.X)
            {
                X = rectScroll.X;
            }
            else
            {
                X += (int)Speed;
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
        }
    }
}
