using UnityEditor;

namespace Fumiki.Editor.SpriteCollection
{
    public static class SpriteCollectionUtility
    {
        public static void RefreshSpriteCollection()
        {
            string[] guids = AssetDatabase.FindAssets("t:SpriteCollection");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Fumiki.SpriteCollection collection = AssetDatabase.LoadAssetAtPath<Fumiki.SpriteCollection>(path);
                collection.Pack();
            }

            AssetDatabase.SaveAssets();
        }
    }
}