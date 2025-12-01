using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgHandler : MonoBehaviour
{
   public List<Sprite> bgList;
   public Image bgImage;
   public int bgIndex = 0;
    void Start()
    {
        SetDefaultBg();
    }
    private void SetDefaultBg()
    {   bgIndex = 0;
        bgImage.sprite = bgList[0];
    }
    public void ChangeBg(int index)
    {
        AudioHandler.instance.PlaySFX("pop");
        bgIndex = index;
        bgImage.sprite = bgList[index];
        foreach (Transform child in bgImage.transform)
        {
            //chceck if child has RectTransformTouchHandler script
            RectTransformTouchHandler touchHandler = child.GetComponent<RectTransformTouchHandler>();
            if (touchHandler != null)
            {
                if(touchHandler.stickerType == RectTransformTouchHandler.StickerType.TextSticker)
                {
                    Color newColor = StickerHandler.instance.colorList[index];
                    child.GetComponent<Image>().color = newColor;
                }
            }
        }
    }

}
