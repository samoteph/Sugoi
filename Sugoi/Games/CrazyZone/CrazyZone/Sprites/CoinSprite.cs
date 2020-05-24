using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class CoinSprite : Sprite
    {
        private PlayPage page;
        private Machine machine;
        private SurfaceTileSheet tiles;

        private Animator coin1Animator;
        private Animator coin5Animator;

        private Animator coinAnimator;

        private int frameCoinAnimation = 0;
        private double framePath = 0;

        private int originalX = 0;
        private int originalY = 0;

        private CoinTypes coinType;

        EasingPath pathMove = new EasingPath();
        EasingPath pathAnimation = new EasingPath();

        public CoinSprite Create(Machine machine, PlayPage page)
        {
            this.machine = machine;

            this.page = page;
            tiles = AssetStore.Tiles;
            
            coin1Animator = AssetStore.CreateCoin1Animation();
            coin5Animator = AssetStore.CreateCoin5Animation();

            this.Width = 16;
            this.Height = 16;

            this.InitializeCollision(3);

            return this;
        }

        public override void Initialize()
        {
            framePath = 0;
        }

        public override string TypeName
        {
            get
            {
                return nameof(CoinSprite);
            }
        }

        /// <summary>
        /// Naissance du bébé
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Born(int x, int y, CoinTypes coinType)
        {
            this.IsAlive = true;

            this.coinType = coinType;

            if(coinType == CoinTypes.Coin1)
            {
                coinAnimator = coin1Animator;
            }
            else
            {
                coinAnimator = coin5Animator;
            }

            coinAnimator.AnimationType = AnimationTypes.Manual;
            coinAnimator.Start();

            int yPath = machine.Screen.BoundsClipped.Bottom - y - this.Height - 4;

            var width = this.machine.GetRandomInteger(10, 70);
            var direction = this.machine.GetRandomInteger(2) == 0 ? -1 : 1;

            // deplacement
            this.pathMove.Initialize(EasingFunctions.QuinticEaseOut, EasingFunctions.BounceEaseOut, width, yPath, direction, 1, 100);
            // temps d'animation : plus c'est le debut plus c'est rapide
            this.pathAnimation.Initialize(EasingFunctions.QuinticEaseOut, EasingFunctions.Linear, 5, 0, 1, 1, 100);

            this.X = x;
            this.Y = y;

            this.originalY = y;
            this.originalX = X;
        }

        public override void Collide(ISprite sprite)
        {
            // ajoute un score
            this.page.AddBonusScore((int)coinType * 10);
            this.IsAlive = false;
            this.machine.Audio.Play("startSound");
        }

        public override void Updated()
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            if (framePath <= pathMove.MaximumFrame + (60 * 2)) // attente supplementaire
            {
                int f = (int)framePath;

                pathMove.GetPosition(f, out var offsetX, out var offsetY);

                X = originalX + offsetX;
                Y = originalY + offsetY;

                framePath += 0.5;

                pathAnimation.GetPosition(f, out var animationX, out var animationY);

                if (frameCoinAnimation > animationX)
                {
                    frameCoinAnimation = 0;

                    coinAnimator.NextFrame();
                }
                else
                {
                    frameCoinAnimation++;
                }
            }
            else
            {
                this.IsAlive = false;
            }

            if (Y < screen.BoundsClipped.Top - this.Height)
            {
                this.IsAlive = false;
            }

            this.SetScroll(page);
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            coinAnimator.Draw(screen, XScrolled, YScrolled);

            this.DrawCollisionBox(screen);
        }
    }

    public enum CoinTypes
    {
        Coin1 = 1,
        Coin5 = 5
    }
}
