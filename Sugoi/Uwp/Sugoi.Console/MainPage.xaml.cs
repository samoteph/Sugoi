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
        SurfaceSprite spriteTiles;
        SurfaceSprite spriteMonkey;

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

            var assetTiles = await LoadAssetFromApplicationAsync<AssetTileSheet>(
            "ms-appx:///Assets/Images/atlas.png",
            (stream) =>
            {
                return AssetTileSheet.Import("tiles", stream, 8, 8);
            });

            var assetMonkey = await LoadAssetFromApplicationAsync<AssetSprite>(
            "ms-appx:///Assets/Images/monkey.png",
            (stream) =>
            {
                return AssetSprite.Import("monkey", stream);
            });

            // normalement cartridge doit copier dans VideoMemory tous les Assets image au démarrage ou alors ils seront chargé à la demande
            //cartridge.Import(assetTiles);
            //cartridge.Import(assetMonkey);
            //var assetTiles = cartridge.GetAsset<AssetTileSheet>("tiles");

            // DEMARRAGE
            this.SugoiControl.Start(cartridge);

            // isic creation de spriteTiles (VideoMemory accessible de SugoiControl ?)
            //this.SugoiControl.

            //var map = new Map();

            //map.Create(100, 100, spriteTiles, new MapTileDescriptor(6));

            //map[0, 0] = new MapTileDescriptor(1);
            //map[1, 0] = new MapTileDescriptor(4) { isVerticalFlipped = true, isHorizontalFlipped = true };
            //map[2, 0] = new MapTileDescriptor(8);
            //map[0, 1] = new MapTileDescriptor(2);
            //map[1, 1] = new MapTileDescriptor(3) { isVerticalFlipped = true, isHorizontalFlipped = true };
            //map[2, 1] = new MapTileDescriptor(5);

        }

        /// <summary>
        /// Chargmeent d'un asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="import"></param>
        /// <returns></returns>

        private async Task<T> LoadAssetFromApplicationAsync<T>(string uri, Func<Stream, T> import) where T : AssetImage
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
            using (var stream = await file.OpenReadAsync())
            {
                return import(stream.AsStreamForRead());
            }
        }

        private void OnFrameUpdated()
        {
            
        }
    }
}
