using UnityEngine;

/// <summary>
/// When the wearer takes melee damage, the attacker takes reflected damage (e.g. Ring #2 Thorn: 5 grass).
/// Battle code should subscribe to melee-hit events and apply <see cref="RingLoadoutEvaluation.MeleeReflectRules"/>.
/// </summary>
[CreateAssetMenu(fileName = "EffectMeleeReflect", menuName = "Ten Rings/Rings/Effects/Melee Reflect Damage", order = 12)]
public class MeleeReflectDamageRingModule : RingEffectModule
{
    [SerializeField] int damage = 5;
    [SerializeField] DamageElement element = DamageElement.Grass;

    public int Damage => damage;
    public DamageElement Element => element;

    public override void Contribute(
        RingInstance ringInstance,
        RingDefinition ownerDefinition,
        int slotIndex,
        RingLoadoutEvaluation evaluation)
    {
        evaluation.AddMeleeReflectRule(new MeleeReflectRule(damage, element, ownerDefinition, slotIndex));
    }
}
