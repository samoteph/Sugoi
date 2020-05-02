using Sugoi.Console.Controls;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sugoi.Console
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SurfaceTileSheet spriteTiles;
        SurfaceSprite spriteMonkey;
        Map map;

        public MainPage()
        {
            this.InitializeComponent();
            this.SugoiControl.Loaded += OnSugoiLoaded;
        }

        /// <summary>
        /// Console prête à fonctionner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void OnSugoiLoaded(object sender, RoutedEventArgs e)
        {
            var cartridge = new Cartridge();

            var assetTiles = await AssetTools.LoadTileSheetFromApplicationAsync("ms-appx:///Assets/Images/atlas.png","tiles",8,8);
            var assetMonkey = await AssetTools.LoadSpriteFromApplicationAsync("ms-appx:///Assets/Images/monkey.png","monkey");

            // normalement cartridge doit copier dans VideoMemory tous les Assets image au démarrage ou alors ils seront chargé à la demande
            //cartridge.Import(assetTiles);
            //cartridge.Import(assetMonkey);
            //var assetTiles = cartridge.GetAsset<AssetTileSheet>("tiles");

            // DEMARRAGE
            this.SugoiControl.Start(cartridge);

            var videoMemory = this.SugoiControl.VideoMemory;

            this.spriteTiles = videoMemory.CreateTileSheet(assetTiles);
            this.spriteMonkey = videoMemory.CreateSprite(assetMonkey);

            this.map = new Map();

            map.Create(10, 10, spriteTiles, new MapTileDescriptor(10));

            map[0, 0] = new MapTileDescriptor(20);
            map[1, 0] = new MapTileDescriptor(4) { isVerticalFlipped = true, isHorizontalFlipped = true };
            map[2, 0] = new MapTileDescriptor(8);
            map[0, 1] = new MapTileDescriptor(2);
            map[1, 1] = new MapTileDescriptor(3) { isVerticalFlipped = true, isHorizontalFlipped = true };
            map[2, 1] = new MapTileDescriptor(5);

            this.SugoiControl.FrameDrawn += OnFrameDrawn;
            this.SugoiControl.FrameUpdated += OnFrameUpdate;
        }

        int sprX;
        int sprY;

        /// <summary>
        /// Mise à jour de la frame
        /// </summary>

        private void OnFrameUpdate()
        {
            var gamepad = this.SugoiControl.Gamepad;

            var hc = gamepad.HorizontalController;

            if (hc == GamepadKeys.Right)
            {
                sprX++;
            }
            else if (hc == GamepadKeys.Left)
            {
                sprX--;
            }

            var vc = gamepad.VerticalController;

            if (vc == GamepadKeys.Up)
            {
                sprY++;
            }
            else if (vc == GamepadKeys.Down)
            {
                sprY--;
            }
        }

        /// <summary>
        /// Chargmeent d'un asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="import"></param>
        /// <returns></returns>

        private void OnFrameDrawn()
        {
            var sprx = this.sprX;
            var spry = this.sprY; 
            var screen = this.SugoiControl.Screen;

            screen.ClearClip();

            screen.Clear(Argb32.White);
            //map[0, 0] = map[0, 0].Flip(isFlipHChecked, isFlipVChecked);

            if (flags[0] || flags[1])
            {
                if (flags[3])
                {
                    screen.DrawSprite(spriteMonkey, sprX, sprY, flags[5], flags[6]);
                }

                if (flags[4])
                {
                    screen.DrawSpriteMap(map, 0, 0, flags[5], flags[6]);
                    screen.DrawSpriteMap(map, sprX, sprY, flags[5], flags[6]);
                }

                if (flags[0])
                {
                    screen.Clip = new Rectangle(20, 20, 6, 6);
                }
                else
                {
                    // plus grand que Monkey
                    screen.Clip = new Rectangle(20, 20, 150, 40);
                }

                screen.Clear(Argb32.Black);
            }

            //screen.DrawRectangle(sprx, spry, screen.Width, screen.Height, Argb32.Red);
            //screen.DrawRectangle(0, 0, 8 * 5, 8 * 5, Argb32.Green);

            //if (flags[1])
            //{
            //    screen.DrawSpriteMap(map, sprx, spry, flags[2], flags[3]);
            //}

            if(flags[4])
            {
                screen.DrawSpriteMap(map, sprX, sprY, flags[5], flags[6]);
            }

            if (flags[3])
            {
                screen.DrawRectangle(sprX, sprY, spriteMonkey.Width, spriteMonkey.Height, Argb32.Blue, true);
                screen.DrawSprite(spriteMonkey, sprX, sprY, flags[5], flags[6]);
            }

            screen.ClearClip();

            //screen.SetPixel(sprx, spry, Argb32.Green);
            //screen.DrawRectangle(sprX, sprY, spriteMonkey.Width, spriteMonkey.Height, Argb32.Red, false);

            //screen.DrawSprite(spriteTiles, sprX, sprY, false, false);
        }

        /// <summary>
        /// Click sur le checkbox Flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFlagClick(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if( int.TryParse((string)checkbox.Tag, out var flagNumber))
            {
                flags[flagNumber] = checkbox.IsChecked.Value;
            }

        }

        private bool[] flags = new bool[7];

        private void OnActionClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (int.TryParse((string)button.Tag, out var actionNumber))
            {
                switch(actionNumber)
                {
                    case 0:
                        this.sprX = 0;
                        this.sprY = 0;
                        break;
                    case 1:
                        this.sprX--;
                        break;
                    case 2:
                        this.sprX++;
                        break;
                }
            }
        }
    }
}
