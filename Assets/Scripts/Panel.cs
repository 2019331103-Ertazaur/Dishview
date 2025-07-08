using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class Panel : MonoBehaviour
{
    [field: SerializeField, Min(0)]
    protected float animationSpeed { get; private set; } = 1f;
    protected RectTransform rectTransform { get; private set; }
    protected CanvasGroup canvasGroup { get; private set; }
    public bool IsVisible { get; private set; }

    [SerializeField]
    protected UnityEvent onShow;
    public event Action OnShow;
    [SerializeField]
    protected UnityEvent onHide;
    public event Action OnHide;
    [SerializeField]
    protected UnityEvent onBack;
    public event Action OnBack;

    protected virtual void Awake()
    {
        GetCoreComponents();
        if(!IsVisible)
        {
            IsVisible = canvasGroup.alpha != 0 && canvasGroup.blocksRaycasts;
            onShow?.Invoke();
            OnShow?.Invoke();
        }
    }

    public void Show(bool instant = false)
    {
        if (IsVisible) return;
        IsVisible = true;
        gameObject.SetActive(true);
        StartCoroutine(ShowAsync(instant));
    }

    public void Hide(bool instant = false)
    {
        if (!IsVisible) return;
        IsVisible = false;
        StartCoroutine(HideAsync(instant));
    }
    
    internal IEnumerator ShowAsync(bool instant = false)
    {
        GetCoreComponents();
        var sequence = GetShowSequence(instant);
        yield return sequence.WaitForCompletion();
        canvasGroup.blocksRaycasts = true;
        onShow?.Invoke();
        OnShow?.Invoke();
    }

    internal IEnumerator HideAsync(bool instant = false)
    { 
        GetCoreComponents();
        canvasGroup.blocksRaycasts = false;
        var sequence = GetHideSequence(instant);
        yield return sequence.WaitForCompletion();
        onHide?.Invoke();
        OnHide?.Invoke();
        gameObject.SetActive(false);
    }

    protected virtual Sequence GetShowSequence(bool instant = false)
    {
        GetCoreComponents();
        var sequence = DOTween.Sequence();
        float speed = instant ? 0 : animationSpeed;
        sequence.Append(canvasGroup.DOFade(1, 0.05f * speed));
        return sequence;
    }
    
    protected virtual Sequence GetHideSequence(bool instant = false)
    {
        GetCoreComponents();
        var sequence = DOTween.Sequence();
        float speed = instant ? 0 : animationSpeed;
        sequence.Append(canvasGroup.DOFade(0, 0.05f * speed));
        return sequence;
    }

    private void GetCoreComponents()
    {
        if(!rectTransform) rectTransform = GetComponent<RectTransform>();
        if(!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Backed()
    {
        onBack?.Invoke();
        OnBack?.Invoke();
    }
}
