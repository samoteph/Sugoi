using CrazyZone.Controls;
using CrazyZone.Sprites;
using Sugoi.Core;
using Sugoi.Core.Navigation;
using System;
using System.Diagnostics;

namespace CrazyZone.Pages
{
    public class PlayPage : INavigationPage, IScrollView
    {
        //const string RETRY_TEXT = "Retry";
        const string GAMEOVER_TEXT = "Game Over";
        const string SCORE_TEXT = "Score: ";
        const string HISCORE_TEXT = "HiScore: ";
        const string GAMEOVER_HISCORE_TEXT = "New Hiscore!";

        const string MULTI_PRESS_START_TEXT = "Press start";
        const string MULTI_WAITING_FOR_P1_TEXT = "Waiting for p1";
        const string MULTI_WAITING_FOR_P2_TEXT = "Waiting for p2";

        const string MULTI_WIN_TEXT = "YOU WIN!";
        const string MULTI_LOOSE_TEXT = "YOU LOOSE!";

        readonly private CrazyZoneGame game;
        readonly private Machine machine;
        private Gamepad gamepad;

        // nom du joueur
        readonly private char[] name = new char[6];

        //readonly private Menu menuRetry = new Menu();

        private int frameGameOver = 0;
        private int frameDucks = 0;
        private int frameFlies = 0;

        public int Score
        {
            get;
            private set;
        }

        public string scoreString;

        private int frameScore = 0;

        private int fontWidth;

        private int hiScore;
        private string hiScoreString;
        private int frameHiScore;

        // hit de petit monstre
        private int hitSmallMonsterCount = 0;

        // hit de gros monstre
        private int hitBigMonsterCount = 0;

        readonly private OpaSprite opa;

        // page accueillant le multi joueur
        private MultiPlayPage multiPage;
        // etat du jeu en mode multi pour l'occurence PlayPage
        
        public MultiPlayStates MultiPlayState
        {
            get;
            set;
        }

        readonly private SpritePool<MotherSprite> mothers = new SpritePool<MotherSprite>(10);
        readonly private SpritePool<AmmoSprite> ammos = new SpritePool<AmmoSprite>(10);
        readonly private SpritePool<BombSprite> bombs = new SpritePool<BombSprite>(10);
        readonly private SpritePool<BulletSprite> bullets = new SpritePool<BulletSprite>(40);
        readonly private SpritePool<KaboomSprite> kabooms = new SpritePool<KaboomSprite>(100); // Kaboom 10
        readonly private SpritePool<BabySprite> babies = new SpritePool<BabySprite>(60);
        readonly private SpritePool<DuckSprite> ducks = new SpritePool<DuckSprite>(20);
        readonly private SpritePool<FlySprite> flies = new SpritePool<FlySprite>(20);
        readonly private SpritePool<CoinSprite> coins = new SpritePool<CoinSprite>(10);

        public Gamepad Gamepad
        {
            get
            {
                return this.gamepad;
            }
        }

        public Players Player
        {
            get;
            private set;
        } = Players.Solo;

