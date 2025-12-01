using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowHandler : MonoBehaviour
{
    public static FlowHandler instance;
    public GameObject[] flowElements;
    public GameObject overlayBG,greetingBG;
    private int currentElementIndex = 0;
    public SpriteRenderer previewImage;
    public static bool textWaveFinished = false;
    [HideInInspector]public string drName = "";
    public Image dustbinImage;
    private void CameraSize()
    {
        if (previewImage == null) return;
        //adjust camera size so that image fits perfectly in the screen
        float imageWidth = previewImage.bounds.size.x;
        float imageHeight = previewImage.bounds.size.y;
        
        // Calculate orthographic size needed for both width and height to fit
        float sizeForWidth = imageWidth / (2f * Camera.main.aspect);
        float sizeForHeight = imageHeight / 2f;
        
        // Use the larger size to ensure both dimensions fit
        Camera.main.orthographicSize = Mathf.Max(sizeForWidth, sizeForHeight);
    } 
    private void Awake()
    {
        //set application refreshrate to 60
        Application.targetFrameRate = 120;
        overlayBG.SetActive(false);
        greetingBG.SetActive(false);
        instance = this;
        CameraSize();
    }
    void Start()
    {
        if(flowElements.Length == 0)
        {
            Debug.LogError("FlowHandler: No flow elements found!");
        }
        else
        {
            flowElements[currentElementIndex].SetActive(true);
        }
    }
public GameObject bgScroll;
public bool doctorNameEntered = false;
    public void GoToNextFlow()
    {
        float duration =0.75f;
        currentElementIndex++;
        if (currentElementIndex >= flowElements.Length)
        {
            Debug.LogError("FlowHandler: No more elements to show!");
            currentElementIndex = 0;
        }
        if(currentElementIndex == 0 || currentElementIndex == 1)
        {
            overlayBG.SetActive(false);
            greetingBG.SetActive(false);
        }
        else
        {
            overlayBG.GetComponent<SpriteRenderer>().DOFade(0.7f, 1f);
            RectTransform greetingRect = greetingBG.GetComponent<RectTransform>();
            greetingRect.anchoredPosition= new Vector2(-Screen.width-greetingRect.rect.width, 312);
            overlayBG.SetActive(true);
            greetingBG.SetActive(true);
            // Clean the drName by removing invisible characters and whitespace
            string cleanDrName = drName.Trim().Replace("\u200B", "").Replace("\u200C", "").Replace("\u200D", "").Replace("\uFEFF", "");
            Debug.Log("FlowHandler: CleanDrName: '" + cleanDrName + "' Length: " + cleanDrName.Length);
            
            if(cleanDrName != "")
            {
                //set greetingBG's child text to drName
                TextMeshProUGUI drNameText = greetingBG.GetComponentInChildren<TextMeshProUGUI>();
                drNameText.text = "Best Wishes : " + cleanDrName;
                doctorNameEntered = true;
            }else{
               greetingBG.transform.GetChild(0).gameObject.SetActive(false); 
            }
            greetingBG.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 312), duration).SetEase(Ease.OutBack);

            RectTransform mainPanelRect= flowElements[currentElementIndex].GetComponent<RectTransform>();
            mainPanelRect.anchoredPosition= new Vector2(Screen.width+mainPanelRect.rect.width, 0);
            mainPanelRect.DOAnchorPos(new Vector2(0, 0), duration).SetEase(Ease.OutBack).OnComplete(()=> {
                bgScroll.SetActive(true);
            });
        }
        foreach (GameObject element in flowElements)
        {
            element.SetActive(false);
        }
        flowElements[currentElementIndex].SetActive(true);
    }
}
