using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FlowHandler : MonoBehaviour
{
    public static FlowHandler instance;
    public GameObject[] flowElements;
    public GameObject overlayBG,greetingBG;
    private int currentElementIndex = 0;
    public SpriteRenderer previewImage;
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

    public void GoToNextFlow()
    {
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
            greetingBG.GetComponent<Image>().DOFade(1.0f, 1f);
            overlayBG.SetActive(true);
            greetingBG.SetActive(true);
        }
        foreach (GameObject element in flowElements)
        {
            element.SetActive(false);
        }
        flowElements[currentElementIndex].SetActive(true);
    }
}
