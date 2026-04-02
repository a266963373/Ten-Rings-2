using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HomeMenuButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Refs")]
    public RectTransform root;          // ?? LayoutGroup ??????????????????????????
    public RectTransform visual;        // ?? root ???????????????????????????
    public Image bg;
    public Image hoverGlow;
    public Image icon;
    public TMP_Text label;

    [Header("Colors")]
    // Multiply tint for HomeSceneButtonBg.png (art already has dark glass + cyan rim).
    public Color bgNormal = new Color(1f, 1f, 1f, 0.94f);
    public Color bgHover = new Color(0.88f, 0.96f, 1f, 1f);
    public Color bgPressed = new Color(0.78f, 0.86f, 0.94f, 0.96f);

    public Color textNormal = new Color(0.82f, 0.94f, 1.00f, 0.94f);
    public Color textHover = new Color(1.00f, 1.00f, 1.00f, 1.00f);

    public Color iconNormal = new Color(0.55f, 0.88f, 1.00f, 0.92f);
    public Color iconHover = new Color(0.85f, 0.98f, 1.00f, 1.00f);

    [Header("Motion")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.02f, 1.02f, 1f);
    public Vector3 pressedScale = new Vector3(0.97f, 0.97f, 1f);

    public float pressedYOffset = -2f;

    [Header("Tweening")]
    public bool useTween = true;
    public float transitionDuration = 0.12f;
    public AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Vector2 _originalPos;
    bool _isHovering;
    bool _isPressed;

    Coroutine _tweenRoutine;

    void Awake()
    {
        if (root == null) root = transform as RectTransform;

        // ????????? visual????????? root ??????????????????????????????
        if (visual == null && root.childCount > 0)
            visual = root.GetChild(0) as RectTransform;

        if (visual == null)
        {
            // ????? root????????????????????????
            visual = root;
        }

        _originalPos = visual.anchoredPosition;
        // Ensure starting state is applied instantly to avoid visible pop.
        ApplyNormalInstant();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        if (!_isPressed) ApplyHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        if (!_isPressed) ApplyNormal();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        ApplyPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        if (_isHovering) ApplyHover();
        else ApplyNormal();
    }

    // Public entry points that choose between instant and tweened transitions
    void ApplyNormal()
    {
        if (useTween) StartTransition(normalScale, _originalPos, bgNormal, 0f, textNormal, iconNormal);
        else ApplyNormalInstant();
    }

    void ApplyHover()
    {
        if (useTween) StartTransition(hoverScale, _originalPos, bgHover, 0.45f, textHover, iconHover);
        else ApplyHoverInstant();
    }

    void ApplyPressed()
    {
        Vector2 pressedPos = _originalPos + new Vector2(0f, pressedYOffset);
        if (useTween) StartTransition(pressedScale, pressedPos, bgPressed, 0.2f, textHover, iconHover);
        else ApplyPressedInstant();
    }

    void StartTransition(Vector3 targetScale, Vector2 targetAnchoredPos, Color targetBg, float targetGlowAlpha, Color targetLabel, Color targetIcon)
    {
        // Stop any running tween
        if (_tweenRoutine != null) StopCoroutine(_tweenRoutine);
        _tweenRoutine = StartCoroutine(TweenRoutine(targetScale, targetAnchoredPos, targetBg, targetGlowAlpha, targetLabel, targetIcon));
    }

    IEnumerator TweenRoutine(Vector3 targetScale, Vector2 targetAnchoredPos, Color targetBg, float targetGlowAlpha, Color targetLabel, Color targetIcon)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.0001f, transitionDuration); // avoid divide by zero

        // Capture starting values (??? visual)
        Vector3 startScale = visual.localScale;
        Vector2 startPos = visual.anchoredPosition;

        Color startBg = bg ? bg.color : Color.clear;
        float startGlow = hoverGlow ? hoverGlow.color.a : 0f;
        Color startLabel = label ? label.color : Color.clear;
        Color startIcon = icon ? icon.color : Color.clear;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float e = easing.Evaluate(t);

            // Interpolate transforms???????? visual??
            visual.localScale = Vector3.LerpUnclamped(startScale, targetScale, e);
            visual.anchoredPosition = Vector2.LerpUnclamped(startPos, targetAnchoredPos, e);

            // Interpolate colors / alpha
            if (bg) bg.color = Color.LerpUnclamped(startBg, targetBg, e);
            if (hoverGlow)
            {
                Color c = hoverGlow.color;
                c.a = Mathf.LerpUnclamped(startGlow, targetGlowAlpha, e);
                hoverGlow.color = c;
            }
            if (label) label.color = Color.LerpUnclamped(startLabel, targetLabel, e);
            if (icon) icon.color = Color.LerpUnclamped(startIcon, targetIcon, e);

            yield return null;
        }

        // Ensure final values
        visual.localScale = targetScale;
        visual.anchoredPosition = targetAnchoredPos;
        if (bg) bg.color = targetBg;
        if (hoverGlow) SetAlpha(hoverGlow, targetGlowAlpha);
        if (label) label.color = targetLabel;
        if (icon) icon.color = targetIcon;

        _tweenRoutine = null;
    }

    // Instant apply methods kept for initialization / instant mode (?????? visual)
    void ApplyNormalInstant()
    {
        visual.localScale = normalScale;
        visual.anchoredPosition = _originalPos;

        if (bg) bg.color = bgNormal;
        if (hoverGlow) SetAlpha(hoverGlow, 0f);
        if (label) label.color = textNormal;
        if (icon) icon.color = iconNormal;
    }

    void ApplyHoverInstant()
    {
        visual.localScale = hoverScale;
        visual.anchoredPosition = _originalPos;

        if (bg) bg.color = bgHover;
        if (hoverGlow) SetAlpha(hoverGlow, 0.45f);
        if (label) label.color = textHover;
        if (icon) icon.color = iconHover;
    }

    void ApplyPressedInstant()
    {
        visual.localScale = pressedScale;
        visual.anchoredPosition = _originalPos + new Vector2(0f, pressedYOffset);

        if (bg) bg.color = bgPressed;
        if (hoverGlow) SetAlpha(hoverGlow, 0.28f);
        if (label) label.color = textHover;
        if (icon) icon.color = iconHover;
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}