        public PlayStates State
        {
            get;
            set;
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

#region ScrollView

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

        public float ScrollX { get; set; }

        public float ScrollY
        {
            get
            {
                return 0;
            }
        }

#endregion

        public PlayPage()
        {
            this.game = GameService.Instance.GetGameSingleton<CrazyZoneGame>();
            this.machine = game.Machine;
            this.opa = new OpaSprite(machine, this);
        }

        /// <summary>
        /// intitialize le jeu à plusieurs
        /// </summary>
        /// <param name="player"></param>

        public void Initialize(MultiPlayPage multiPage, Players player)
        {
            this.Player = player;
            this.multiPage = multiPage;

            this.Initialize();
        }

        /// <summary>
        /// initialisation du jeu en mode Solo
        /// </summary>

        public void Initialize()
        {
            gamepad = this.machine.GamepadGlobal;

            // Lecture du nom du joueur dans la Ram
            this.machine.BatteryRam.ReadCharArray((int)BatteryRamAddress.Name, name);

            if (Player == Players.Solo)
            {
                State = PlayStates.Play;
                this.machine.Audio.PlayLoop("playSound");
            }
            else
            {
                State = PlayStates.ChooseGamepadP1andP2;

                this.machine.Audio.PlayLoop("waitForP2Sound");

                if (Player ==  Players.Player1 )
                {
                    MultiPlayState = MultiPlayStates.WaitStart;
                }
                else
                {
                    MultiPlayState = MultiPlayStates.WaitOtherPlayer;
                }
            }

            this.machine.Frame = 0;

            frameGameOver = 0;
            frameDucks = 0;
            frameFlies = 0;

            frameScore = 0;
            Score = 0;

            frameHiScore = 0;

            fontWidth = machine.Screen.Font.FontSheet.TileWidth;

            hiScore = this.machine.BatteryRam.ReadInt((int)BatteryRamAddress.HiScore);
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
            // attente que les deux players soientt prêt
            if(State == PlayStates.ChooseGamepadP1andP2)
            {
                // on continue a faire un scroll lent mais on affiche aucun autre sprte a part le texte "press start" et waiting for p2"
                ScrollX += 0.5f;

                return;
            }

            if (State != PlayStates.Quit)
            {
                if (opa.IsAlive && opa.IsDying == false)
                {
                    ScrollX += opa.Speed;
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

                // il y a un ordre dans les Updated
                // selon que les sprites soient créees par d'autres sprites : par exemple bomb est crée par opa, baby par mothers, ...
                opa.Updated();

                ammos.Updated();
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

                if (this.Player == Players.Solo || this.multiPage.State == MultiPlayPage.MultiStates.GameOver)
                {
                    if (State == PlayStates.GameOver)
                    {
                        if (frameGameOver == 60 * 20)
                        {
                            // il ne passera qu'une seule fois
                            frameGameOver++;
                            this.machine.StopWait(false);

                            this.Quit();
                        }
                        else
                        {
                            frameGameOver++;
                        }
                    }
                }
            }
        }

            /// <summary>
            /// Mise à jour après les waits
            /// </summary>

        public void Updated()
        {
            if (State == PlayStates.ChooseGamepadP1andP2)
            {
                if (MultiPlayState == MultiPlayStates.WaitStart)
                {
                    // Player1 quitte car ne veut pas commencer la partie à 2
                    if (Player == Players.Player1)
                    {
                        if (machine.GamepadGlobal.IsPressed(GamepadKeys.ButtonB))
                        {
                            this.machine.Audio.Stop("waitForP2Sound");
                            this.machine.Audio.Play("selectSound");

                            this.machine.GamepadGlobal.WaitForRelease(() =>
                            {
                                this.Quit();
                            });
                        }
                    }

                    machine.Gamepads.FindGamepadByKeyPressed(GamepadKeys.ButtonA, (g) =>
                    {
                        if (multiPage.FirstGamepad != g)
                        {
                            // Player 1
                            if (multiPage.FirstGamepad == null)
                            {
                                multiPage.SelectFirstGamepad(g);
                                MultiPlayState = MultiPlayStates.WaitOtherPlayer;
                                this.machine.Audio.Play("startSound");
                            }
                            // player2
                            else
                            {
                                this.machine.Audio.Stop("waitForP2Sound");
                                this.machine.Audio.Play("startSound");
                                multiPage.ReadyToPlay();
                            }

                            this.gamepad = g;

                            return true;
                        }

                        return false;
                    });
                }
                else if (MultiPlayState == MultiPlayStates.WaitOtherPlayer)
                {
                    if (Player == Players.Player1)
                    {
                        // demande de retour
                        if (this.machine.GamepadGlobal.IsPressed(GamepadKeys.ButtonB))
                        {
                            this.machine.Audio.Play("selectSound");

                            this.machine.GamepadGlobal.WaitForRelease(() =>
                            {
                                this.gamepad = machine.GamepadGlobal;
                                this.multiPage.UnselectFirstGamepad();
                                MultiPlayState = MultiPlayStates.WaitStart;
                            });
                        }
                    }
                }

                return;
            }

            if (State != PlayStates.Quit)
            {
                // Augmente le score toutes les secondes environs
                if (Opa.IsDying == false && Opa.IsAlive == true)
                {
                    if (frameScore > 60)
                    {
                        frameScore = 0;
                        Score++;
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

                    if (this.Player == Players.Solo || this.multiPage.State == MultiPlayPage.MultiStates.GameOver)
                    {
                        if (gamepad.IsButtonsPressed)
                        {
                            gamepad.WaitForRelease(() => this.Quit() );
                        }
                    }

                    break;

                case PlayStates.Quit:

                    if (Player != Players.Player2)
                    {
                        this.machine.Audio.Stop("playSound");

                        machine.WaitForFrame(30, () =>
                        {
                            if (Score <= hiScore)
                            {
                                game.Navigation.Navigate<HomePage>();
                            }
                            else
                            {
                                if (Player == Players.Solo)
                                {
                                    var page = (InputNamePage)game.Navigation.Navigate<InputNamePage>();
                                    page.Score = this.Score;
                                    page.TypeOfPageDestination = typeof(HallOfFamePage);
                                }
                                else
                                {
                                    game.Navigation.Navigate<HomePage>();
                                }
                            }
                        });
                    }
                    break;
            }
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;

            if (State != PlayStates.Quit)
            {
                screen.DrawScrollMap(maps[0], true, (int)(-ScrollX * 0.50), 0, 0, 0, 320, 136);
                screen.DrawScrollMap(maps[1], true, (int)(-ScrollX * 0.60), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
                screen.DrawScrollMap(maps[3], true, (int)(-ScrollX * 0.70), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
                screen.DrawScrollMap(maps[2], true, (int)(-ScrollX * 0.80), 0, 0, screen.Height - maps[2].Height, 320, 136);
                screen.DrawScrollMap(maps[4], true, (int)(-ScrollX * 0.90), 0, 0, screen.Height - maps[4].Height, 320, 136);
                screen.DrawScrollMap(maps[5], true, (int)(-ScrollX * 1.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

                if (State != PlayStates.ChooseGamepadP1andP2)
                {
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
                }

                switch(State)
                {
                    case PlayStates.GameOver:

                        var yCenter = (screen.BoundsClipped.Height - 8) / 2;

                        screen.DrawText(GAMEOVER_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (GAMEOVER_TEXT.Length * 8)) / 2), yCenter);
                        
                        if(Player != Players.Solo)
                        {
                            if (this.multiPage.State == MultiPlayPage.MultiStates.GameOver)
                            {
                                if (Score < this.multiPage.GetOpponentScore(this.Player))
                                {
                                    screen.DrawText(MULTI_LOOSE_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MULTI_LOOSE_TEXT.Length * 8)) / 2), yCenter + 16);
                                }
                                else
                                {
                                    screen.DrawText(MULTI_WIN_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MULTI_WIN_TEXT.Length * 8)) / 2), yCenter + 16);
                                }
                            }
                        }
                        else
                        {
                            // en solo seulement
                            if(Score > this.hiScore)
                            {
                                if (frameHiScore < 30)
                                {
                                    screen.DrawText(GAMEOVER_HISCORE_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (GAMEOVER_HISCORE_TEXT.Length * 8)) / 2), yCenter + 16);
                                    screen.DrawText(scoreString, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (scoreString.Length * 8)) / 2), yCenter + 24);
                                }

                                frameHiScore = (frameHiScore + 1) % 60;
                            }
                        }

