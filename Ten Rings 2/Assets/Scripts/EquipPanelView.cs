using UnityEngine;

/// <summary>
/// Root view for the ring equip panel: character region + ring region (equipped slots + warehouse list).
/// Wire references in the inspector or use the editor menu that builds EquipPanel.prefab.
/// </summary>
public class EquipPanelView : MonoBehaviour
{
    [Header("Main regions")]
    public RectTransform characterArea;
    public RectTransform ringArea;

    [Header("Ring region (equipped + stocked)")]
    public RectTransform equippedArea;
    public RectTransform stockedArea;

    [Tooltip("ScrollRect content; list rows will be parented here later.")]
    public RectTransform stockedContent;

    [Tooltip("Slot 0 = left pinky, 9 = right pinky per design doc.")]
    public RectTransform[] equippedSlots = new RectTransform[10];

    public EquipRingSlotView[] ringSlotViews = new EquipRingSlotView[10];
}
