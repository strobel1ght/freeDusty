using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace freeDusty
{
    // Removes the eyes from Dusty's box
    internal class BoxEditor : IAssetEditor
    {
        private readonly IModHelper _helper;
        private readonly string _prefix;
        private readonly bool _eyes;

        public BoxEditor(IModHelper helper, string pre = "", bool eyes = false)
        {
            _helper = helper;
            _prefix = pre;
            _eyes = eyes;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("/Maps/" + _prefix + "_town");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (!asset.AssetNameEquals("/Maps/" + _prefix + "_town")) return;
            var editor = asset.AsImage();
            var emptyBox = _helper.Content.Load<Texture2D>("assets/" + _prefix + "Box.png");
            var eyesBox = _helper.Content.Load<Texture2D>("assets/" + _prefix + "BoxEyes.png");
            editor.PatchImage(!_eyes ? emptyBox : eyesBox, targetArea: new Rectangle(192, 0, 16, 16));
        }
    }
}