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
        Map mapBack;

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
            cartridge.Import(assetTiles);
            cartridge.Import(assetMonkey);

            this.SugoiControl.Initialized += OnInitialized;
            this.SugoiControl.FrameDrawn += OnFrameDrawn;
            this.SugoiControl.FrameUpdated += OnFrameUpdate;

            // DEMARRAGE
            this.SugoiControl.Start(cartridge);
        }

        int sprX;
        int speedX;
        
        int sprY;
        int speedY;

        int sprZ = 44;

        /// <summary>
        /// Initialisation
        /// </summary>

        private void OnInitialized()
        {
            var videoMemory = this.SugoiControl.VideoMemory;
            var cartridge = this.SugoiControl.Cartridge;

            var assetTiles = cartridge.GetAsset<AssetTileSheet>("tiles");
            var assetMonkey = cartridge.GetAsset<AssetSprite>("monkey");

            this.spriteTiles = videoMemory.CreateTileSheet(assetTiles);
            this.spriteMonkey = videoMemory.CreateSprite(assetMonkey);

            this.map = new Map();

            map.Create(30*2, 17*2, spriteTiles, new MapTileDescriptor(1));

            map.SetTile(0,  0, new MapTileDescriptor(20));
            map.SetTile(1,  0, new MapTileDescriptor(4) { isVerticalFlipped = true, isHorizontalFlipped = true });
            map.SetTile(2,  0, new MapTileDescriptor(8));
            map.SetTile(0,  1, new MapTileDescriptor(2));
            map.SetTile(1,  1, new MapTileDescriptor(3) { isVerticalFlipped = true, isHorizontalFlipped = true });
            map.SetTile(2,  1, new MapTileDescriptor(5));
            map.SetTile(0, 19, new MapTileDescriptor(8));

            this.mapBack = new Map();

            mapBack.Create(30 * 2, 17 * 2, spriteTiles, new MapTileDescriptor(4));

            mapBack.SetTile(0, 0, new MapTileDescriptor(20));
            mapBack.SetTile(1, 0, new MapTileDescriptor(4) { isVerticalFlipped = true, isHorizontalFlipped = true });
            mapBack.SetTile(2, 0, new MapTileDescriptor(8));
            mapBack.SetTile(0, 1, new MapTileDescriptor(2));
            mapBack.SetTile(1, 1, new MapTileDescriptor(3) { isVerticalFlipped = true, isHorizontalFlipped = true });
            mapBack.SetTile(2, 1, new MapTileDescriptor(5));
            mapBack.SetTile(0, 19, new MapTileDescriptor(8));
        }

        /// <summary>
        /// Mise à jour de la frame
        /// </summary>

        private void OnFrameUpdate()
        {
            var gamepad = this.SugoiControl.Gamepad;
            var screen = this.SugoiControl.Screen;

            var hc = gamepad.HorizontalController;

            if (hc == GamepadKeys.Right)
            {
                if (flags[3])
                {
                    speedX = 1;
                }
                else
                {
                    sprX++;
                }
            }
            else if (hc == GamepadKeys.Left)
            {
                if (flags[3])
                {
                    speedX = -1;
                }
                else
                {
                    sprX--;
                }
            }

            var vc = gamepad.VerticalController;

            if (vc == GamepadKeys.Up)
            {
                if (flags[3])
                {
                    speedY = 1;
                }
                else
                {
                    sprY++;
                }
            }
            else if (vc == GamepadKeys.Down)
            {
                if (flags[3])
                {
                    speedY = -1;
                }
                else
                {
                    sprY--;
                }
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

            screen.Clear(Argb32.White);

            if(speedX != 0)
            {
                if( screen.CanScrollMap(map, sprX + speedX, sprY))
                {
                    sprX += speedX;
                }

                speedX = 0;
            }

            if (speedY != 0)
            {
                if (screen.CanScrollMap(map, sprX, sprY+speedY))
                {
                    sprY += speedY;
                }

                speedY = 0;
            }

            screen.DrawScrollMap(mapBack, 0, 0);
            screen.DrawSprite(spriteMonkey, 0, 0);
            screen.DrawScrollMap(mapBack, 4, 4);
            screen.DrawScrollMap(map, sprX, sprY);
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
                    case 3:
                        this.sprZ--;
                        break;
                    case 4:
                        this.sprZ++;
                        break;
                }
            }
        }
    }
}
