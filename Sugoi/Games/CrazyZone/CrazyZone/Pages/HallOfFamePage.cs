using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CrazyZone.Pages
{
    public class HallOfFamePage : IPage
    {
        Game game;
        Machine machine;
        Screen screen;
        Gamepad gamepad;

        float scrollY;
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

        public HallOfFamePage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
            this.screen = machine.Screen;
            this.gamepad = machine.GamepadGlobal;

            maps = AssetStore.ParallaxMaps;

            mapFameItems.Create("mapFame", 40, 108, AssetStore.Font, MapTileDescriptor.HiddenTile);

            mapFameItems.SetText(14, 3, "HALL OF FAME");
            mapFameItems.SetText(14, 4, "------------");
            mapFameItems.SetText(9, 6, "RANK");
            mapFameItems.SetText(16, 6, "NAME");
            mapFameItems.SetText(26, 6, "SCORE");

            path.Initialize(EasingFunctions.Linear, EasingFunctions.CircularEaseOut, 0, screen.Height - 4, 1, 1, 30);

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
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                mapFameItems.SetText(10, 8 + i, item.Rank);
                mapFameItems.SetText(14, 8 + i, item.Name);
                mapFameItems.SetText(21, 8 + i, item.Score);
            }
        }

        /// <summary>
        /// Initilize
        /// </summary>

        public void Initialize()
        {
            frameMoving = 0;
            isMoving = false;
            scrollY = 0;
            page = 0;
            pageIndex = 0;

        }

        /// <summary>
        /// Updating
        /// </summary>

        public void Updating()
        {
            frameScroll += 0.5f;

            if (isMoving == false)
            {
                var key = gamepad.VerticalController;

                switch (key)
                {
                    case GamepadKeys.Down:

                        if (pageIndex > 0)
                        {
                            isMoving = true;
                            direction = -1;
                        }

                        break;

                    case GamepadKeys.Up:

                        if (pageIndex < 4)
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

                    scrollY = page + offsetY * direction;
                }
                else
                {
                    frameMoving = 0;
                    isMoving = false;
                    page += this.path.Height * direction;
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

            this.screen.DrawScrollMap(mapFameItems, false, 0, (int)-scrollY);
        }
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
