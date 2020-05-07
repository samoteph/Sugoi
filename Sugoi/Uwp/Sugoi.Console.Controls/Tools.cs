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
        public static async Task<T> LoadFileFromApplicationAsync<T>(string uri, Func<Stream, T> import)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
            using (var stream = await file.OpenReadAsync())
            {
                return import(stream.AsStreamForRead());
            }
        }

        public static async Task<AssetSprite> LoadSpriteFromApplicationAsync(string uri, string assetName)
        {
            return await LoadFileFromApplicationAsync<AssetSprite>(uri,
            (stream) =>
            {
                return AssetSprite.Import(assetName, stream);
            });
        }

        public static async Task<AssetTileSheet> LoadTileSheetFromApplicationAsync(string uri, string assetName, int tileWidth, int tileHeight)
        {
            return await LoadFileFromApplicationAsync<AssetTileSheet>(uri,
            (stream) =>
            {
                return AssetTileSheet.Import(assetName, stream, tileWidth, tileHeight);
            });
        }

        public static async Task<AssetFont> LoadFontFromApplicationAsync(string uri, string assetName, int tileWidth, int tileHeight, FontTypes fontType, int tileHeightBank = -1)
        {
            return await LoadFileFromApplicationAsync<AssetFont>(uri,
            (stream) =>
            {
                return AssetFont.Import(assetName, stream, tileWidth, tileHeight, fontType, tileHeightBank);
            });
        }

        public static async Task<List<AssetMap>> LoadTmxMapFromApplicationAsync(string uri, string assetTileSheetName)
        {
            return await LoadFileFromApplicationAsync<List<AssetMap>>(uri,
            (stream) =>
            {
                return AssetMap.Import(assetTileSheetName, stream);
            });
        }
    }
}
