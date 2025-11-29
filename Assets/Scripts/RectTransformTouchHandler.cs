using UnityEngine;
using UnityEngine.EventSystems;

public class RectTransformTouchHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    // Minimum scale limit
    public float minScale = 0.5f;
    // Maximum scale limit
    public float maxScale = 3.0f;

    private float initialDistance;
    private Vector3 initialScale;
    private float initialAngle;
    private Quaternion initialRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("RectTransformTouchHandler: No Canvas found in parent! Please ensure this object is a child of a Canvas.");
        }
        else
        {
            Debug.Log($"RectTransformTouchHandler: Initialized. Canvas Scale Factor: {canvas.scaleFactor}");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Set as last sibling
        transform.SetAsLastSibling();
        Debug.Log("RectTransformTouchHandler: OnPointerDown");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Only allow single finger drag if we are not pinching/rotating
        if (Input.touchCount < 2)
        {
            if (canvas == null) 
            {
                Debug.LogWarning("RectTransformTouchHandler: Canvas is null, cannot drag.");
                return;
            }

            //Debug.Log($"RectTransformTouchHandler: Dragging. Delta: {eventData.delta}");
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       //if transform is placed outside the parent destroy the object
       RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
       if (parentRect != null)
       {
           if (!parentRect.rect.Contains(rectTransform.anchoredPosition))
           {
               Destroy(gameObject);
           }
       }
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Handle Zoom (Scale)
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch0.position, touch1.position);
                initialScale = rectTransform.localScale;
                
                // Calculate initial angle for rotation
                Vector2 v2 = touch0.position - touch1.position;
                initialAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                initialRotation = rectTransform.localRotation;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                // --- ZOOM ---
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                if (Mathf.Approximately(initialDistance, 0)) return;

                float factor = currentDistance / initialDistance;
                Vector3 newScale = initialScale * factor;

                // Clamp scale
                newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
                newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
                newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

                rectTransform.localScale = newScale;

                // --- ROTATE ---
                Vector2 v2 = touch0.position - touch1.position;
                float currentAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                float angleDelta = currentAngle - initialAngle;

                rectTransform.localRotation = initialRotation * Quaternion.Euler(0, 0, angleDelta);
            }
        }
    }
}
