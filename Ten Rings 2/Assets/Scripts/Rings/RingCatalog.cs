using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Optional registry of all <see cref="RingDefinition"/> assets for runtime lookup by <see cref="RingDefinition.DefinitionId"/>.
/// Create one asset (e.g. under Resources or reference from a bootstrap) and assign definitions.
/// </summary>
[CreateAssetMenu(fileName = "RingCatalog", menuName = "Ten Rings/Ring Catalog", order = 1)]
public class RingCatalog : ScriptableObject
{
    [SerializeField] RingDefinition[] definitions;

    readonly Dictionary<string, RingDefinition> _byId = new Dictionary<string, RingDefinition>(StringComparer.Ordinal);

    void OnEnable()
    {
        RebuildIndex();
    }

    void OnValidate()
    {
        RebuildIndex();
    }

    void RebuildIndex()
    {
        _byId.Clear();
        if (definitions == null) return;

        foreach (var def in definitions)
        {
            if (def == null) continue;
            var id = def.DefinitionId;
            if (string.IsNullOrEmpty(id))
                continue;
            if (_byId.ContainsKey(id))
            {
                Debug.LogWarning($"RingCatalog: duplicate definition id '{id}', keeping first.", this);
                continue;
            }

            _byId[id] = def;
        }
    }

    public bool TryGetDefinition(string definitionId, out RingDefinition definition)
    {
        if (string.IsNullOrEmpty(definitionId))
        {
            definition = null;
            return false;
        }

        return _byId.TryGetValue(definitionId, out definition);
    }

    public RingDefinition GetDefinitionOrNull(string definitionId)
    {
        TryGetDefinition(definitionId, out var def);
        return def;
    }

    public IReadOnlyList<RingDefinition> Definitions => definitions ?? Array.Empty<RingDefinition>();
}
