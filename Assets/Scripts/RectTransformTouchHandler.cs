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
    private bool isPointerDown = false;
    private Transform originalParent;
    private Transform dragParent;

    //create enum of type NoneTextSticker,Decoration,ElementSticker
    public enum StickerType
    {
        None,
        TextSticker,
        Decoration,
        ElementSticker
    }
    public StickerType stickerType = StickerType.None;       
    private void Awake()
    {
        originalParent = transform.parent;
        dragParent = originalParent.parent.GetChild(1);
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
    public void SetStickerType(StickerType type)
    {
        stickerType = type;
    }
private Vector2 pointerOffset;

    public void OnPointerDown(PointerEventData eventData)
    {
        FlowHandler.instance.dustbinImage.transform.parent.gameObject.SetActive(true);
        transform.SetParent(dragParent);
        isPointerDown = true;
        //Set as last sibling
        transform.SetAsLastSibling();
        // Calculate offset
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out pointerOffset
    );

    pointerOffset = rectTransform.anchoredPosition - pointerOffset;  
        Debug.Log("RectTransformTouchHandler: OnPointerDown");
    }


    public void OnDrag(PointerEventData eventData)
{
    if (canvas == null) return;

    Vector2 pos;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out pos
    );

  rectTransform.anchoredPosition = pos + pointerOffset;
}


    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        //if transform is placed outside the parent destroy the object
       RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
       if (parentRect != null)
       {
           // Check if overlapping with FlowHandler's dustbin
           if (FlowHandler.instance != null && FlowHandler.instance.dustbinImage != null)
           {
               RectTransform dustbinRect = FlowHandler.instance.dustbinImage.GetComponent<RectTransform>();
               if (dustbinRect != null)
               {
                   // Check if this object's bounds overlap with dustbin bounds
                   Vector3[] dustbinCorners = new Vector3[4];
                   Vector3[] objectCorners = new Vector3[4];
                   dustbinRect.GetWorldCorners(dustbinCorners);
                   rectTransform.GetWorldCorners(objectCorners);
                   
                   // Simple overlap check - if object center is within dustbin bounds
                   Vector3 objectCenter = rectTransform.position;
                   if (objectCenter.x >= dustbinCorners[0].x && objectCenter.x <= dustbinCorners[2].x &&
                       objectCenter.y >= dustbinCorners[0].y && objectCenter.y <= dustbinCorners[2].y)
                   {
                    FlowHandler.instance.dustbinImage.transform.parent.gameObject.SetActive(false);
                       Destroy(gameObject);
                       return;
                   }
               }
           }
           
           if (!parentRect.rect.Contains(rectTransform.anchoredPosition))
           {
                FlowHandler.instance.dustbinImage.transform.parent.gameObject.SetActive(false);
               Destroy(gameObject);
           }else
           {FlowHandler.instance.dustbinImage.transform.parent.gameObject.SetActive(false);
              
               transform.SetParent(originalParent);
           }
       }
    }

    private void Update()
    {
        // Only process two-finger gestures if this object is currently selected
        if (Input.touchCount == 2 && isPointerDown)
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
