using UnityEditor;
using UnityEngine;

/// <summary>One-shot assets for rings #1 Strength, #2 Thorn, #3 Fireball (table IDs).</summary>
public static class CreateSampleRings123
{
    const string Root = "Assets/Data/Rings/Sample";

    [MenuItem("Ten Rings/Rings/Create Sample Rings 1-3 (Data Assets)")]
    public static void Create()
    {
        EnsureFolder("Assets/Data");
        EnsureFolder("Assets/Data/Rings");
        EnsureFolder(Root);
        EnsureFolder($"{Root}/Effects");

        var strMod = ScriptableObject.CreateInstance<StatBonusRingModule>();
        ApplySerialized(strMod, s =>
        {
            s.FindProperty("stat").enumValueIndex = (int)StatId.Strength;
            s.FindProperty("flatBonus").intValue = 10;
        });
        Save(strMod, $"{Root}/Effects/Module_Ring01_StatStrength.asset");

        var thornMod = ScriptableObject.CreateInstance<MeleeReflectDamageRingModule>();
        ApplySerialized(thornMod, s =>
        {
            s.FindProperty("damage").intValue = 5;
            s.FindProperty("element").enumValueIndex = (int)DamageElement.Grass;
        });
        Save(thornMod, $"{Root}/Effects/Module_Ring02_Thorn.asset");

        var fireMod = ScriptableObject.CreateInstance<GrantSkillRingModule>();
        ApplySerialized(fireMod, s => { s.FindProperty("skillId").stringValue = "Fireball"; });
        Save(fireMod, $"{Root}/Effects/Module_Ring03_Fireball.asset");

        var ring1 = CreateRingDef("ring_01_strength", "Strength", "Flat Strength bonus.", strMod);
        Save(ring1, $"{Root}/Ring_01_Strength.asset");

        var ring2 = CreateRingDef("ring_02_thorn", "Thorn", "On taking melee damage, the attacker takes 5 grass damage.", thornMod);
        Save(ring2, $"{Root}/Ring_02_Thorn.asset");

        var ring3 = CreateRingDef("ring_03_fireball", "Fireball", "Grants the Fireball skill.", fireMod);
        Save(ring3, $"{Root}/Ring_03_Fireball.asset");

        var catalog = ScriptableObject.CreateInstance<RingCatalog>();
        var cs = new SerializedObject(catalog);
        var defs = cs.FindProperty("definitions");
        defs.arraySize = 3;
        defs.GetArrayElementAtIndex(0).objectReferenceValue = ring1;
        defs.GetArrayElementAtIndex(1).objectReferenceValue = ring2;
        defs.GetArrayElementAtIndex(2).objectReferenceValue = ring3;
        cs.ApplyModifiedPropertiesWithoutUndo();
        Save(catalog, $"{Root}/RingCatalog_Sample123.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorGUIUtility.PingObject(catalog);
        Debug.Log($"Sample rings created under {Root}. Assign RingCatalog_Sample123 where you load definitions.");
    }

    static RingDefinition CreateRingDef(string id, string display, string desc, RingEffectModule single)
    {
        var def = ScriptableObject.CreateInstance<RingDefinition>();
        ApplySerialized(def, s =>
        {
            s.FindProperty("definitionId").stringValue = id;
            s.FindProperty("displayName").stringValue = display;
            s.FindProperty("description").stringValue = desc;
            var arr = s.FindProperty("effectModules");
            arr.arraySize = 1;
            arr.GetArrayElementAtIndex(0).objectReferenceValue = single;
        });
        return def;
    }

    static void ApplySerialized(Object o, System.Action<SerializedObject> edit)
    {
        var s = new SerializedObject(o);
        edit(s);
        s.ApplyModifiedPropertiesWithoutUndo();
    }

    static void Save(ScriptableObject o, string path)
    {
        AssetDatabase.CreateAsset(o, path);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        var parent = path.Substring(0, path.LastIndexOf('/'));
        var name = path.Substring(path.LastIndexOf('/') + 1);
        AssetDatabase.CreateFolder(parent, name);
    }
}
