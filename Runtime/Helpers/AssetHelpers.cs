using UnityEditor;

namespace Helpers
{
    public static class AssetHelpers
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
    }
}