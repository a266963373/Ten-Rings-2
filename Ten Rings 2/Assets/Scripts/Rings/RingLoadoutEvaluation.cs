using System;
using System.Collections.Generic;

/// <summary>Output of <see cref="RingLoadoutEvaluator"/> for one character's equipped rings (order preserved where relevant).</summary>
public sealed class RingLoadoutEvaluation
{
    readonly List<GrantedSkillEntry> _grantedSkills = new List<GrantedSkillEntry>();
    readonly List<MeleeReflectRule> _meleeReflect = new List<MeleeReflectRule>();

    readonly Dictionary<StatId, int> _statFlat = new Dictionary<StatId, int>();

    public IReadOnlyDictionary<StatId, int> StatFlatBonuses => _statFlat;
    public IReadOnlyList<GrantedSkillEntry> GrantedSkills => _grantedSkills;
    public IReadOnlyList<MeleeReflectRule> MeleeReflectRules => _meleeReflect;

    internal void AddStatFlat(StatId stat, int amount)
    {
        if (amount == 0) return;
        if (_statFlat.TryGetValue(stat, out var cur))
            _statFlat[stat] = cur + amount;
        else
            _statFlat[stat] = amount;
    }

    internal void AddGrantedSkill(string skillId, RingDefinition sourceDefinition, int slotIndex)
    {
        _grantedSkills.Add(new GrantedSkillEntry(skillId, sourceDefinition, slotIndex));
    }

    internal void AddMeleeReflectRule(MeleeReflectRule rule)
    {
        _meleeReflect.Add(rule);
    }

    public int GetFlatStat(StatId stat)
    {
        return _statFlat.TryGetValue(stat, out var v) ? v : 0;
    }
}

public readonly struct GrantedSkillEntry : IEquatable<GrantedSkillEntry>
{
    public readonly string SkillId;
    public readonly RingDefinition SourceDefinition;
    public readonly int SlotIndex;

    public GrantedSkillEntry(string skillId, RingDefinition sourceDefinition, int slotIndex)
    {
        SkillId = skillId;
        SourceDefinition = sourceDefinition;
        SlotIndex = slotIndex;
    }

    public bool Equals(GrantedSkillEntry other) =>
        SkillId == other.SkillId && SourceDefinition == other.SourceDefinition && SlotIndex == other.SlotIndex;

    public override bool Equals(object obj) => obj is GrantedSkillEntry other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(SkillId, SourceDefinition, SlotIndex);
}

public readonly struct MeleeReflectRule : IEquatable<MeleeReflectRule>
{
    public readonly int Damage;
    public readonly DamageElement Element;
    public readonly RingDefinition SourceDefinition;
    public readonly int SlotIndex;

    public MeleeReflectRule(int damage, DamageElement element, RingDefinition sourceDefinition, int slotIndex)
    {
        Damage = damage;
        Element = element;
        SourceDefinition = sourceDefinition;
        SlotIndex = slotIndex;
    }

    public bool Equals(MeleeReflectRule other) =>
        Damage == other.Damage && Element == other.Element && SourceDefinition == other.SourceDefinition && SlotIndex == other.SlotIndex;

    public override bool Equals(object obj) => obj is MeleeReflectRule other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Damage, (int)Element, SourceDefinition, SlotIndex);
}
