using System.Collections.Generic;

/// <summary>
/// Aggregates <see cref="RingEffectModule"/> contributions for 10 ordered slots.
/// </summary>
public static class RingLoadoutEvaluator
{
    /// <summary>
    /// Walks slots 0→9. Null or missing definitions are skipped.
    /// </summary>
    public static RingLoadoutEvaluation Evaluate(
        IReadOnlyList<RingInstance> orderedSlots,
        RingCatalog catalog)
    {
        var eval = new RingLoadoutEvaluation();
        if (orderedSlots == null || catalog == null) return eval;

        var count = orderedSlots.Count;
        if (count > 10) count = 10;

        for (var i = 0; i < count; i++)
        {
            var inst = orderedSlots[i];
            if (inst == null || string.IsNullOrEmpty(inst.DefinitionId)) continue;
            if (!catalog.TryGetDefinition(inst.DefinitionId, out var def) || def == null) continue;

            def.ContributeFromModules(inst, i, eval);
        }

        return eval;
    }
}
