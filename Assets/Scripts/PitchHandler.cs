using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PitchHandler : MonoBehaviour
{
    public GameObject title,subtitle,playButton;
    [SerializeField] private WaveTypeWriter waveTypewriter;
    public GameObject bgmObject;
    private IEnumerator Start()
    {
        bgmObject.SetActive(true);
        waveTypewriter.ResetText();
        yield return new WaitForSeconds(0.5f);
        title.transform.localScale = Vector3.zero;
        title.SetActive(true);
        title.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(AudioHandler.instance.PlayVO("wish"));
        subtitle.SetActive(true);
        waveTypewriter.PlayPopScaleAnimation();
        yield return new WaitForSeconds(AudioHandler.instance.PlayVO("create")) ;
        playButton.transform.localScale = Vector3.zero;
        playButton.SetActive(true);
        playButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        AudioHandler.instance.PlaySFX("click");
    }
    public void OnPlayButtonPressed()
    {
        AudioHandler.instance.PlaySFX("pop");
        FlowHandler.instance.GoToNextFlow();
    }
}
