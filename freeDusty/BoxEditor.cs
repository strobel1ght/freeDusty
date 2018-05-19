using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace freeDusty
{
    // Removes the eyes from Dusty's box
    internal class BoxEditor : IAssetEditor
    {
        private IModHelper Helper;
        private string prefix = "";

        public BoxEditor(IModHelper helper)
        {
            this.Helper = helper;

            if (Game1.currentSeason.ToLower().Equals("spring") || Game1.currentSeason.ToLower().Equals("summer"))
                prefix = "spring";
            else if (Game1.currentSeason.ToLower().Equals("fall"))
                prefix = "fall";
            else
                prefix = "winter";
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(prefix + "_town"))
                return true;
            else if (asset.AssetNameEquals(@"/Maps/" + prefix + "_town"))
                return true;

            return false;           
        }
        
        public void Edit<T>(IAssetData asset)
        {
            Texture2D emptyBox = this.Helper.Content.Load<Texture2D>("assets/"+prefix+"Box.png", ContentSource.ModFolder);
            asset.AsImage().PatchImage(emptyBox, targetArea: new Rectangle(192, 0, 16, 16));                        
        }
    }
}