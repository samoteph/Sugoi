using CrazyZone.Controls;
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

        private Menu menu;

        private string[] menuEntries = new string[]
        {
            MENU_LINE1,
            MENU_LINE2
        };

        private int hiScore;
        private string hiScoreString;

        private Game game;
        private Machine machine;

        private int fontWidth;

        private int frameScroll;

        private int menuPosition;

        private string[] credits_lines = new string[]
        {
            "code:",
            "samuel blanchard",
            "",
            "music:",
            "eric matyas",
            "www.soundimage.org",
            "",
            "redesigned logo:",
            "alessandra sada"
        };

        SurfaceSprite title;
        SurfaceTileSheet tiles;

        Map[] maps;
        Animator cursor;
 
        enum HomeStates
        {
            Home,
            Menu,
            P1OrP2,
            Credits,
            Quit,
        }

        /// <summary>
        /// Etat de la page
        /// </summary>

        HomeStates homeState;

        public HomePage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
            this.menu = new Menu();
            
            this.menu.MenuSelectedCallback = (menuPosition) =>
            {
                if (menuPosition == 0)
                {
                    this.machine.Audio.Play("startSound");
                    this.homeState = HomeStates.Quit;
                }
                else
                {
                    this.machine.Audio.Play("selectSound");

                    // clique sur Credits
                    this.homeState = HomeStates.Credits;
                }
            };

            this.menu.CursorMoveCallback = (menuPosition) =>
            {
                this.machine.Audio.Play("menuSound", 0.5, false);
            };

            this.menu.BackCallback = () =>
            {
                this.machine.Audio.Play("selectSound");
                homeState = HomeStates.Home;
            };
        }

        public void Initialize()
        {
            this.machine.Frame = 0;

            this.homeState = HomeStates.Home;

            this.title = AssetStore.Title;
            this.maps = AssetStore.ParallaxMaps;
            this.tiles = AssetStore.Tiles;

            cursor = AssetStore.OpaCursorAnimator;

            this.menu.Initialize(machine, cursor, menuEntries);

            menuPosition = 0;

            frameScroll = 0;

            hiScore = this.machine.BatteryRam.ReadInt(0x0000);
            hiScoreString = "hiscore: " + hiScore;

            this.fontWidth = this.machine.Screen.Font.FontSheet.TileWidth;

            this.machine.Audio.PlayLoop("homeSound");

            cursor.Start();
        }

        public void Updating()
        {
            var frame = this.machine.Frame;

            if (homeState == HomeStates.Menu)
            {
                cursor.Update();
            }

            frameScroll = (int)(frame * 0.5);
        }

        public void Updated()
        {
            var gamepad = this.machine.Gamepad;

            switch (homeState)
            {
                case HomeStates.Home:
                    
                    // detection du bouton Start        
                    if (gamepad.IsPressed(GamepadKeys.ButtonA))
                    {
                        this.machine.Audio.Play("selectSound");

                        gamepad.WaitForRelease(() =>
                        {
                            homeState = HomeStates.Menu;
                        });
                    }
                    break;

                case HomeStates.Menu:

                    menu.Update();
                    break;

                case HomeStates.Credits:

                    if (gamepad.IsButtonsPressed())
                    {
                        this.machine.Audio.Play("selectSound");

                        gamepad.WaitForRelease(() =>
                        {
                            this.menuPosition = 0;
                            this.homeState = HomeStates.Home;
                        });
                    }

                    break;

                case HomeStates.Quit:

                    this.machine.Audio.Stop("homeSound");

                    machine.WaitForFrame(30, () =>
                    {
                        game.Navigate(typeof(PlayPage));
                    });

                    break;
            }
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;
            var font = screen.Font;
            var frame = this.machine.Frame;

            //screen.Clear(new Argb32(0xeeeecc));

            screen.DrawScrollMap(maps[0], true, (int)(-frameScroll * 0.25), 0, 0, 0, 320, 136);
            screen.DrawScrollMap(maps[1], true, (int)(-frameScroll * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[3], true, (int)(-frameScroll * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[2], true, (int)(-frameScroll * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
            screen.DrawScrollMap(maps[4], true, (int)(-frameScroll * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);
            screen.DrawScrollMap(maps[5], true, (int)(-frameScroll * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

            // pas affiché dans les crédits
            if (homeState != HomeStates.Credits)
            {
                // Titre FantazyZone
                screen.DrawSprite(title, (screen.Width - title.Width) / 2, 24);
            }

            switch (homeState)
            {
                case HomeStates.Home:
                    screen.DrawText(TITLE_LINE1, (screen.Width - TITLE_LINE1.Length * font.FontSheet.TileWidth) / 2, 128);
                    screen.DrawText(TITLE_LINE2, (screen.Width - TITLE_LINE2.Length * font.FontSheet.TileWidth) / 2, 140);

                    if (frame % 100 < 50)
                    {
                        screen.DrawText(PRESS_START, (screen.Width - PRESS_START.Length * font.FontSheet.TileWidth) / 2, 158);
                    }

                    break;

                case HomeStates.Menu:

                    //var centerX = (screen.Width - MENU_LINE1.Length * font.FontSheet.TileWidth) / 2;

                    //screen.DrawText(MENU_LINE1, centerX, 128);
                    //screen.DrawText(MENU_LINE2, centerX, 140);

                    //cursor.Draw(screen, centerX - 24, (128 - 4) + menuPosition * 12, true, false);

                    menu.Draw(frameExecuted, 128);

                    break;

                case HomeStates.Credits:

                    int y = 50;

                    for (int i = 0; i < this.credits_lines.Length; i++)
                    {
                        var line = credits_lines[i];
                        screen.DrawText(line, (screen.Width - line.Length * font.FontSheet.TileWidth) / 2, y);
                        y += 8;
                    }

                    break;

                case HomeStates.Quit:

                    screen.Clear(Argb32.Black);
                    
                    break;
            }

            if(homeState != HomeStates.Quit)
            {
                // hi score
                screen.DrawText(hiScoreString, screen.BoundsClipped.Right - hiScoreString.Length * fontWidth - 4, 0);
            }

#if DEBUG
            screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
#endif
        }
    }
}
