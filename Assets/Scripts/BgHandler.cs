using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgHandler : MonoBehaviour
{
   public List<Sprite> bgList;
   public Image bgImage;
    void Start()
    {
        ChangeBg(0);
    }
    public void ChangeBg(int index)
    {
        bgImage.sprite = bgList[index];
    }

}
