using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Project-wide Chinese TMP font: set in Assets/TextMesh Pro/Resources/TMP Settings.asset (default for NEW text).
/// Existing TextMeshProUGUI still store their old font until you run the menu items below.
/// </summary>
public static class TmpChineseFontApplier
{
    public const string ChineseFontAssetPath = "Assets/Chinese Fonts/SourceHanSansSC-Medium SDF.asset";

    public static TMP_FontAsset LoadChineseFont()
    {
        return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ChineseFontAssetPath);
    }

    [MenuItem("Ten Rings/TMP/Apply Chinese Font To All Prefabs")]
    public static void ApplyToAllPrefabs()
    {
        var font = LoadChineseFont();
        if (font == null)
        {
            Debug.LogError($"TMP: missing font at {ChineseFontAssetPath}");
            return;
        }

        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        var componentCount = 0;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("PackageCache")) continue;

            using (var scope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                var root = scope.prefabContentsRoot;
                componentCount += SwapFontOnHierarchy(root, font);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"TMP: set Chinese font on {componentCount} component(s) across prefabs.");
    }

    [MenuItem("Ten Rings/TMP/Apply Chinese Font To Loaded Scenes")]
    public static void ApplyToLoadedScenes()
    {
        var font = LoadChineseFont();
        if (font == null)
        {
            Debug.LogError($"TMP: missing font at {ChineseFontAssetPath}");
            return;
        }

        var total = 0;
        for (var i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            var dirty = false;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (SwapFontOnHierarchy(root, font) > 0)
                    dirty = true;
            }

            if (dirty)
            {
                total++;
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        Debug.Log($"TMP: Chinese font applied in {total} scene(s) (dirty marked; save scenes to keep).");
    }

    static int SwapFontOnHierarchy(GameObject root, TMP_FontAsset font)
    {
        var n = 0;
        foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp.font != font)
            {
                tmp.font = font;
                n++;
            }
        }

        foreach (var tmp in root.GetComponentsInChildren<TextMeshPro>(true))
        {
            if (tmp.font != font)
            {
                tmp.font = font;
                n++;
            }
        }

        return n;
    }
}
