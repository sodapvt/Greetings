using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PreviewGreeting : MonoBehaviour
{
    private Vector3 originalPosition;
    private float originalSize;
    public GameObject Block,panelOne,PanelTwo;
    private float duration = 0.25f;
    private void Start()
    {
        Block.SetActive(false);
    }
    public void OnPreviewGreetingClick()
    {
       AudioHandler.instance.PlaySFX("pop");
        originalPosition = Camera.main.transform.position;
        originalSize = Camera.main.orthographicSize;
        CameraPosition();
        //set size
        CameraSize();
    }
    public void ResetCamera()
    {
        AudioHandler.instance.PlaySFX("pop");
        //Camera.main.transform.position = originalPosition;
        Camera.main.transform.DOMove(originalPosition, duration).SetEase(Ease.Linear);
        //set size
       // Camera.main.orthographicSize = originalSize;
        Camera.main.DOOrthoSize(originalSize, duration).SetEase(Ease.Linear).OnComplete(()=> {
            panelOne.SetActive(true);
            PanelTwo.SetActive(true);
            Block.SetActive(false);
            });
    } 
    public Image previewImage,offestImage;
    private void CameraSize()
    {
        if (previewImage == null) return;
        //adjust camera size so that image fits perfectly in the screen
        float imageWidth = previewImage.rectTransform.rect.width * previewImage.transform.lossyScale.x;
        float imageHeight = previewImage.rectTransform.rect.height * previewImage.transform.lossyScale.y;
        float offestWidth = offestImage.rectTransform.rect.width * offestImage.transform.lossyScale.x;
        float offestHeight = offestImage.rectTransform.rect.height * offestImage.transform.lossyScale.y;
        if(!FlowHandler.instance.doctorNameEntered)
        {
            offestHeight = 0;
        }
        // Calculate orthographic size needed for both width and height to fit
        float sizeForWidth = imageWidth / (2f * Camera.main.aspect);
        float sizeForHeight = (imageHeight+offestHeight) / 2f;
        
        // Use the larger size to ensure both dimensions fit
       // Camera.main.orthographicSize = Mathf.Max(sizeForWidth, sizeForHeight);
        Camera.main.DOOrthoSize(Mathf.Max(sizeForWidth, sizeForHeight), duration).SetEase(Ease.Linear).OnComplete(()=> {
             panelOne.SetActive(false);
        PanelTwo.SetActive(false);
        Block.SetActive(true);
            });
    } 
    public Vector2 offest;
    private void CameraPosition()
    {
        if (previewImage == null) return;
        Vector3 newPos = previewImage.transform.position;
        if(FlowHandler.instance.doctorNameEntered)
        {
            newPos.y += offest.y;
        }
        newPos.z=-10;
        //Camera.main.transform.position = newPos;
        //use dotween to move camera to new position
        Camera.main.transform.DOMove(newPos, duration).SetEase(Ease.Linear);
    } 

}
