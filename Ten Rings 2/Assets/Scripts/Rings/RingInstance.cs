using System;
using UnityEngine;

/// <summary>
/// One owned ring in the warehouse or on a character. Identified by <see cref="InstanceId"/>.
/// </summary>
[Serializable]
public class RingInstance
{
    [SerializeField] string instanceId;
    [SerializeField] string definitionId;
    [SerializeField] string boundTargetId;

    public string InstanceId => instanceId;
    public string DefinitionId => definitionId;

    /// <summary>Optional binding (e.g. memory / capture ring). Empty if unused.</summary>
    public string BoundTargetId => boundTargetId ?? string.Empty;

    public bool HasBoundTarget => !string.IsNullOrEmpty(boundTargetId);

    public static RingInstance Create(RingDefinition definition, string boundTargetId = null)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        return new RingInstance
        {
            instanceId = Guid.NewGuid().ToString("N"),
            definitionId = definition.DefinitionId,
            boundTargetId = boundTargetId ?? string.Empty
        };
    }

    /// <summary>New instance id, same definition and binding (rare; prefer <see cref="Create"/> for new loot).</summary>
    public RingInstance Clone()
    {
        return new RingInstance
        {
            instanceId = Guid.NewGuid().ToString("N"),
            definitionId = definitionId,
            boundTargetId = boundTargetId ?? string.Empty
        };
    }

    public void SetBoundTargetId(string targetId)
    {
        boundTargetId = targetId ?? string.Empty;
    }

    public void ClearBoundTarget()
    {
        boundTargetId = string.Empty;
    }
}
