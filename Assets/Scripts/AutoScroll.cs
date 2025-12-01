using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float fromPos = 1.0f;
    [SerializeField] private float toPos = 0f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    private void OnEnable()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
        AnimateScroll();
    }

    private void AnimateScroll()
    {
        if (scrollRect != null)
        {
            // Set starting position
            scrollRect.horizontalNormalizedPosition = fromPos;
            
            // Animate to target position
            scrollRect.DOHorizontalNormalizedPos(toPos, duration).SetEase(easeType);
        }
    }
}
