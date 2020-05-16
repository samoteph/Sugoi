using CrazyZone.Sprites;
using Sugoi.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Pages
{
    public class PlayPage : IPage
    {
        private Game game;
        private Machine machine;
        private float scrollX;
        
        private OpaSprite opa;

        private SpritePool<MotherSprite> mothers = new SpritePool<MotherSprite>(10);
        private SpritePool<AmmoSprite> ammos = new SpritePool<AmmoSprite>(10);
        private SpritePool<BombSprite> bombs = new SpritePool<BombSprite>(10);
        private SpritePool<KaboomSprite> kabooms = new SpritePool<KaboomSprite>(10);
        private SpritePool<BabySprite> babies = new SpritePool<BabySprite>(10);

        Map[] maps;

        public SpritePool<AmmoSprite> Ammos
        {
            get
            {
                return this.ammos;
            }
        }

        public SpritePool<BombSprite> Bombs
        {
            get
            {
                return this.bombs;
            }
        }

        public SpritePool<KaboomSprite> Kabooms
        {
            get
            {
                return this.kabooms;
            }
        }

        public SpritePool<BabySprite> Babies
        {
            get
            {
                return this.babies;
            }
        }

        public int ScrollWidth
        {
            get;
            private set;
        }

        public float ScrollX
        {
            get
            {
                return this.scrollX;
            }
        }

        public PlayPage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
            this.opa = new OpaSprite(machine, this);
        }

        public void Initialize()
        {
            this.machine.Frame = 0;

            this.maps = AssetStore.ParallaxMaps;
            this.ScrollWidth = this.maps[0].Width;

            this.opa.Initialize();

            this.babies.Reset();
            this.mothers.Reset();
            this.ammos.Reset();
            this.bombs.Reset();
            this.kabooms.Reset();

            var scrollWidth = this.maps[0].Width;

            this.mothers.GetSprite().Create(machine, this,  0, 0);
            this.mothers.GetSprite().Create(machine, this, 210, 100);
            this.mothers.GetSprite().Create(machine, this, 310, 80);
        }

        /// <summary>
        /// Mise à jour avant les waits
        /// </summary>

        public void Updating()
        {

        }

        /// <summary>
        /// Mise à jour après les waits
        /// </summary>

        public void Updated()
        {
            if (opa.IsAlive)
            {
                scrollX += opa.Speed;
            }

            mothers.SetScroll((int)-scrollX, 0);
            kabooms.SetScroll((int)-scrollX, 0);

            opa.Updated();
            ammos.Updated();

            // on place le setscroll ici car c'est Opa qui Fire la bomb et elle a besoin du scroll une fois tirée
            bombs.SetScroll((int)-scrollX, 0);
            bombs.Updated();

            mothers.Updated();

            babies.SetScroll((int)-scrollX, 0);
            babies.Updated();

            kabooms.Updated();

            mothers.CheckCollision(opa, CollisionStrategies.RectIntersect);
            babies.CheckCollision(opa, CollisionStrategies.RectIntersect);
            ammos.CheckCollision(mothers, CollisionStrategies.RectIntersect);
            ammos.CheckCollision(babies, CollisionStrategies.RectIntersect);
            bombs.CheckCollision(mothers, CollisionStrategies.RectIntersect);
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;
            var font = screen.Font;

            var frame = this.machine.Frame;

            screen.DrawScrollMap(maps[0], true, (int)(-scrollX * 0.25), 0, 0, 0, 320, 136);
            screen.DrawScrollMap(maps[1], true, (int)(-scrollX * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[3], true, (int)(-scrollX * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[2], true, (int)(-scrollX * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
            screen.DrawScrollMap(maps[4], true, (int)(-scrollX * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);

            ammos.Draw(frameExecuted);
            babies.Draw(frameExecuted);
            mothers.Draw(frameExecuted);
            kabooms.Draw(frameExecuted);
            opa.Draw(frameExecuted);
            bombs.Draw(frameExecuted);

            screen.DrawScrollMap(maps[5], true, (int)(-scrollX * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

            screen.DrawLine(0, 0, screen.BoundsClipped.Right, screen.BoundsClipped.Bottom, Argb32.Red);

            screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
        }
    }
}
