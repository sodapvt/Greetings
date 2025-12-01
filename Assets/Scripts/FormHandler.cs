using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FormHandler : MonoBehaviour
{
    private List<string> Country = new List<string>()
    {
    "Kenya",
    "Uganda",
    "Tanzania",
    "Zambia",
    "Srilanka",
    "Cambodia",
    "Myanmar",
    "Nepal",
    "Others"
    };
    private List<string> Division = new List<string>()
    {
        "3D",
        "Neo",
        "Femicare",
        "Curis",
        "Discovery",
        "Others"
    };
    public TMP_Dropdown countryDropdown,divisionDropdown;
    public TextMeshProUGUI drName,drEmail,drContact,mrName,mrID;
    public GameObject loadingPanel;
    private void Start()
    {
        SetupCountryDropdown();
        SetupDivisionDropdown();
    }   
    private void SetupCountryDropdown()
    {
        countryDropdown.ClearOptions();
        countryDropdown.AddOptions(Country);
    }
    private void SetupDivisionDropdown()
    {
        divisionDropdown.ClearOptions();
        divisionDropdown.AddOptions(Division);
    }
    public void SubmitForm()
    {
        
        AndroidToaster.ShowToast("Data saved. Uploading...");
        loadingPanel.SetActive(true);
        AudioHandler.instance.PlaySFX("pop");
    }
    public void OnDataSuccessUploaded()
    {
        loadingPanel.SetActive(false);
        AndroidToaster.ShowToast("Submitted successfully!");
         FlowHandler.instance.GoToNextFlow();
    }
}
