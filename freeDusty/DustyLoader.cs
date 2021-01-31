using System;
using StardewModdingAPI;

namespace freeDusty
{
    // Loads the Dusty sprite
    internal class DustyLoader : IAssetLoader
    {
        private static IModHelper _helper;

        public DustyLoader(IModHelper helper)
        {
            _helper = helper;
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Dusty");
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(@"Dusty")) return _helper.Content.Load<T>("assets/Dusty.png");

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}