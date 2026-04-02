using UnityEngine;

/// <summary>Flat stat bonus (e.g. Ring #1 Strength).</summary>
[CreateAssetMenu(fileName = "EffectStatBonus", menuName = "Ten Rings/Rings/Effects/Stat Bonus", order = 10)]
public class StatBonusRingModule : RingEffectModule
{
    [SerializeField] StatId stat = StatId.Strength;
    [SerializeField] int flatBonus = 1;

    public StatId Stat => stat;
    public int FlatBonus => flatBonus;

    public override void Contribute(
        RingInstance ringInstance,
        RingDefinition ownerDefinition,
        int slotIndex,
        RingLoadoutEvaluation evaluation)
    {
        evaluation.AddStatFlat(stat, flatBonus);
    }
}
