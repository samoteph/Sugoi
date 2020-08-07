using CrazyZone.Controls;
using Sugoi.Core;
using Sugoi.Core.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrazyZone.Pages
{
    public class HomePage : IPage
    {
        private const string PRESS_START = "press button A";

        private const string TITLE_LINE1 = "programmed by ©samsoft";
        private const string TITLE_LINE2 = "for sugoi Virtual console";

        private const string MENU_LINE1 = "game start";
        private const string MENU_LINE2 = "credits";
        private const string MENU_LINE3 = "hall of fame";

        private const string MENU1POR2P_LINE1 = "1 player";
        private const string MENU1POR2P_LINE2 = "2 players";

        private Menu menuStart;
        private Menu menu1Por2P;

        private string[] menuEntries = new string[]
        {
            MENU_LINE1,
            MENU_LINE2,
            MENU_LINE3,
        };

        private string[] menu1Por2Pentries = new string[]
        {
            MENU1POR2P_LINE1,
            MENU1POR2P_LINE2
        };

        private int hiScore;
        private string hiScoreString;

        private CrazyZoneGame game;

        private int fontWidth;

        private int frameScroll;

        private string[] credits_lines = new string[]
        {
            "mini crazy zone for",
            "sugoi virtual console",
            "---------------------",
            "",
            "code:",
            "samuel blanchard",
            "",
            "music:",
            "Komiku",
            "medium.com/@Monplaisir",
            "",
            "logo:",
            "alessandra sada",
            "",
            "inspired by fantazy zone",
            "by ©sega",
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

        public Players Player
        {
            get;
            private set;
        }

        public Machine Machine
        {
            get;
            private set;
        }

        public HomePage()
        {
            this.game = GameService.Instance.GetGameSingleton<CrazyZoneGame>();

            this.Machine = game.Machine;
            this.menuStart = new Menu();
            this.menu1Por2P = new Menu();
            
            this.menuStart.MenuSelectedCallback = (menuPosition) =>
            {
                if (menuPosition == 0)
                {
                    this.Machine.Audio.Play("selectSound");
                    this.homeState = HomeStates.P1OrP2;
                }
                else if(menuPosition == 1)
                {
                    this.Machine.Audio.Play("selectSound");

                    // clique sur Credits
                    this.homeState = HomeStates.Credits;
                }
                else
                {
                    // clique sur Hall Of Fame
                    this.Machine.Audio.Stop("homeSound");
                    this.Machine.Audio.Play("startSound");

                    this.game.Navigation.NavigateWithFade<HallOfFamePage>();
                }
            };

            this.menuStart.BackCallback = () =>
            {
                this.Machine.Audio.Play("selectSound");
                homeState = HomeStates.Home;
            };

            this.menu1Por2P.MenuSelectedCallback = (menuPosition) =>
            {
                if (menuPosition == 0)
                {
                    this.Machine.Audio.Play("startSound");
                    this.homeState = HomeStates.Quit;
                    this.Player = Players.Solo;
                }
                else
                {
                    this.Machine.Audio.Play("startSound");
                    // != de Solo == 2 players
                    this.Player = Players.Player2;
                    this.homeState = HomeStates.Quit;
                }
            };

            this.menu1Por2P.BackCallback = () =>
            {
                this.Machine.Audio.Play("selectSound");
                homeState = HomeStates.Menu;
            };

            Action<int> handlerMove = (menuPosition) =>
            {
                this.Machine.Audio.Play("menuSound", 0.5, false);
            };

            this.menuStart.CursorMoveCallback = handlerMove;
            this.menu1Por2P.CursorMoveCallback = handlerMove;
        }

        public async void Initialize()
        {
            this.Machine.Frame = 0;

            this.homeState = HomeStates.Home;

            this.title = AssetStore.Title;
            this.maps = AssetStore.ParallaxMaps;
            this.tiles = AssetStore.Tiles;

            cursor = AssetStore.OpaCursorAnimator;

            this.menuStart.Initialize(Machine, cursor, menuEntries);
            this.menu1Por2P.Initialize(Machine, cursor, menu1Por2Pentries);

            frameScroll = 0;

            hiScore = this.Machine.BatteryRam.ReadInt((int)BatteryRamAddress.HiScore);
            hiScoreString = "hiscore: " + hiScore;

            this.fontWidth = this.Machine.Screen.Font.FontSheet.TileWidth;

            this.Machine.Audio.PlayLoop("homeSound");

            await this.Machine.ExecuteAsync(() => this.game.SaveNameAndScoreIfNeededAsync()); 

            cursor.Start();
        }

        public void Updating()
        {
            var frame = this.Machine.Frame;

            cursor.Update();

            frameScroll = (int)(frame * 0.5);
        }

        public void Updated()
        {
            var gamepad = this.Machine.GamepadGlobal;

            switch (homeState)
            {
                case HomeStates.Home:
                    
                    // detection du bouton Start        
                    if (gamepad.IsPressed(GamepadKeys.ButtonA))
                    {
                        this.Machine.Audio.Play("selectSound");

                        gamepad.WaitForRelease(() =>
                        {
                            homeState = HomeStates.Menu;
                        });
                    }
                    break;

                case HomeStates.Menu:

                    menuStart.Update();
                    break;

                case HomeStates.P1OrP2:

                    menu1Por2P.Update();
                    break;

                case HomeStates.Credits:

                    if (gamepad.IsButtonsPressed)
                    {
                        this.Machine.Audio.Play("selectSound");

                        gamepad.WaitForRelease(() =>
                        {
                            this.menuStart.MenuPosition = 0;
                            this.homeState = HomeStates.Home;
                        });
                    }

                    break;

                case HomeStates.Quit:

                    this.Machine.Audio.Stop("homeSound");

                    if (Player == Players.Solo)
                    {
                        game.Navigation.NavigateWithFade<PlayPage>();
                    }
                    else
                    {
                        game.Navigation.NavigateWithFade<MultiPlayPage>();
                    }

                    break;
            }
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.Machine.Screen;
            var font = screen.Font;
            var frame = this.Machine.Frame;

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
                    menuStart.Draw(frameExecuted, 128);
                    break;

                case HomeStates.P1OrP2:
                    menu1Por2P.Draw(frameExecuted, 128);
                    break;

                case HomeStates.Credits:

                    int y = (screen.Height - (credits_lines.Length * 8)) / 2;

                    for (int i = 0; i < this.credits_lines.Length; i++)
                    {
                        var line = credits_lines[i];
                        screen.DrawText(line, (screen.Width - line.Length * font.FontSheet.TileWidth) / 2, y);
                        y += 8;
                    }

                    break;
            }

            // hi score
            screen.DrawText(hiScoreString, screen.BoundsClipped.Right - hiScoreString.Length * fontWidth - 4, 0);

#if DEBUG
            screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
#endif
        }
    }
}
