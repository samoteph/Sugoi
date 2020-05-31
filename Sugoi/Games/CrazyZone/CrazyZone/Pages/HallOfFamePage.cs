using GameJolt;
using GameJolt.Objects;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZone.Pages
{
    public class HallOfFamePage : IPage
    {
        public const string LOADING_SCORE_TEXT = "Loading scores";

        Game game;
        Machine machine;
        Screen screen;
        Gamepad gamepad;

        float frameArrow = 0;

        float scrollX;
        float frameScroll;

        bool isMoving = false;
        int frameMoving = 0;
        int page = 0;
        int pageIndex = 0;
        int direction = -1;

        EasingPath path = new EasingPath();

        Map[] maps;
        MapText mapFameItems = new MapText();

        FameItem[] items = new FameItem[100];

        HallOfFameStates State
        {
            get;
            set;
        }

        public HallOfFamePage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
            this.screen = machine.Screen;
            this.gamepad = machine.GamepadGlobal;

            maps = AssetStore.ParallaxMaps;

            mapFameItems.Create("mapFame", 40 * 10, 10, AssetStore.Font, MapTileDescriptor.HiddenTile);

            path.Initialize(EasingFunctions.CircularEaseOut, EasingFunctions.Linear, screen.Width, 0, 1, 1, 30);

            int rank = 1;

            for(int i=0;i<items.Length;i++)
            {
                var item = new FameItem();

                item.Rank = rank++;

                items[i] = item;
            }

            this.DrawFameItems();
        }

        private void DrawFameItems()
        {
            for (int j = 0; j < 10  ; j++)
            {
                int pageIndex = j * 40;
                int itemIndex = j * 10;

                for (int i = 0; i < 10; i++)
                {
                    var item = items[i + itemIndex];

                    mapFameItems.SetText(10 + pageIndex, i, item.Rank);
                    mapFameItems.SetText(14 + pageIndex, i, item.Name);
                    mapFameItems.SetText(21 + pageIndex, i, item.Score);
                }
            }
        }

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

            await this.LoadScoresAsync();
        }

        private async Task LoadScoresAsync()
        {
            await this.machine.ExecuteAsync(async () =>
            {
                bool isLoaded = await this.game.Leaderboard.LoadScoresAsync();

                if (isLoaded)
                {
                    var leaderboardItems = this.game.Leaderboard.Items;

                    for (int i = 0; i < leaderboardItems.Count; i++)
                    {
                        var leaderboardItem = leaderboardItems[i];

                        var item = items[i];

                        item.Rank = i + 1;
                        item.Name = leaderboardItem.Name.ToCharArray();
                        item.Score = leaderboardItem.Score;
                    }

                    this.DrawFameItems();
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
                    this.game.NavigateWithFade(typeof(HomePage));
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
                        }

                        break;

                    case GamepadKeys.Right:

                        if (pageIndex < 9)
                        {
                            isMoving = true;
                            direction = 1;
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
                this.screen.DrawScrollMap(mapFameItems, false, (int)-scrollX, 8 * 8);

                frameArrow = (frameArrow + 0.2f) % 5;
                var arrowOffset = (int)frameArrow;

                if (pageIndex > 0)
                {
                    screen.DrawText('<', 2 * 8 - arrowOffset, (screen.Height / 2) - 4);
                }

                if (pageIndex < 9)
                {
                    screen.DrawText('>', 38 * 8 + arrowOffset, (screen.Height / 2) - 4);
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

        public char[] Name
        {
            get;
            set;
        } = new char[6];

        public int Score
        {
            get;
            set;
        }
    }
}
