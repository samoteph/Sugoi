using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace EmptyGame.Uwp.Cartridges
{
    public class DemoCartridge : ExecutableCartridge
    {
        private Machine machine;
        SurfaceSprite sprite;
        SurfaceTileSheet tileSheet;
        Animator animatorCoin;
        SurfaceSprite smallScreen;

        Map mapCoin;
        Map mapCoinSlice;

        bool isPressedA;
        bool isPressedB;

        double autoScrollX;
        double scrollX;
        double scrollY;

        int count;

        /// <summary>
        /// Load file integrated in application as Content in Assets folder
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>

        private async Task<Stream> LoadContentFile(string filename)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + filename));

            var stream = await file.OpenReadAsync();

            return stream.AsStream();
        }

        /// <summary>
        /// Start the console
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>

        public override async Task StartAsync(Machine machine)
        {
            this.machine = machine;

            // loading some assets and put them in the video memory of the console
            var assetSprite = AssetSprite.Import(this, "sprite", await LoadContentFile("Game/Sprite.png"));
            this.sprite = this.machine.VideoMemory.CreateSprite(assetSprite);

            // tilesheet is composed of tiles of 8x8 pixels
            // tilesheet is a sprite too 
            
            var assetTileSheet = AssetTileSheet.Import(this, "tilesheet", await LoadContentFile("Game/TileSheet.png"), 8, 8);
            this.tileSheet = this.machine.VideoMemory.CreateTileSheet(assetTileSheet);

            // Read the font file and create an AssetFontSheet
            var assetFont = AssetFontSheet.Import(this, "font", await LoadContentFile("Game/Font.png"), 8, 8, FontTypes.PolychromeStatic);
            // Copy the AssetFontSheet to the VideMemory
            var surfaceFont = machine.VideoMemory.CreateFontSheet(assetFont);

            // Get the mapping of the font based on FontSheet/surfaceFont
            Font font = new Font
            {
                FontSheet = surfaceFont
            };

            font.AddCharacters(CharactersGroups.AlphaUpperAndLower);
            font.AddCharacters(CharactersGroups.Numeric);
            font.AddCharacters(".,\"'?!@*#$%: ");
            font.CharacterIndex += 5; // Jump blank characters
            font.AddCharacters("()+-/=©<>~§"); //~ correspond to the save icon (disk), § to the heart icon
            font.UnknownTileNumber = font.GetTileNumber('$'); // by default an unknown character will be represent by a $

            // set the default font of the Screen
            this.machine.Screen.Font = font;

            // create map of a coin
            this.mapCoin = new Map();
            this.mapCoin.Create("mapCoin", 2, 2, tileSheet);
            this.mapCoin.SetTiles(110, 111, 120, 121);

            // create map of the slice of a coin
            this.mapCoinSlice = new Map();
            this.mapCoinSlice.Create("mapCoinSlice", 2, 2, tileSheet);
            this.mapCoinSlice.SetTiles(112, 113, 122, 123);

            // animator
            this.animatorCoin = new Animator();
            this.animatorCoin.Initialize(
                new MapAnimationFrame(mapCoin, 10), // change every 10 frames
                new MapAnimationFrame(mapCoinSlice, 10) // change every 10 frames
            );
            // launch the animator
            this.animatorCoin.Play();

            // small screen
            this.smallScreen = this.machine.VideoMemory.CreateEmptySprite("smallScreen", 50, 50);
            this.smallScreen.Font = font;

            machine.UpdatingCallback = Updating;
            machine.UpdatedCallback = Updated;
            // Method where the game renders one frame
            machine.DrawCallback = Draw;
        }

        /// <summary>
        /// Here you can update some values use in Draw method. Updating can be executed up to twice, depends if the computer is slow down or not. The  Updating is always executed. 
        /// </summary>

        private void Updating()
        {
            // move scroll
            autoScrollX += 0.5;

            // update animation of the coin
            this.animatorCoin.Update();

            var gamepad = this.machine.GamepadGlobal;

            switch (gamepad.HorizontalController)
            {
                case GamepadKeys.Right:
                    scrollX += 0.5;
                    break;
                case GamepadKeys.Left:
                    scrollX -= 0.5;
                    break;
            }

            switch (gamepad.VerticalController)
            {
                case GamepadKeys.Up:
                    scrollY -= 0.5;
                    break;
                case GamepadKeys.Down:
                    scrollY += 0.5;
                    break;
            }

            isPressedA = this.machine.GamepadGlobal.IsPressed(GamepadKeys.ButtonA);

            if (isPressedA)
            {
                count++;
            }
        }

        /// <summary>
        /// Here you can update some values use in Draw method. Updated can be executed up to twice, depends if the computer is slow down or not. The  Updated is NOT always executed.
        /// If a GamePad.WaitForRelease/Machine.WaitForFrame is used, Updated method just wait for its completion before restart in a normal way. 
        /// </summary>

        private void Updated()
        {
            //  
            if(this.isPressedB == false)
            {
                if(this.machine.GamepadGlobal.IsPressed(GamepadKeys.ButtonB))
                {
                    this.isPressedB = true;
                    count++;

                    // When WaitForRelease is launched the method Updated is not executed until the button B is released (Updating is a always executed)
                    this.machine.GamepadGlobal.WaitForRelease(GamepadKeys.ButtonB, () => { this.isPressedB = false; });
                }
            }
        }

        /// <summary>
        /// Render a frame
        /// </summary>

        private void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;

            screen.Clear(Argb32.Green);

            int x = 0;

            // Draw a sprite
            screen.DrawSprite(sprite, 0, 0);
            // Draw sprite with inversion
            screen.DrawSprite(sprite, 0, sprite.Height, true, true);

            x += sprite.Width;

            // Draw a tile from a tilesheet (a tile as a dimension of 8x8 pixels)
            screen.DrawTile(tileSheet, 0, x, 0);
            // Draw tile with inversion
            screen.DrawTile(tileSheet, 0, x, 8, true, true);
            x += 8;

            // Draw alternatively face and slide of coin to make s aimple animaation
            if (this.machine.Frame % 20 > 10)
            {
                // Draw a part of tilesheet as sprite (cause TileSheet is also a sprite)
                // This is fast then DrawSpriteMap
                screen.DrawSprite(tileSheet, x, 0, false, false, 16, 11 * 8, 16, 16);
            }
            else
            {
                // Draw a map that show a coin
                // You can change dynamically the tile in the mapcoin
                screen.DrawSpriteMap(mapCoin, x, 0);
            }

            x += 16;

            // Draw a rectangle as background of the scroll
            screen.DrawRectangle(x, 0, 50, 50, Argb32.Black);
            // Draw a map with infinite scrolling (the map is repeat)

            screen.DrawScrollMap(mapCoin, true, (int)autoScrollX, (int)autoScrollX, x, 0, 50, 50);
            x += 50;

            // text

            const string hello = "Hello!";
            screen.DrawText(hello, x, 0);
            x += hello.Length * 8;

            // animated coin
            screen.DrawAnimator(animatorCoin, x, 0);
            x += 16;

            // Draw a small screen 
            smallScreen.Clear(Argb32.Red);
            // draw a coin centered in smallscreen
            smallScreen.DrawScrollMap(mapCoin, true, (int)scrollX, (int)scrollY, 0, 0, 50, 50);
            smallScreen.DrawSprite(sprite, (smallScreen.Width - sprite.Width) / 2, (smallScreen.Height - sprite.Height) / 2, false, false, 0, 11 * 8, 16, 16);


            screen.DrawSprite(smallScreen, x, 0);
            x += 50;

            // clipping
            smallScreen.SetClip(new Rectangle(8, 8, 8 * 5, 34));
            smallScreen.DrawText("#move", 8, 8);

            screen.DrawSprite(smallScreen, x, 0);
            // end of clipping
            smallScreen.ResetClip();

            // Button A et B different way of pression
            if(this.isPressedA == true)
            {
                screen.DrawText("A pressed", 0, 50);
            }

            if (this.isPressedB == true)
            {
                screen.DrawText("B pressed and WaitForRelease", 0, 58);
            }

            screen.DrawText(count, 0, 66);
        }
    }
}
