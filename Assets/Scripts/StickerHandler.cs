using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StickerHandler : MonoBehaviour
{
    public static StickerHandler instance;
    public GameObject stickerPrefab;
    public Transform stickerParent;
    public List<Sprite> stickerList;
    public List<Sprite> elementList;
    public List<Sprite> textList;
    public List<Color> colorList;
    public BgHandler bgHandler;

    private void Awake()
    {
        instance = this;
    }

    public void AddSticker(int index)
    {
        RectTransform stickerParentRect = stickerParent.GetComponent<RectTransform>();
        GameObject sticker = Instantiate(stickerPrefab, stickerParent);
        sticker.GetComponent<Image>().sprite = stickerList[index];
        
        RectTransform stickerRect = sticker.GetComponent<RectTransform>();
        //set the sticker size as image native size
        stickerRect.sizeDelta = sticker.GetComponent<Image>().sprite.rect.size;
        
        float x = Random.Range(stickerParentRect.rect.xMin + stickerRect.sizeDelta.x / 2, stickerParentRect.rect.xMax - stickerRect.sizeDelta.x / 2);
        float y = Random.Range(stickerParentRect.rect.yMin + stickerRect.sizeDelta.y / 2, stickerParentRect.rect.yMax - stickerRect.sizeDelta.y / 2);
        stickerRect.anchoredPosition = new Vector2(x, y);
        sticker.transform.localScale = Vector3.zero;
        sticker.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        SetupTouchHandler(sticker);
    }
    public void AddElement(int index)
    {
        RectTransform stickerParentRect = stickerParent.GetComponent<RectTransform>();
        GameObject element = Instantiate(stickerPrefab, stickerParent);
        element.GetComponent<Image>().sprite = elementList[index];
        
        RectTransform elementRect = element.GetComponent<RectTransform>();
        //set the element size as image native size
        elementRect.sizeDelta = element.GetComponent<Image>().sprite.rect.size;
        
        float x = Random.Range(stickerParentRect.rect.xMin + elementRect.sizeDelta.x / 2, stickerParentRect.rect.xMax - elementRect.sizeDelta.x / 2);
        float y = Random.Range(stickerParentRect.rect.yMin + elementRect.sizeDelta.y / 2, stickerParentRect.rect.yMax - elementRect.sizeDelta.y / 2);
        elementRect.anchoredPosition = new Vector2(x, y);
        element.transform.localScale = Vector3.zero;
        element.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        
        SetupTouchHandler(element);
    }
    public void AddText(int index)
    {
        RectTransform stickerParentRect = stickerParent.GetComponent<RectTransform>();
        GameObject text = Instantiate(stickerPrefab, stickerParent);
        text.GetComponent<Image>().sprite = textList[index];
        text.GetComponent<Image>().color = colorList[bgHandler.bgIndex];
        
        RectTransform textRect = text.GetComponent<RectTransform>();
        //set the text size as image native size
        textRect.sizeDelta = text.GetComponent<Image>().sprite.rect.size;
        
        float x = Random.Range(stickerParentRect.rect.xMin + textRect.sizeDelta.x / 2, stickerParentRect.rect.xMax - textRect.sizeDelta.x / 2);
        float y = Random.Range(stickerParentRect.rect.yMin + textRect.sizeDelta.y / 2, stickerParentRect.rect.yMax - textRect.sizeDelta.y / 2);
        textRect.anchoredPosition = new Vector2(x, y);
        text.transform.localScale = Vector3.zero;
        text.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        text.GetComponent<RectTransformTouchHandler>().SetStickerType(RectTransformTouchHandler.StickerType.TextSticker);
        SetupTouchHandler(text);
    }

    private void SetupTouchHandler(GameObject obj)
    {
        if (obj.GetComponent<RectTransformTouchHandler>() == null)
        {
            obj.AddComponent<RectTransformTouchHandler>();
        }
        
        Image img = obj.GetComponent<Image>();
        if (img != null)
        {
            img.raycastTarget = true;
        }
    }
}
