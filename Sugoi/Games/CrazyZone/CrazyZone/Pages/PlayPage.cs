using CrazyZone.Sprites;
using Sugoi.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrazyZone.Pages
{
    public class PlayPage : IPage, IScrollView
    {
        const string RETRY_TEXT = "Retry";
        const string GAMEOVER_TEXT = "Game Over";
        const string SCORE_TEXT = "Score: ";
        const string HISCORE_TEXT = "HiScore: ";

        private Game game;
        private Machine machine;
        private float scrollX;

        private int frameGameOver = 0;
        private int frameDucks = 0;
        private int frameFlies = 0;

        private int score;
        private int frameScore = 0;

        private int fontWidth;

        private int hiScore;
        private string hiScoreString;

        // hit de petit monstre
        private int hitSmallMonsterCount = 0;

        // hit de gros monstre
        private int hitBigMonsterCount = 0;

        private OpaSprite opa;

        private SpritePool<MotherSprite> mothers = new SpritePool<MotherSprite>(10);
        private SpritePool<AmmoSprite> ammos = new SpritePool<AmmoSprite>(10);
        private SpritePool<BombSprite> bombs = new SpritePool<BombSprite>(10);
        private SpritePool<BulletSprite> bullets = new SpritePool<BulletSprite>(40);
        private SpritePool<KaboomSprite> kabooms = new SpritePool<KaboomSprite>(100); // Kaboom 10
        private SpritePool<BabySprite> babies = new SpritePool<BabySprite>(60);
        private SpritePool<DuckSprite> ducks = new SpritePool<DuckSprite>(20);
        private SpritePool<FlySprite> flies = new SpritePool<FlySprite>(20);
        private SpritePool<CoinSprite> coins = new SpritePool<CoinSprite>(10);

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

        public SpritePool<CoinSprite> Coins
        {
            get
            {
                return this.coins;
            }
        }

        public int ScrollHeight
        {
            get
            {
                return 0;
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

            private set
            {
                this.scrollX = value;
            }
        }

        public float ScrollY
        {
            get
            {
                return 0;
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

            this.machine.Audio.PlayLoop("playSound");

            this.machine.Frame = 0;

            frameGameOver = 0;
            frameDucks = 0;
            frameFlies = 0;

            frameScore = 0;
            score = 0;

            fontWidth = machine.Screen.Font.FontSheet.TileWidth;

            hiScore = this.machine.BatteryRam.ReadInt(0x0000);
            hiScoreString = HISCORE_TEXT + hiScore;

            hitSmallMonsterCount = 0;
            hitBigMonsterCount = 0;

            this.maps = AssetStore.ParallaxMaps;
            
            this.ScrollWidth = this.maps[0].Width;
            this.ScrollX = 0;

            this.opa.Initialize();

            this.babies.Create(baby => baby.Create(machine, this));
            this.mothers.Create(mother => mother.Create(machine, this));
            this.ammos.Create( ammo => ammo.Create(machine, this));
            this.bombs.Create(bomb => bomb.Create(machine, this));
            this.kabooms.Create(kaboom => kaboom.Create(machine, this));
            this.ducks.Create(duck => duck.Create(machine, this));
            this.bullets.Create(bullet => bullet.Create(machine, this));
            this.flies.Create(fly => fly.Create(machine, this));
            this.coins.Create(coin => coin.Create(machine, this));

            this.mothers.GetFreeSprite().SetPosition(0, 8);
            this.mothers.GetFreeSprite().SetPosition(100, 110);
            this.mothers.GetFreeSprite().SetPosition(210, 60);
            this.mothers.GetFreeSprite().SetPosition(310, 80);
        }

        /// <summary>
        /// Mise à jour avant les waits
        /// </summary>

        public void Updating()
        {
            if (State != PlayStates.Quit)
            {
                if (opa.IsAlive && opa.IsDying == false)
                {
                    scrollX += opa.Speed;
                }

                if (frameDucks > (60 * 8))
                {
                    frameDucks = 0;

                    const int duckCount = 4;
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
                    this.flies.GetFreeSprite().Born(40, centerY);
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

                opa.Updated();
                ammos.Updated();

                // on place le setscroll ici car c'est Opa qui Fire la bomb et elle a besoin du scroll une fois tirée
                bombs.SetScroll((int)-scrollX, 0);
                bombs.Updated();

                mothers.Updated();
                babies.Updated();
                ducks.Updated();
                flies.Updated();
                bullets.Updated();

                mothers.CheckCollision(opa, CollisionStrategies.RectIntersect);
                babies.CheckCollision(opa, CollisionStrategies.RectIntersect);
                ducks.CheckCollision(opa, CollisionStrategies.RectIntersect);
                flies.CheckCollision(opa, CollisionStrategies.RectIntersect);
                bullets.CheckCollision(opa, CollisionStrategies.RectIntersect);
                coins.CheckCollision(opa, CollisionStrategies.RectIntersect);

                ammos.CheckCollision(mothers, CollisionStrategies.RectIntersect);
                ammos.CheckCollision(babies, CollisionStrategies.RectIntersect);
                ammos.CheckCollision(ducks, CollisionStrategies.RectIntersect);
                ammos.CheckCollision(flies, CollisionStrategies.RectIntersect);

                bombs.CheckCollision(mothers, CollisionStrategies.RectIntersect);
                bombs.CheckCollision(babies, CollisionStrategies.RectIntersect);
                bombs.CheckCollision(ducks, CollisionStrategies.RectIntersect);
                bombs.CheckCollision(flies, CollisionStrategies.RectIntersect);


                // Mise à jour des coins car appeler par les collisions
                coins.Updated();
                // Mise à jour de Kaboom à la fin car les collisions peuvent l'appeler
                kabooms.Updated();

                // Quit le GameOver au bout d'un certain temps si le player n'a pas appuyé sur son choix (retry, exit)
                if (State == PlayStates.GameOver)
                {
                    if (frameGameOver == 60 * 5)
                    {
                        // il ne passera qu'une seule fois
                        frameGameOver++;
                        this.machine.StopWait(false);

                        State = PlayStates.Quit;
                    }
                    else
                    {
                        frameGameOver++;
                    }
                }
            }
        }

            /// <summary>
            /// Mise à jour après les waits
            /// </summary>

        public void Updated()
        {
            if (State != PlayStates.Quit)
            {
                // Augmente le score toutes les secondes environs
                if (Opa.IsDying == false && Opa.IsAlive == true)
                {
                    if (frameScore > 60)
                    {
                        frameScore = 0;
                        score++;
                    }
                    else
                    {
                        frameScore++;
                    }
                }
            }

            switch (State)
            {
                case PlayStates.GameOver:

                    if (this.machine.Gamepad.IsButtonsPressed())
                    {
                        this.machine.Gamepad.WaitForRelease(() => State = PlayStates.Quit);
                    }

                    break;

                case PlayStates.Quit:

                    this.machine.Audio.Stop("playSound");

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
                screen.DrawScrollMap(maps[0], true, (int)(-scrollX * 0.50), 0, 0, 0, 320, 136);
                screen.DrawScrollMap(maps[1], true, (int)(-scrollX * 0.60), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
                screen.DrawScrollMap(maps[3], true, (int)(-scrollX * 0.70), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
                screen.DrawScrollMap(maps[2], true, (int)(-scrollX * 0.80), 0, 0, screen.Height - maps[2].Height, 320, 136);
                screen.DrawScrollMap(maps[4], true, (int)(-scrollX * 0.90), 0, 0, screen.Height - maps[4].Height, 320, 136);
                screen.DrawScrollMap(maps[5], true, (int)(-scrollX * 1.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

                ammos.Draw(frameExecuted);
                babies.Draw(frameExecuted);
                mothers.Draw(frameExecuted);

                ducks.Draw(frameExecuted);
                flies.Draw(frameExecuted);

                kabooms.Draw(frameExecuted);

                opa.Draw(frameExecuted);
                bombs.Draw(frameExecuted);
                bullets.Draw(frameExecuted);
                coins.Draw(frameExecuted);

                if (State == PlayStates.GameOver)
                {
                    screen.DrawText(GAMEOVER_TEXT, (screen.BoundsClipped.Width - (GAMEOVER_TEXT.Length * 8)) / 2, (screen.BoundsClipped.Height - 8) / 2);
                }

                // score
                screen.DrawText(SCORE_TEXT, screen.BoundsClipped.X + 4, 0);
                screen.DrawText(score, screen.BoundsClipped.X + SCORE_TEXT.Length * fontWidth, 0);
                // hi score
                screen.DrawText(hiScoreString, screen.BoundsClipped.Right - hiScoreString.Length * fontWidth - 4, 0);

#if DEBUG
                // scroll
                screen.DrawText((int)scrollX, 160, 0);
#endif
            }
            else
            {
                screen.Clear(Argb32.Black);
            }

            //screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
        }

        /// <summary>
        /// GameOver !
        /// </summary>

        public async void GameOver()
        {
            this.State = PlayStates.GameOver;

            if (this.hiScore < this.score)
            {
                this.machine.BatteryRam.WriteInt(0x0000, this.score);
                await this.machine.BatteryRam.FlashAsync();
            }
        }

        /// <summary>
        /// Ajoute un bnus
        /// </summary>
        /// <param name="bonus"></param>

        public void AddBonusScore(int bonus)
        {
            this.score += bonus;
        }

        /// <summary>
        /// Ajouter un hit de monstre (or mother) et donne une récompense eventuellement
        /// </summary>

        public void AddHitSmallMonster(int x, int y)
        {
            this.hitSmallMonsterCount++;

            bool hasReward = false;

            CoinTypes coinType = CoinTypes.Coin1;

            if (hitSmallMonsterCount != 0)
            {
                hasReward = this.hitSmallMonsterCount % 10 == 0;
            
                if(this.hitSmallMonsterCount % 100 == 0)
                {
                    hasReward = true;
                    coinType = CoinTypes.Coin5;
                }
            }

            if(hasReward)
            {
                this.coins.GetFreeSprite().Born(x, y, coinType);
            }
        }

        public void AddHitBigMonster(int x, int y)
        {
            this.hitBigMonsterCount++;

            var hasBigReward = (this.hitBigMonsterCount % 4) == 0;

            if (hasBigReward)
            {
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin5);
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin5);
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin5);

                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin1);
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin1);
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin1);
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin1);
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin1);
            }
            else
            {
                this.coins.GetFreeSprite().Born(x, y, CoinTypes.Coin5);
            }
        }
    }

    public enum PlayStates
    {
        Pause,
        Play,
        GameOver,
        Quit
    }
}
