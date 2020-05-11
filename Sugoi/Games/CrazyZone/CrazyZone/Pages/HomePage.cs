using Sugoi.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrazyZone.Pages
{
    public class HomePage : IPage
    {
        private const string PRESS_START = "press start";

        private const string TITLE_LINE1 = "original from ©sega";
        private const string TITLE_LINE2 = "programmed by ©samsoft";

        private const string MENU_LINE1 = "game start";
        private const string MENU_LINE2 = "credits";


        private Game game;
        private Machine machine;
        private int frame;

        private int frameCursor;
        private int menuPosition;
        private bool mustWaitForUnpress = false;

        SurfaceSprite title;
        SurfaceTileSheet tiles;

        Map[] maps;
        Map[] cursor;
 
        enum HomeStates
        {
            Start,
            Menu,
            P1OrP2,
            Options,
            Stop
        }

        /// <summary>
        /// Etat de la page
        /// </summary>

        HomeStates homeState;

        public HomePage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
        }

        public void Initialize()
        {
            frame = 0;

            mustWaitForUnpress = false;

            this.homeState = HomeStates.Start;

            this.title = AssetStore.Title;
            this.maps = AssetStore.ParallaxMaps;
            this.tiles = AssetStore.Tiles;

            cursor = new Map[2];
            
            Map map;

            map = new Map();
            map.Create("OpaCursor1", 2, 2, tiles, MapTileDescriptor.HiddenTile);
            map.SetTiles(182, 183, 192, 193);
            cursor[0] = map;

            map = new Map();
            map.Create("OpaCursor2", 2, 2, tiles, MapTileDescriptor.HiddenTile);
            map.SetTiles(184, 185, 194, 195);
            cursor[1] = map;

            menuPosition = 0;
        }

        public void Update()
        {
            var gamepad = this.machine.Gamepad;

            switch (homeState)
            {
                case HomeStates.Start:
                    
                    // detection du bouton Start        
                    if (gamepad.IsPressed(GamepadKeys.ButtonA))
                    {
                        Debug.WriteLine("Wait for release ButtonA Start");

                        gamepad.WaitForRelease(() =>
                        {
                            homeState = HomeStates.Menu;
                        });
                    }
                    break;

                case HomeStates.Menu:

                    // retour en arrière        
                    if (gamepad.IsPressed(GamepadKeys.ButtonB))
                    {
                        gamepad.WaitForRelease(() =>
                       {
                           homeState = HomeStates.Start;
                       });
                    }

                    if (gamepad.VerticalController == GamepadKeys.Down)
                    {
                        menuPosition = (menuPosition + 1) % 2;
                        gamepad.WaitForRelease();
                    }
                    else if(gamepad.VerticalController == GamepadKeys.Up)
                    {
                        var value = menuPosition - 1;

                        if (value < 0)
                        {
                            menuPosition = -(value % 2);
                        }
                        else
                        {
                            menuPosition = (menuPosition + 1) % 2;
                        }

                        gamepad.WaitForRelease();
                    }

                    if (gamepad.IsPressed(GamepadKeys.ButtonA))
                    {
                        gamepad.WaitForRelease(() =>
                        {
                            machine.WaitForFrame(100, () =>
                            {
                                game.Navigate(typeof(PlayPage));
                            });
                        });
                    }

                    break;
            }
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;
            var font = screen.Font;

            screen.Clear(new Argb32(0xeeeecc));

            frame += frameExecuted;

            int frameScroll = (int)(frame * 0.5);

            screen.DrawScrollMap(maps[0], true, (int)(-frameScroll * 0.25), 0, 0, screen.Height - maps[0].Height - 32, 320, 136);
            screen.DrawScrollMap(maps[1], true, (int)(-frameScroll * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[3], true, (int)(-frameScroll * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[2], true, (int)(-frameScroll * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
            screen.DrawScrollMap(maps[4], true, (int)(-frameScroll * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);
            screen.DrawScrollMap(maps[5], true, (int)(-frameScroll * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

            screen.DrawSprite(title, (screen.Width - title.Width) / 2, 16);

            switch (homeState)
            {
                case HomeStates.Start:
                    screen.DrawText(TITLE_LINE1, (screen.Width - TITLE_LINE1.Length * font.FontSheet.TileWidth) / 2, 120);
                    screen.DrawText(TITLE_LINE2, (screen.Width - TITLE_LINE2.Length * font.FontSheet.TileWidth) / 2, 132);

                    if (frame % 100 < 50)
                    {
                        screen.DrawText(PRESS_START, (screen.Width - PRESS_START.Length * font.FontSheet.TileWidth) / 2, 150);
                    }

                    break;

                case HomeStates.Menu:

                    var centerX = (screen.Width - MENU_LINE1.Length * font.FontSheet.TileWidth) / 2;

                    screen.DrawText(MENU_LINE1, centerX, 120);
                    screen.DrawText(MENU_LINE2, centerX, 132);

                    frameCursor = (frame % 30) < 15 ? 0 : 1;

                    screen.DrawSpriteMap(cursor[frameCursor], centerX - 24, (120 - 4) + menuPosition * 12, true, false);

                    break;
            }

            screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
        }
    }
}
