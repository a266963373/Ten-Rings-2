using UnityEngine;

/// <summary>One composable piece of a <see cref="RingDefinition"/>.</summary>
public abstract class RingEffectModule : ScriptableObject
{
    /// <param name="slotIndex">0–9, equipment order.</param>
    public abstract void Contribute(
        RingInstance ringInstance,
        RingDefinition ownerDefinition,
        int slotIndex,
        RingLoadoutEvaluation evaluation);
}
