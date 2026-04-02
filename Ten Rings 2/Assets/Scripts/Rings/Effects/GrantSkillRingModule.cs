using UnityEngine;

/// <summary>Grants a skill by id (e.g. Ring #3 Fireball).</summary>
[CreateAssetMenu(fileName = "EffectGrantSkill", menuName = "Ten Rings/Rings/Effects/Grant Skill", order = 11)]
public class GrantSkillRingModule : RingEffectModule
{
    [SerializeField] string skillId = "Fireball";

    public string SkillId => skillId;

    public override void Contribute(
        RingInstance ringInstance,
        RingDefinition ownerDefinition,
        int slotIndex,
        RingLoadoutEvaluation evaluation)
    {
        if (string.IsNullOrEmpty(skillId)) return;
        evaluation.AddGrantedSkill(skillId, ownerDefinition, slotIndex);
    }
}
