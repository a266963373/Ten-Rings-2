using UnityEngine;

/// <summary>
/// Static template for a ring kind (stats rules, skills, etc. live in data/effects later).
/// Same definition = same gameplay rules; variable state belongs on <see cref="RingInstance"/>.
/// </summary>
[CreateAssetMenu(fileName = "RingDefinition", menuName = "Ten Rings/Ring Definition", order = 0)]
public class RingDefinition : ScriptableObject
{
    [Tooltip("Stable id for saves and lookups. Must be unique across all ring definitions.")]
    [SerializeField] string definitionId;

    [SerializeField] string displayName;

    [TextArea(2, 6)]
    [SerializeField] string description;

    [SerializeField] Sprite icon;

    [Tooltip("If true, this ring may store per-instance binding (e.g. captured enemy id).")]
    [SerializeField] bool supportsInstanceBinding;

    [SerializeField] RingEffectModule[] effectModules;

    public string DefinitionId => definitionId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public bool SupportsInstanceBinding => supportsInstanceBinding;

    public void ContributeFromModules(RingInstance ringInstance, int slotIndex, RingLoadoutEvaluation evaluation)
    {
        if (effectModules == null || evaluation == null) return;
        foreach (var module in effectModules)
        {
            if (module == null) continue;
            module.Contribute(ringInstance, this, slotIndex, evaluation);
        }
    }

    void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(definitionId))
            definitionId = name;
        definitionId = definitionId.Trim();
    }
}
