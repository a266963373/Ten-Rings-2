using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One equipped ring slot: icon area for ring art (or empty placeholder) + guide text (finger / index).
/// </summary>
public class EquipRingSlotView : MonoBehaviour
{
    [HideInInspector] public int slotIndex;

    public Image ringIcon;
    public TextMeshProUGUI guideLabel;
}
