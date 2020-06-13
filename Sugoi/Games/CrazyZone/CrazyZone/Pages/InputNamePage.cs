using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZone.Pages
{
    public class InputNamePage : IPage
    {
        const string ENTER_NAME_TEXT = "Enter your name";
        const string SAVING_SCORE_TEXT = "saving score and name";

        Game game;
        Machine machine;
        Screen screen;
        Map[] maps;
        int xScreen, yScreen;

        private Gamepad gamepad;

        char[,] glyphs;
        private double frameScroll;

        private Animator cursor;

        private int xGlyph = 0;
        private int yGlyph = 0;

        private int wGlyph;
        private int hGlyph;

        private char[] name;
        private int xName = 0;

        private InputNameStates State
        {
            get;
            set;
        } = InputNameStates.Input;

        /// <summary>
        /// Type de la page destination
        /// </summary>

        public Type TypeOfPageDestination
        {
            get;
            set;
        } = typeof(HomePage);

        public int Score { get; internal set; }

        public InputNamePage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
            this.screen = machine.Screen;
            this.gamepad = machine.GamepadGlobal;

            this.maps = AssetStore.ParallaxMaps;
            this.cursor = AssetStore.GlyphCursorAnimator;

            glyphs = new char[5, 8] { 
                {'A','B','C','D','E','F','G','H'},
                {'I','J','K','L','M','N','O','P'},
                {'Q','R','S','T','U','V','W','X'},
                {'Y','Z','0','1','2','3','4','5'},
                {'6','7','8','9','.','?','!','~'}
            };

            name = new char[6] { '-','-','-','-','-', '-' };

            var firstChar = machine.BatteryRam.ReadChar((int)BatteryRamAddress.Name);

            // un enregistrement a deja eu lieu
            if (firstChar != 0)
            {
                // on ecrase
                machine.BatteryRam.ReadCharArray((int)BatteryRamAddress.Name, name);
            }
            else
            {

            }
            

            wGlyph = glyphs.GetLength(1);
            hGlyph = glyphs.GetLength(0);

            xScreen = (screen.Bounds.Width - (wGlyph * 16)) / 2;
            yScreen = 8*9;
        }

        public void Initialize()
        {
            this.State = InputNameStates.Input;

            if (name[0] == '-')
            {
                // aucun enregistrement
                xGlyph = 0;
                yGlyph = 0;
            }
            else
            {
                // on place sur la disquette
                xGlyph = wGlyph - 1;
                yGlyph = hGlyph - 1;
                
                for(int x=0; x < name.Length; x++)
                {
                    if(name[x] != '-')
                    {
                        xName++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            this.cursor.Start();

            this.machine.Audio.PlayLoop("inputNameSound");
        }

        public void Updating()
        {
            frameScroll += 0.5;

            if (State == InputNameStates.Input)
            {
                cursor.Update();
            }
        }

        public async void Updated()
        {
            if(State == InputNameStates.Saving)
            {
                return;
            }

            if(gamepad.IsButtonsPressed)
            {
                if(gamepad.IsPressed(GamepadKeys.ButtonA))
                {
                    var glyph = glyphs[yGlyph, xGlyph];

                    // on presse l'enregistrement ?
                    if(glyph == '~')
                    {
                        if (name[0] != '-')
                        {
                            this.State = InputNameStates.Saving;

                            this.machine.Audio.Play("startSound");

                           this.Save((isSaved) =>
                           {
                               this.machine.Audio.Stop("inputNameSound");
                               this.game.NavigateWithFade(TypeOfPageDestination);
                           });
                        }
                        else
                        {
                            this.gamepad.WaitForRelease();
                        }

                        return;
                    }

                    name[xName] = glyph;
                    
                    if(xName < name.Length - 1)
                    {
                        xName++;
                    }

                    this.machine.Audio.Play("selectSound");
                    
                    this.gamepad.WaitForRelease();
                }
                else if(gamepad.IsPressed(GamepadKeys.ButtonB))
                {
                    if (xName == name.Length - 1 && name[xName] != '-')
                    {
                        name[xName] = '-';
                    }
                    else if (xName > 0)
                    {
                        xName--;
                        name[xName] = '-';
                    }

                    this.machine.Audio.Play("selectSound");

                    this.gamepad.WaitForRelease();
                }
            }
            else if (gamepad.IsControllerPressed == true)
            {
                // on ne veut pas de déplacement en diagonal

                this.machine.Audio.Play("menuSound");

                if (gamepad.IsPressed(GamepadKeys.Right))
                {
                    xGlyph = (xGlyph + 1) % wGlyph;
                }
                else if (gamepad.IsPressed(GamepadKeys.Left))
                {
                    if( (xGlyph - 1) < 0)
                    {
                        xGlyph = wGlyph - 1;                    
                    }
                    else
                    {
                        xGlyph--;
                    }
                }
                else if (gamepad.IsPressed(GamepadKeys.Up))
                {
                    if ((yGlyph - 1) < 0)
                    {
                        yGlyph = hGlyph - 1;
                    }
                    else
                    {
                        yGlyph--;
                    }
                }
                else if (gamepad.IsPressed(GamepadKeys.Down))
                {
                    yGlyph = (yGlyph + 1) % hGlyph;
                }

                this.gamepad.WaitForRelease(10);
            }
        }

        public void Draw(int frameExecuted)
        {
            screen.DrawScrollMap(maps[0], true, (int)(-frameScroll * 0.25), 0, 0, 0, 320, 136);
            screen.DrawScrollMap(maps[1], true, (int)(-frameScroll * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[3], true, (int)(-frameScroll * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[2], true, (int)(-frameScroll * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
            screen.DrawScrollMap(maps[4], true, (int)(-frameScroll * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);
            screen.DrawScrollMap(maps[5], true, (int)(-frameScroll * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

            for (int y = 0; y < hGlyph; y++)
            {
                for (int x = 0; x < wGlyph; x++)
                {
                    var glyph = glyphs[y, x];

                    screen.DrawText(glyph, xScreen + x * 16, yScreen + y * 16);                   
                }
            }

            cursor.Draw(screen, xScreen + (xGlyph * 16) - 8, yScreen + (yGlyph * 16) - 8);

            screen.DrawText(ENTER_NAME_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (ENTER_NAME_TEXT.Length * 8)) / 2), 8 * 3 );

            for (int i = 0; i < name.Length; i++)
            {
                var glyph = name[i];
                screen.DrawText(glyph, xScreen + i * 16 + 16, 8 * 6);
            }

            if (State == InputNameStates.Saving)
            {
                screen.DrawText(SAVING_SCORE_TEXT, screen.BoundsClipped.X + ((screen.BoundsClipped.Width - (SAVING_SCORE_TEXT.Length * 8)) / 2), 8 * 20);
            }
        }

        /// <summary>
        /// Sauvegarde du nom
        /// </summary>

        public async void Save( Action<bool> saveCompleted )
        {
            await this.machine.ExecuteAsync(async () =>
            {
                await this.game.SaveNameAndHisScoreAsync(name, Score, saveCompleted);

                if (Score > 0)
                {
                    this.State = InputNameStates.Saving;
                }
            });
        }

        /// <summary>
        /// Etat
        /// </summary>

        public enum InputNameStates
        {
            Input,
            Saving
        }
    }
}
