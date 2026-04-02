using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class EquipPanelPrefabBuilder
{
    const string PrefabPath = "Assets/Prefabs/UI/EquipPanel.prefab";
    const string TmpFontAssetPath = "Assets/Chinese Fonts/SourceHanSansSC-Medium SDF.asset";

    /// <summary>Slot 0 = left pinky → 9 = right pinky (hands open, palm up, left to right).</summary>
    static readonly string[] FingerGuideLabels =
    {
        "左小指", "左无名指", "左中指", "左食指", "左拇指",
        "右拇指", "右食指", "右中指", "右无名指", "右小指"
    };

    [MenuItem("Ten Rings/UI/Build Equip Panel Prefab")]
    public static void BuildPrefab()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TmpFontAssetPath);

        var root = new GameObject("EquipPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(EquipPanelView));
        var rootRt = root.GetComponent<RectTransform>();
        StretchFull(rootRt);
        root.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.12f, 0.92f);

        var view = root.GetComponent<EquipPanelView>();

        var hlg = root.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 16f;
        hlg.padding = new RectOffset(24, 24, 24, 24);
        hlg.childAlignment = TextAnchor.UpperCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;

        var characterArea = CreateRegion("CharacterArea", root.transform, new Color(0.12f, 0.16f, 0.22f, 0.55f));
        var charLe = characterArea.gameObject.AddComponent<LayoutElement>();
        charLe.preferredWidth = 400f;
        charLe.flexibleWidth = 0f;
        charLe.minWidth = 280f;
        var charVlg = characterArea.gameObject.AddComponent<VerticalLayoutGroup>();
        charVlg.spacing = 10f;
        charVlg.padding = new RectOffset(12, 12, 12, 12);
        charVlg.childAlignment = TextAnchor.UpperCenter;
        charVlg.childControlWidth = true;
        charVlg.childControlHeight = true;
        charVlg.childForceExpandWidth = true;
        charVlg.childForceExpandHeight = false;
        AddSectionLabel(characterArea, "Character", font, useLayoutElement: true);
        var charBody = CreateRegion("CharacterBody", characterArea.transform, new Color(0.08f, 0.11f, 0.15f, 0.35f));
        var charBodyLe = charBody.gameObject.AddComponent<LayoutElement>();
        charBodyLe.flexibleHeight = 1f;
        charBodyLe.minHeight = 120f;

        var ringArea = CreateRegion("RingArea", root.transform, new Color(0.1f, 0.14f, 0.18f, 0.45f));
        var ringLe = ringArea.gameObject.AddComponent<LayoutElement>();
        ringLe.flexibleWidth = 1f;
        ringLe.minWidth = 480f;

        var ringVlg = ringArea.gameObject.AddComponent<VerticalLayoutGroup>();
        ringVlg.spacing = 12f;
        ringVlg.padding = new RectOffset(16, 16, 16, 16);
        ringVlg.childAlignment = TextAnchor.UpperCenter;
        ringVlg.childControlWidth = true;
        ringVlg.childControlHeight = true;
        ringVlg.childForceExpandWidth = true;
        ringVlg.childForceExpandHeight = false;

        var equippedArea = CreateRegion("EquippedArea", ringArea.transform, new Color(0.14f, 0.18f, 0.24f, 0.65f));
        var eqLe = equippedArea.gameObject.AddComponent<LayoutElement>();
        eqLe.preferredHeight = 260f;
        eqLe.flexibleHeight = 0f;
        eqLe.minHeight = 200f;

        var eqVlg = equippedArea.gameObject.AddComponent<VerticalLayoutGroup>();
        eqVlg.spacing = 8f;
        eqVlg.padding = new RectOffset(12, 12, 12, 12);
        eqVlg.childAlignment = TextAnchor.UpperCenter;
        eqVlg.childControlWidth = true;
        eqVlg.childControlHeight = true;
        eqVlg.childForceExpandWidth = true;
        eqVlg.childForceExpandHeight = false;

        AddSectionLabel(equippedArea, "Equipped (0–9)", font, useLayoutElement: true);

        var slotsRow = new GameObject("EquippedSlotsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        slotsRow.transform.SetParent(equippedArea.transform, false);
        var slotsRt = slotsRow.GetComponent<RectTransform>();
        StretchLayoutChild(slotsRt);
        var slotsLe = slotsRow.AddComponent<LayoutElement>();
        slotsLe.preferredHeight = 148f;
        slotsLe.flexibleHeight = 0f;
        var hSlots = slotsRow.GetComponent<HorizontalLayoutGroup>();
        hSlots.spacing = 6f;
        hSlots.padding = new RectOffset(4, 4, 4, 4);
        hSlots.childAlignment = TextAnchor.MiddleCenter;
        hSlots.childControlWidth = true;
        hSlots.childControlHeight = true;
        hSlots.childForceExpandWidth = true;
        hSlots.childForceExpandHeight = true;

        var slots = new RectTransform[10];
        var slotViews = new EquipRingSlotView[10];
        for (var i = 0; i < 10; i++)
        {
            var (slotRt, slotView) = CreateSlot(slotsRow.transform, i, font);
            slots[i] = slotRt;
            slotViews[i] = slotView;
        }

        var stockedArea = CreateRegion("StockedArea", ringArea.transform, new Color(0.11f, 0.15f, 0.2f, 0.55f));
        var stLe = stockedArea.gameObject.AddComponent<LayoutElement>();
        stLe.flexibleHeight = 1f;
        stLe.minHeight = 200f;

        var stVlg = stockedArea.gameObject.AddComponent<VerticalLayoutGroup>();
        stVlg.spacing = 8f;
        stVlg.padding = new RectOffset(12, 12, 12, 12);
        stVlg.childAlignment = TextAnchor.UpperCenter;
        stVlg.childControlWidth = true;
        stVlg.childControlHeight = true;
        stVlg.childForceExpandWidth = true;
        stVlg.childForceExpandHeight = false;

        AddSectionLabel(stockedArea, "Warehouse", font, useLayoutElement: true);

        var scrollRoot = new GameObject("StockedScroll", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollRoot.transform.SetParent(stockedArea.transform, false);
        var scrollRt = scrollRoot.GetComponent<RectTransform>();
        StretchLayoutChild(scrollRt);
        var scrollLe = scrollRoot.AddComponent<LayoutElement>();
        scrollLe.flexibleHeight = 1f;
        scrollLe.minHeight = 120f;
        scrollRoot.GetComponent<Image>().color = new Color(0.05f, 0.07f, 0.1f, 0.5f);

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        viewport.transform.SetParent(scrollRoot.transform, false);
        var vpRt = viewport.GetComponent<RectTransform>();
        StretchFull(vpRt);
        viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0.01f);

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.anchoredPosition = Vector2.zero;
        contentRt.sizeDelta = new Vector2(0f, 0f);
        var vlgContent = content.GetComponent<VerticalLayoutGroup>();
        vlgContent.childAlignment = TextAnchor.UpperCenter;
        vlgContent.childControlWidth = true;
        vlgContent.childControlHeight = true;
        vlgContent.childForceExpandWidth = true;
        vlgContent.spacing = 6f;
        vlgContent.padding = new RectOffset(8, 8, 8, 8);
        var fitter = content.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        var sr = scrollRoot.GetComponent<ScrollRect>();
        sr.viewport = vpRt;
        sr.content = contentRt;
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;
        sr.scrollSensitivity = 32f;

        AddPlaceholderRow(contentRt, "No rings in warehouse (placeholder)", font);

        view.characterArea = characterArea;
        view.ringArea = ringArea;
        view.equippedArea = equippedArea;
        view.stockedArea = stockedArea;
        view.stockedContent = contentRt;
        view.equippedSlots = slots;
        view.ringSlotViews = slotViews;

        EnsurePrefabFolder();
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);

        AssetDatabase.Refresh();
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath));
        Debug.Log($"Equip panel prefab saved to {PrefabPath}. Add it under a Screen Space Canvas (e.g. UICanvas).");
    }

    static void EnsurePrefabFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
    }

    static RectTransform CreateRegion(string name, Transform parent, Color bg)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        StretchLayoutChild(rt);
        go.GetComponent<Image>().color = bg;
        return rt;
    }

    static (RectTransform root, EquipRingSlotView view) CreateSlot(Transform parent, int index, TMP_FontAsset font)
    {
        var go = new GameObject($"RingSlot_{index}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup), typeof(EquipRingSlotView));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        var frame = go.GetComponent<Image>();
        frame.color = new Color(0.16f, 0.28f, 0.4f, 0.55f);

        var vlg = go.GetComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(5, 5, 5, 5);
        vlg.spacing = 5f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 76f;
        le.preferredHeight = 132f;
        le.flexibleWidth = 1f;
        le.minWidth = 56f;

        var iconGo = new GameObject("RingIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGo.transform.SetParent(go.transform, false);
        var iconRt = iconGo.GetComponent<RectTransform>();
        StretchLayoutChild(iconRt);
        var iconImg = iconGo.GetComponent<Image>();
        iconImg.color = new Color(0.08f, 0.12f, 0.18f, 0.75f);
        iconImg.raycastTarget = true;
        iconImg.preserveAspect = true;
        var iconLe = iconGo.AddComponent<LayoutElement>();
        iconLe.preferredHeight = 58f;
        iconLe.minHeight = 48f;
        iconLe.flexibleHeight = 0f;

        var labelGo = new GameObject("GuideLabel", typeof(RectTransform), typeof(CanvasRenderer));
        labelGo.transform.SetParent(go.transform, false);
        StretchLayoutChild(labelGo.GetComponent<RectTransform>());
        var guideTmp = labelGo.AddComponent<TextMeshProUGUI>();
        guideTmp.text = $"<size=17><b>{index}</b></size>\n<size=11>{FingerGuideLabels[index]}</size>";
        guideTmp.fontSize = 12f;
        guideTmp.lineSpacing = -4f;
        guideTmp.alignment = TextAlignmentOptions.Center;
        guideTmp.enableWordWrapping = true;
        guideTmp.color = new Color(0.82f, 0.92f, 1f, 0.92f);
        if (font != null) guideTmp.font = font;
        var labelLe = labelGo.AddComponent<LayoutElement>();
        labelLe.preferredHeight = 52f;
        labelLe.flexibleHeight = 0f;
        labelLe.minHeight = 44f;

        var slotView = go.GetComponent<EquipRingSlotView>();
        slotView.slotIndex = index;
        slotView.ringIcon = iconImg;
        slotView.guideLabel = guideTmp;

        return (rt, slotView);
    }

    static void AddSectionLabel(RectTransform region, string text, TMP_FontAsset font, bool useLayoutElement = false)
    {
        var go = new GameObject("SectionLabel", typeof(RectTransform), typeof(CanvasRenderer));
        go.transform.SetParent(region.transform, false);
        var rt = go.GetComponent<RectTransform>();
        if (useLayoutElement)
        {
            StretchLayoutChild(rt);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 32f;
            le.flexibleHeight = 0f;
        }
        else
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, 32f);
            rt.anchoredPosition = Vector2.zero;
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 32f;
            le.flexibleHeight = 0f;
        }

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.margin = new Vector4(4f, 0f, 4f, 0f);
        tmp.color = new Color(0.85f, 0.94f, 1f, 0.95f);
        if (font != null) tmp.font = font;
    }

    static void AddPlaceholderRow(RectTransform parent, string text, TMP_FontAsset font)
    {
        var go = new GameObject("PlaceholderRow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        var img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.22f, 0.3f, 0.4f);
        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 40f;
        le.preferredHeight = 44f;

        var tmpGo = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer));
        tmpGo.transform.SetParent(go.transform, false);
        StretchFull(tmpGo.GetComponent<RectTransform>());
        var tmp = tmpGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.margin = new Vector4(12f, 0f, 12f, 0f);
        tmp.color = new Color(0.7f, 0.82f, 0.95f, 0.75f);
        if (font != null) tmp.font = font;
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
    }

    static void StretchLayoutChild(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
    }
}
