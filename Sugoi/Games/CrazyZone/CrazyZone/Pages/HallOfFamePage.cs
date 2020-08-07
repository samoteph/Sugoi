using GameJolt;
using GameJolt.Objects;
using Sugoi.Core;
using Sugoi.Core.IO;
using Sugoi.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZone.Pages
{
    public class HallOfFamePage : IPage
    {
        public const string LOADING_SCORE_ERROR_TEXT = "Loading error!";
        public const string LOADING_SCORE_TEXT = "Loading scores";
        public const string MANUAL_TEXT = "< or > to navigate";
        
        readonly CrazyZoneGame game;
        readonly Machine machine;
        readonly Screen screen;
        readonly Gamepad gamepad;

        float frameArrow = 0;

        float scrollX;
        float frameScroll;

        bool isMoving = false;
        int frameMoving = 0;
        int page = 0;
        int maxPage = 9;
        int pageIndex = 0;
        int direction = -1;

        readonly EasingPath path = new EasingPath();

        readonly Map[] maps;
        readonly MapText mapFameItems = new MapText();

        readonly FameItem[] items = new FameItem[100];

        HallOfFameStates State
        {
            get;
            set;
        }

        public HallOfFamePage()
        {
            this.game = GameService.Instance.GetGameSingleton<CrazyZoneGame>();
            this.machine = game.Machine;
            this.screen = machine.Screen;
            this.gamepad = machine.GamepadGlobal;

            maps = AssetStore.ParallaxMaps;

            mapFameItems.Create("mapFame", 40 * 10, 10, AssetStore.Font, MapTileDescriptor.HiddenTile);

            path.Initialize(EasingFunctions.CircularEaseOut, EasingFunctions.Linear, screen.Width, 0, 1, 1, 30);

            int rank = 1;

            for(int i=0;i<items.Length;i++)
            {
                var item = new FameItem
                {
                    Rank = rank++
                };

                items[i] = item;
            }
        }

        private void DrawFameItems()
        {
            for (int j = 0; j < maxPage  ; j++)
            {
                int pageIndex = j * 40;
                int itemIndex = j * 10;

                for (int i = 0; i < 10; i++)
                {
                    var item = items[i + itemIndex];

                    if (string.IsNullOrWhiteSpace(item.Name) == false)
                    {
                        // coeur qui marque son propre score
                        if(item.Name == name)
                        {
                            mapFameItems.SetText(9 + pageIndex, i, "§");
                            mapFameItems.SetText(31 + pageIndex, i, "§");
                        }
                        else
                        {
                            mapFameItems.SetTile(9 + pageIndex, i, MapTileDescriptor.HiddenTile);
                            mapFameItems.SetTile(31 + pageIndex, i, MapTileDescriptor.HiddenTile);
                        }

                        mapFameItems.SetText(12 + pageIndex, i, item.Rank, MapText.TextPositions.RightToLeft);
                        mapFameItems.SetText(19 + pageIndex, i, item.Name6Characters, MapText.TextPositions.RightToLeft);
                        mapFameItems.SetText(30 + pageIndex, i, item.Score, MapText.TextPositions.RightToLeft);
                    }
                }
            }
        }

        readonly char[] names = new char[6];
        string name;

        /// <summary>
        /// Initilize
        /// </summary>

        public async void Initialize()
        {
            frameMoving = 0;
            isMoving = false;
            scrollX = 0;
            page = 0;
            pageIndex = 0;

            State = HallOfFameStates.Loading;

            this.machine.Audio.PlayLoop("hallOfFameSound");

            machine.BatteryRam.ReadCharArray((int)BatteryRamAddress.Name, names);
            name = new string(names).Replace("-","");

            await this.machine.ExecuteAsync(() => this.game.SaveNameAndScoreIfNeededAsync());

            await this.LoadScoresAsync();
        }

        bool isScoreLoaded = false;

        private async Task LoadScoresAsync()
        {
            await this.machine.ExecuteAsync(async () =>
            {
                isScoreLoaded = false;

                bool isLoaded = await this.game.Leaderboard.LoadScoresAsync();

                if (isLoaded)
                {
                    var leaderboardItems = this.game.Leaderboard.Items;

                    for (int i = 0; i < leaderboardItems.Count; i++)
                    {
                        var leaderboardItem = leaderboardItems[i];

                        var item = items[i];

                        item.Rank = i + 1;
                        item.Name = leaderboardItem.Name;

                        if(item.Name.Length > 6)
                        {
                            item.Name = leaderboardItem.Name.Substring(0, 6);
                        }

                        item.Score = leaderboardItem.Score;
                    }

                    this.maxPage = ((leaderboardItems.Count - 1) / 10) + 1;

                    this.DrawFameItems();

                    isScoreLoaded = true;

                }

                State = HallOfFameStates.Play;
            });
        }

        /// <summary>
        /// Updating
        /// </summary>

        public void Updating()
        {
            frameScroll += 0.5f;

            if(State == HallOfFameStates.Loading)
            {
                return;
            }

            if(gamepad.IsPressed(GamepadKeys.ButtonA) || gamepad.IsPressed(GamepadKeys.ButtonB))
            {
                this.gamepad.WaitForRelease(30, () =>
                {
                    this.machine.Audio.Stop("hallOfFameSound");
                    this.machine.Audio.Play("selectSound");
                    this.game.Navigation.NavigateWithFade<HomePage>();
                });
            }

            if (isMoving == false)
            {
                var key = gamepad.HorizontalController;

                switch (key)
                {
                    case GamepadKeys.Left:

                        if (pageIndex > 0)
                        {
                            isMoving = true;
                            direction = -1;
                            this.machine.Audio.Play("menuSound");
                        }

                        break;

                    case GamepadKeys.Right:

                        if (pageIndex < this.maxPage - 1)
                        {
                            isMoving = true;
                            direction = 1;
                            this.machine.Audio.Play("menuSound");
                        }

                        break;
                }
            }
            else
            {
                if (frameMoving < path.MaximumFrame)
                {
                    this.path.GetPosition(frameMoving, out int offsetX, out int offsetY);
                    frameMoving++;

                    scrollX = page + offsetX * direction;
                }
                else
                {
                    frameMoving = 0;
                    isMoving = false;
                    page += this.path.Width * direction;
                    pageIndex += direction;
                }
            }
        }

        /// <summary>
        /// Updated
        /// </summary>

        public void Updated()
        {
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="frameExecuted"></param>

        public void Draw(int frameExecuted)
        {
            screen.DrawScrollMap(maps[0], true, (int)(-frameScroll * 0.25), 0, 0, 0, 320, 136);
            screen.DrawScrollMap(maps[1], true, (int)(-frameScroll * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[3], true, (int)(-frameScroll * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[2], true, (int)(-frameScroll * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
            screen.DrawScrollMap(maps[4], true, (int)(-frameScroll * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);
            screen.DrawScrollMap(maps[5], true, (int)(-frameScroll * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

            screen.DrawText("HALL OF FAME", 14 * 8, 3 * 8);
            screen.DrawText("------------", 14 * 8, 4 * 8);
            screen.DrawText("RANK", 9 * 8, 6 * 8);
            screen.DrawText("NAME", 16 * 8, 6 * 8);
            screen.DrawText("SCORE", 26 * 8, 6 * 8);

            if (State == HallOfFameStates.Loading)
            {
                screen.DrawText(LOADING_SCORE_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (LOADING_SCORE_TEXT.Length * 8)) / 2), 8 * (8 + 5));
            }
            else
            {
                if (isScoreLoaded)
                {
                    this.screen.DrawScrollMap(mapFameItems, false, (int)-scrollX, 0, 0, 8*8);

                    frameArrow = (frameArrow + 0.2f) % 5;
                    var arrowOffset = (int)frameArrow;

                    if (pageIndex > 0)
                    {
                        screen.DrawText('<', 2 * 8 - arrowOffset, (screen.Height / 2) - 4);
                    }

                    if (pageIndex < (maxPage - 1))
                    {
                        screen.DrawText('>', 38 * 8 + arrowOffset, (screen.Height / 2) - 4);
                    }

                    screen.DrawText(MANUAL_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (MANUAL_TEXT.Length * 8)) / 2), 8 * 20);
                }
                else
                {
                    screen.DrawText(LOADING_SCORE_ERROR_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (LOADING_SCORE_ERROR_TEXT.Length * 8)) / 2), 8 * (8 + 5));
                }
            }
        }
    }

    public enum HallOfFameStates
    {
        Loading,
        Play
    }

    /// <summary>
    /// FameItem
    /// </summary>

    public class FameItem
    {
        public int Rank
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                this.name = value;
                this.Name6Characters = value.PadLeft(6, ' ');
            }
        }

        private string name;

        public string Name6Characters
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }
    }
}
