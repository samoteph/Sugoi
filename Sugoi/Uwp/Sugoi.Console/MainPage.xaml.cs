using Sugoi.Console.Controls;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        SurfaceFontSheet fontSheetPoly;
        SurfaceFontSheet fontSheetMono;

        SurfaceTileSheet spriteTiles;
        SurfaceSprite spriteMonkey;
        
        Map[] maps;
        Map[] playerMaps;
        int indexPlayerFrame = 0;
        int frame = 0;

        Font fontMono;
        Font fontPoly;

        public MainPage()
        {
            this.InitializeComponent();
            this.SugoiControl.Loaded += OnSugoiLoaded;

            FocusManager.GotFocus += FocusManager_GotFocus;

        }

        private void FocusManager_GotFocus(object sender, FocusManagerGotFocusEventArgs e)
        {
            Debug.WriteLine("Sender=" + sender +  " Focused=" + e.NewFocusedElement);
        }

        /// <summary>
        /// Console prête à fonctionner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void OnSugoiLoaded(object sender, RoutedEventArgs e)
        {
            var cartridge = new Cartridge();

            var assetFontMonoTetris = await AssetTools.LoadFontFromApplicationAsync("ms-appx:///Assets/Images/FontMono.png", "fontMono", 8, 8, FontTypes.MonoChromeDynamic);
              var assetTiles = await AssetTools.LoadTileSheetFromApplicationAsync("ms-appx:///Assets/Images/TileSheet.png","tiles",8,8);
            var assetMonkey = await AssetTools.LoadSpriteFromApplicationAsync("ms-appx:///Assets/Images/monkey.png","monkey");

            var assetFontPolyTetris = await AssetTools.LoadFontFromApplicationAsync("ms-appx:///Assets/Images/FontPoly.png", "fontPoly", 8,8, FontTypes.PolyChromeStatic, 4);

            var assetTmxMaps = await AssetTools.LoadTmxMapFromApplicationAsync("ms-appx:///Assets/Maps/Map.tmx", "tiles");

            var assetMap = AssetMap.Import("mapBack1","tiles", 60,17, new uint[] {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 177, 177, 177, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 177, 177, 177, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 0, 180, 139, 140, 140, 140, 140, 2147483787, 2147483828, 0, 0, 0, 0, 0, 0, 0, 0, 180, 139, 140, 140, 140, 140, 140, 140, 140, 2147483787, 2147483828, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 0, 138, 140, 140, 140, 140, 140, 140, 2147483786, 0, 0, 0, 0, 0, 0, 0, 177, 138, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483786, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 180, 139, 140, 140, 140, 2147483787, 2147483828, 177, 177, 180, 139, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483787, 2147483828, 180, 139, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483787, 2147483828, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 177, 177, 177, 138, 140, 140, 140, 140, 140, 2147483786, 177, 177, 138, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483786, 138, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483787, 0, 0, 0, 0, 0, 180, 139, 2147483787, 2147483828, 177, 177, 0, 0, 0, 180, 139, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483787, 2147483828, 177, 138, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2147483786, 177, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140});

            // normalement cartridge doit copier dans VideoMemory tous les Assets image au démarrage ou alors ils seront chargé à la demande
            cartridge.Import(assetFontPolyTetris);
            cartridge.Import(assetFontMonoTetris);
            cartridge.Import(assetTiles);
            cartridge.Import(assetMonkey);
            cartridge.Import(assetMap);

            foreach(var map in assetTmxMaps)
            {
                cartridge.Import(map);
            }

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
            var assetFontPoly = cartridge.GetAsset<AssetFont>("fontPoly");
            var assetFontMono = cartridge.GetAsset<AssetFont>("fontMono");

            var assetLayers = new AssetMap[6];

            assetLayers[0] = cartridge.GetAsset<AssetMap>("Calque 1");
            assetLayers[1] = cartridge.GetAsset<AssetMap>("Calque 2");
            assetLayers[2] = cartridge.GetAsset<AssetMap>("Calque 3");
            assetLayers[3] = cartridge.GetAsset<AssetMap>("Calque 4");
            assetLayers[4] = cartridge.GetAsset<AssetMap>("Calque 5");
            assetLayers[5] = cartridge.GetAsset<AssetMap>("Calque 6");

            var assetMap = cartridge.GetAsset<AssetMap>("mapBack1");

            this.spriteMonkey = videoMemory.CreateSprite(assetMonkey);
            this.spriteTiles = videoMemory.CreateTileSheet(assetTiles);

            this.fontSheetMono = videoMemory.CreateFontSheetMonoChromeDynamic(assetFontMono, Argb32.Blue, Argb32.White, Argb32.Green, Argb32.Red);
            this.fontSheetPoly = videoMemory.CreateFontSheetPolyChromeStatic(assetFontPoly);

            this.fontMono = new StandardFont();
            this.fontMono.FontSheet = fontSheetMono;

            this.fontPoly = new StandardFont();
            this.fontPoly.FontSheet = fontSheetPoly;

            this.maps = new Map[assetLayers.Length];

            for (int l = 0; l < this.maps.Length; l++)
            {
                this.maps[l] = new Map();
                this.maps[l].Create(assetLayers[l], videoMemory);
            }

            this.playerMaps = new Map[3];

            var map = new Map();
            map.Create(2, 2, spriteTiles, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(0);
            map[1, 0] = new MapTileDescriptor(1);
            map[0, 1] = new MapTileDescriptor(10);
            map[1, 1] = new MapTileDescriptor(11);
            this.playerMaps[0] = map;

            map = new Map();
            map.Create(2, 2, spriteTiles, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(2);
            map[1, 0] = new MapTileDescriptor(3);
            map[0, 1] = new MapTileDescriptor(12);
            map[1, 1] = new MapTileDescriptor(13);
            this.playerMaps[1] = map;

            map = new Map();
            map.Create(2, 2, spriteTiles, MapTileDescriptor.HiddenTile);
            map[0, 0] = new MapTileDescriptor(4);
            map[1, 0] = new MapTileDescriptor(5);
            map[0, 1] = new MapTileDescriptor(14);
            map[1, 1] = new MapTileDescriptor(15);
            this.playerMaps[2] = map;

            //this.map = new Map();

            //map.Create(30*2, 17*2, spriteTiles, new MapTileDescriptor(1));

            //map.SetTile(0,  0, new MapTileDescriptor(20));
            //map.SetTile(1,  0, new MapTileDescriptor(4) { isVerticalFlipped = true, isHorizontalFlipped = true });
            //map.SetTile(2,  0, new MapTileDescriptor(8));
            //map.SetTile(0,  1, new MapTileDescriptor(2));
            //map.SetTile(1,  1, new MapTileDescriptor(3) { isVerticalFlipped = true, isHorizontalFlipped = true });
            //map.SetTile(2,  1, new MapTileDescriptor(5));
            //map.SetTile(0, 19, new MapTileDescriptor(8));

            //this.mapBack = new Map();
            //mapBack.Create(30 * 2, 17 * 2, spriteTiles, new MapTileDescriptor(4));

            //mapBack.SetTile(0, 0, new MapTileDescriptor(20));
            //mapBack.SetTile(1, 0, new MapTileDescriptor(4) { isVerticalFlipped = true, isHorizontalFlipped = true });
            //mapBack.SetTile(2, 0, new MapTileDescriptor(8));
            //mapBack.SetTile(0, 1, new MapTileDescriptor(2));
            //mapBack.SetTile(1, 1, new MapTileDescriptor(3) { isVerticalFlipped = true, isHorizontalFlipped = true });
            //mapBack.SetTile(2, 1, new MapTileDescriptor(5));
            //mapBack.SetTile(0, 19, new MapTileDescriptor(8));
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

            screen.Clear(new Argb32(0xeeeecc));

            //if(speedX != 0)
            //{
            //    if( screen.CanScrollMap(map, sprX + speedX, sprY))
            //    {
                    sprX += speedX;
            //    }

                speedX = 0;
            //}

            //if (speedY != 0)
            //{
            //    if (screen.CanScrollMap(map, sprX, sprY+speedY))
            //    {
                    sprY += speedY;
            //    }

                speedY = 0;
            //}

            //screen.DrawScrollMap(mapBack, 0, 0);
            //screen.DrawSprite(spriteMonkey, 0, 0);

            //screen.DrawScrollMap(maps[5], true, (int)(-sprX * 0.5), 0);
            //screen.DrawScrollMap(maps[2], true,  (int)(-sprX * 0.6), 0);
            //screen.DrawScrollMap(maps[1], true, (int)(-sprX * 0.7), 0);
            //screen.DrawScrollMap(maps[4], true, (int)(-sprX * 0.8), 0);
            //screen.DrawScrollMap(maps[0], true, (int)(-sprX * 0.9), 0);

            //screen.DrawSpriteMap(this.playerMaps[indexPlayerFrame], 100, 60 + sprY, true, false);

            //screen.DrawScrollMap(maps[3], true, (int)(-sprX * 1.0), 0);

            //screen.DrawSpriteMap(this.playerMaps[indexPlayerFrame], 100, 60 + sprY, true, false);

            //screen.DrawScrollMap(this.playerMaps[0], true, -sprx, 0);

            if (flags[0])
            {
                screen.DrawScrollMap(maps[0], true, (int)(-sprX), (int)(-sprY), 12, 12, 240 - (20), 136 - (20), flags[3], flags[4]);
            }
            else
            {
                screen.DrawScrollMap(maps[0], true, (int)(-sprX), (int)(-sprY), 8, 8, 240 - 8, 136 - 8, flags[3], flags[4]);
            }

            if (frame == 5)
            {
                this.indexPlayerFrame = (this.indexPlayerFrame + 1) % this.playerMaps.Length;
                frame = 0;
            }

            frame++;

            //screen.DrawSprite(fontMono.FontSheet, 0, 0, false, false); 
            //screen.DrawSprite(fontPoly.FontSheet,240-fontPoly.FontSheet.Width,0,false,false);

            for (int bank = 0; bank < fontMono.FontSheet.BankCount; bank++)
            {
                screen.DrawText(fontMono, text, 0, bank * fontMono.FontSheet.TileHeight, bank);
            }
            
            //screen.DrawText(fontMono, text, 0, 0);
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

        /// <summary>
        /// Text a afficher
        /// </summary>
        string text = "Hello!";

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            text = ((TextBox)sender).Text;
        }
    }
}