                        break;

                    case PlayStates.ChooseGamepadP1andP2:

                        if (Player == Players.Player1)
                        {
                            switch (MultiPlayState)
                            {
                                case MultiPlayStates.WaitStart:
                                    screen.DrawText(MULTI_PRESS_START_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MULTI_PRESS_START_TEXT.Length * 8)) / 2), (screen.BoundsClipped.Height - 8) / 2);
                                    break;
                                case MultiPlayStates.WaitOtherPlayer:
                                    screen.DrawText(MULTI_WAITING_FOR_P2_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MULTI_WAITING_FOR_P2_TEXT.Length * 8)) / 2), (screen.BoundsClipped.Height - 8) / 2);
                                    break;
                            }
                        }
                        else if(Player == Players.Player2)
                        {
                            switch (MultiPlayState)
                            {
                                case MultiPlayStates.WaitStart:
                                    screen.DrawText(MULTI_PRESS_START_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MULTI_PRESS_START_TEXT.Length * 8)) / 2), (screen.BoundsClipped.Height - 8) / 2);
                                    break;
                                case MultiPlayStates.WaitOtherPlayer:
                                    screen.DrawText(MULTI_WAITING_FOR_P1_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MULTI_WAITING_FOR_P2_TEXT.Length * 8)) / 2), (screen.BoundsClipped.Height - 8) / 2);
                                    break;
                            }
                        }

                        break;
                }

                // score
                screen.DrawText(SCORE_TEXT, screen.BoundsClipped.X + 4, 0);
                screen.DrawText(Score, screen.BoundsClipped.X + SCORE_TEXT.Length * fontWidth, 0);

                if (Player == Players.Solo)
                {
                    // hi score
                    screen.DrawText(hiScoreString, screen.BoundsClipped.Right - hiScoreString.Length * fontWidth - 4, 0);

#if DEBUG
                    // scroll
                    screen.DrawText((int)ScrollX, 160, 0);
#endif
                }
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

            if (this.Player == Players.Solo)
            {
                if (this.hiScore < this.Score)
                {
                    this.machine.Audio.Play("winSound");

                    this.scoreString = "§ " + this.Score.ToString() + " §";
                    this.machine.BatteryRam.WriteInt((int)BatteryRamAddress.HiScore, this.Score);
                    await this.machine.BatteryRam.FlashAsync();
                }
            }
            else
            {
                this.multiPage?.GameOver();
            }
        }

        /// <summary>
        /// On s'en va
        /// </summary>

        public void Quit()
        {
            if (this.State != PlayStates.Quit)
            {
                this.State = PlayStates.Quit;
                this.multiPage?.Quit();
            }
        }

        /// <summary>
        /// Ajoute un bnus
        /// </summary>
        /// <param name="bonus"></param>

        public void AddBonusScore(int bonus)
        {
            this.Score += bonus;
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

        public bool Navigate(NavigationStates state, object parameter)
        {
            return true;
        }
    }

    public enum PlayStates
    {
        Pause,
        Play,
        GameOver,
        Quit,
        ChooseGamepadP1andP2
    }

    /// <summary>
    /// Joueur en train de jouer pour le jeu à plusieurs. Solo represente le player en mode Solo
    /// </summary>

    public enum Players
    {
        Solo = -1,
        Player1,
        Player2
    }

    public enum MultiPlayStates
    {
        WaitStart,
        WaitOtherPlayer,
        StartPlaying,
    }
}
