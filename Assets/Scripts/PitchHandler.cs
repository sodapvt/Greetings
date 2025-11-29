using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchHandler : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        FlowHandler.instance.GoToNextFlow();
    }
}
