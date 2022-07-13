using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Editor.Helpers
{
    public static class EditorAddressableHelpers
    {
        public static void CreateAllFolders(string path)
        {
            var folders = path.Split('/');
            for (var i = 1; i < folders.Length; i++)
            {
                var initial = string.Join('/', folders[..i]);
                var next = folders[i];

                if (!AssetDatabase.IsValidFolder($"{initial}/{next}"))
                {
                    AssetDatabase.CreateFolder(initial, next);
                }
            }
        }

        public static void SetAddressable(string path)
        {
            var guid = AssetDatabase.GUIDFromAssetPath(path);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.DefaultGroup;
            var entry = settings.CreateOrMoveEntry(guid.ToString(), group, false, false);

            entry.address = AssetDatabase.GUIDToAssetPath(guid);

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        }
    }
}