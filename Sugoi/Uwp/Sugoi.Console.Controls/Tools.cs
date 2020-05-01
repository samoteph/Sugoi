using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Sugoi.Console.Controls
{
    public class AssetTools
    {
        public static  async Task<T> LoadImageFromApplicationAsync<T>(string uri, Func<Stream, T> import) where T : AssetImage
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
            using (var stream = await file.OpenReadAsync())
            {
                return import(stream.AsStreamForRead());
            }
        }

        public static async Task<AssetSprite> LoadSpriteFromApplicationAsync(string uri, string assetName)
        {
            return await LoadImageFromApplicationAsync<AssetSprite>(uri,
            (stream) =>
            {
                return AssetSprite.Import(assetName, stream);
            });
        }

        public static async Task<AssetTileSheet> LoadTileSheetFromApplicationAsync(string uri, string assetName, int tileWidth, int tileHeight)
        {
            return await LoadImageFromApplicationAsync<AssetTileSheet>(uri,
            (stream) =>
            {
                return AssetTileSheet.Import(assetName, stream, tileWidth, tileHeight);
            });

        }
    }
}
