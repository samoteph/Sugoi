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
        const string GAMEOVER_TEXT = "Game Over";

        private Game game;
        private Machine machine;
        private float scrollX;

        private int frameGameOver = 0;
        private int frameDucks = 0;
        private int frameFlies = 0;

        private OpaSprite opa;

        private SpritePool<MotherSprite> mothers = new SpritePool<MotherSprite>(10);
        private SpritePool<AmmoSprite> ammos = new SpritePool<AmmoSprite>(10);
        private SpritePool<BombSprite> bombs = new SpritePool<BombSprite>(10);
        private SpritePool<BulletSprite> bullets = new SpritePool<BulletSprite>(30);
        private SpritePool<KaboomSprite> kabooms = new SpritePool<KaboomSprite>(10);
        private SpritePool<BabySprite> babies = new SpritePool<BabySprite>(60);
        private SpritePool<DuckSprite> ducks = new SpritePool<DuckSprite>(20);
        private SpritePool<FlySprite> flies = new SpritePool<FlySprite>(20);

        public PlayStates State
        {
            get;
            private set;
        }

        Map[] maps;

        public OpaSprite Opa
        {
            get
            {
                return this.opa;
            }
        }

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

        public SpritePool<BulletSprite> Bullets
        {
            get
            {
                return this.bullets;
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
            State = PlayStates.Play;

            this.machine.Frame = 0;

            frameGameOver = 0;
            frameDucks = 0;
            frameFlies = 0;

            scrollX = 0;

            this.maps = AssetStore.ParallaxMaps;
            this.ScrollWidth = this.maps[0].Width;

            this.opa.Initialize();

            this.babies.Create(baby => baby.Create(machine, this));
            this.mothers.Create(mother => mother.Create(machine, this));
            this.ammos.Create( ammo => ammo.Create(machine));
            this.bombs.Create(bomb => bomb.Create(machine, this));
            this.kabooms.Create(kaboom => kaboom.Create(machine, this));
            this.ducks.Create(duck => duck.Create(machine, this));
            this.bullets.Create(bullet => bullet.Create(machine, this));
            this.flies.Create(fly => fly.Create(machine, this));

            this.mothers.GetFreeSprite().SetPosition(0, 0);
            this.mothers.GetFreeSprite().SetPosition(210, 100);
            this.mothers.GetFreeSprite().SetPosition(310, 80);
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
            if (State != PlayStates.Quit)
            {
                if (opa.IsAlive && opa.IsDying == false)
                {
                    scrollX += opa.Speed;
                }

                if(frameDucks > (60 * 8) )
                {
                    frameDucks = 0;

                    const int  duckCount = 4;
                    var step = machine.Screen.BoundsClipped.Height / (duckCount + 1);

                    for (int y = 0; y < duckCount; y++)
                    {
                        this.ducks.GetFreeSprite().Born(step + y * step);
                    }
                }
                else
                {
                    frameDucks++;
                }

                if (frameFlies > (60 * 12))
                {
                    frameFlies = 0;

                    var centerY = (machine.Screen.BoundsClipped.Height - 16) / 2;

                    this.flies.GetFreeSprite().Born(0, centerY);
                    
                    this.flies.GetFreeSprite().Born(20, centerY - 8);
                    this.flies.GetFreeSprite().Born(20, centerY + 8);
                    
                    this.flies.GetFreeSprite().Born(40, centerY - 16);
                    this.flies.GetFreeSprite().Born(40, centerY );
                    this.flies.GetFreeSprite().Born(40, centerY + 16);

                    this.flies.GetFreeSprite().Born(60, centerY - 24);
                    this.flies.GetFreeSprite().Born(60, centerY - 8);
                    this.flies.GetFreeSprite().Born(60, centerY + 8);
                    this.flies.GetFreeSprite().Born(60, centerY + 24);

                }
                else
                {
                    frameFlies++;
                }

                mothers.SetScroll((int)-scrollX, 0);

                opa.Updated();
                ammos.Updated();

                // on place le setscroll ici car c'est Opa qui Fire la bomb et elle a besoin du scroll une fois tirée
                bombs.SetScroll((int)-scrollX, 0);
                bombs.Updated();

                mothers.Updated();

                babies.SetScroll((int)-scrollX, 0);
                babies.Updated();

                ducks.SetScroll((int)-scrollX, 0);
                ducks.Updated();

                flies.SetScroll((int)-scrollX, 0);
                flies.Updated();

                bullets.SetScroll((int) -scrollX, 0);
                bullets.Updated();

                kabooms.SetScroll((int)-scrollX, 0);
                kabooms.Updated();

                mothers.CheckCollision(opa, CollisionStrategies.RectIntersect);
                babies.CheckCollision(opa, CollisionStrategies.RectIntersect);
                ducks.CheckCollision(opa, CollisionStrategies.RectIntersect);
                flies.CheckCollision(opa, CollisionStrategies.RectIntersect);
                bullets.CheckCollision(opa, CollisionStrategies.RectIntersect);

                ammos.CheckCollision(mothers, CollisionStrategies.RectIntersect);
                ammos.CheckCollision(babies, CollisionStrategies.RectIntersect);
                ammos.CheckCollision(ducks, CollisionStrategies.RectIntersect);
                ammos.CheckCollision(flies, CollisionStrategies.RectIntersect);

                bombs.CheckCollision(mothers, CollisionStrategies.RectIntersect);
                bombs.CheckCollision(babies, CollisionStrategies.RectIntersect);
                bombs.CheckCollision(ducks, CollisionStrategies.RectIntersect);
                bombs.CheckCollision(flies, CollisionStrategies.RectIntersect);
            }

            switch (State)
            {
                case PlayStates.GameOver:
                    if (frameGameOver > 60 * 5)
                    {
                        State = PlayStates.Quit;
                    }
                    else
                    {
                        frameGameOver++;
                    }
                    break;

                case PlayStates.Quit:
                    machine.WaitForFrame(30, () =>
                    {
                        game.Navigate(typeof(HomePage));
                    });
                    break;
            }
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;

            if (State != PlayStates.Quit)
            {
                screen.DrawScrollMap(maps[0], true, (int)(-scrollX * 0.25), 0, 0, 0, 320, 136);
                screen.DrawScrollMap(maps[1], true, (int)(-scrollX * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
                screen.DrawScrollMap(maps[3], true, (int)(-scrollX * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
                screen.DrawScrollMap(maps[2], true, (int)(-scrollX * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
                screen.DrawScrollMap(maps[4], true, (int)(-scrollX * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);

                ammos.Draw(frameExecuted);
                ducks.Draw(frameExecuted);
                flies.Draw(frameExecuted);
                babies.Draw(frameExecuted);
                mothers.Draw(frameExecuted);
                kabooms.Draw(frameExecuted);
                opa.Draw(frameExecuted);
                bombs.Draw(frameExecuted);
                bullets.Draw(frameExecuted);

                screen.DrawScrollMap(maps[5], true, (int)(-scrollX * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

                if (State == PlayStates.GameOver)
                {
                    screen.DrawText(GAMEOVER_TEXT, (screen.BoundsClipped.Width - (GAMEOVER_TEXT.Length * 8)) / 2, (screen.BoundsClipped.Height - 8) / 2);
                }

            }
            else
            {
                screen.Clear(Argb32.Black);
            }

            screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
        }

        public void GameOver()
        {
            this.State = PlayStates.GameOver;
        }
    }

    public enum PlayStates
    {
        Play,
        GameOver,
        Quit
    }
}
