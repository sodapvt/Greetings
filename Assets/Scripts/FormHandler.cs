using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FormHandler : MonoBehaviour
{
    public TextMeshProUGUI mrName,drName,password;
    public void SubmitForm()
    {
        if(mrName.text.Length == 0 || drName.text.Length == 0 || password.text.Length == 0)
        {
            Debug.LogError("FormHandler: Please fill all fields!");
            return;
        }
        FlowHandler.instance.GoToNextFlow();
    }
}
