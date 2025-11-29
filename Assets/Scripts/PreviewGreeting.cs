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
       
        originalPosition = Camera.main.transform.position;
        originalSize = Camera.main.orthographicSize;
        CameraPosition();
        //set size
        CameraSize();
    }
    public void ResetCamera()
    {
       
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
    public Image previewImage;
    private void CameraSize()
    {
        if (previewImage == null) return;
        //adjust camera size so that image fits perfectly in the screen
        float imageWidth = previewImage.rectTransform.rect.width * previewImage.transform.lossyScale.x;
        float imageHeight = previewImage.rectTransform.rect.height * previewImage.transform.lossyScale.y;
        
        // Calculate orthographic size needed for both width and height to fit
        float sizeForWidth = imageWidth / (2f * Camera.main.aspect);
        float sizeForHeight = imageHeight / 2f;
        
        // Use the larger size to ensure both dimensions fit
       // Camera.main.orthographicSize = Mathf.Max(sizeForWidth, sizeForHeight);
        Camera.main.DOOrthoSize(Mathf.Max(sizeForWidth, sizeForHeight), duration).SetEase(Ease.Linear).OnComplete(()=> {
             panelOne.SetActive(false);
        PanelTwo.SetActive(false);
        Block.SetActive(true);
            });
    } 
    private void CameraPosition()
    {
        if (previewImage == null) return;
        Vector3 newPos = previewImage.transform.position;
        newPos.z = -10;
        //Camera.main.transform.position = newPos;
        //use dotween to move camera to new position
        Camera.main.transform.DOMove(newPos, duration).SetEase(Ease.Linear);
    } 

}
