using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GoogleSheetUploader : MonoBehaviour
{
    [SerializeField] private string googleWebAppURL = "YOUR_WEB_APP_URL_HERE";
    [SerializeField] private string googleSheetURL = "YOUR_SHEET_URL_HERE";

    private Queue<Record> retryQueue = new Queue<Record>();
    private bool isUploading = false;

    // -------------------------------
    //  USER PRESSED SUBMIT BUTTON
    // -------------------------------
    public void OnSubmit()
    {
        AudioHandler.instance.PlaySFX("pop");
        
        // Trim whitespace and check if fields are empty
        string drName = formHandler.drName.text.Trim();
        string drEmail = formHandler.drEmail.text.Trim();
        string drContact = formHandler.drContact.text.Trim();
        string mrName = formHandler.mrName.text.Trim();
        string mrID = formHandler.mrID.text.Trim();
        string country = formHandler.countryDropdown.options[formHandler.countryDropdown.value].text;
        string division = formHandler.divisionDropdown.options[formHandler.divisionDropdown.value].text;
        Debug.Log($"Form Data Collected:\nDr Name: {drName}\nDr Email: {drEmail}\nDr Contact: {drContact}\nMR Name: {mrName}\nMR ID: {mrID}\nCountry: {country}\nDivision: {division}");
        if (string.IsNullOrWhiteSpace(mrName) || string.IsNullOrWhiteSpace(drName) || string.IsNullOrWhiteSpace(mrID))
        {
            Debug.LogError("FormHandler: Please fill all fields!");
            AndroidToaster.ShowToast("Please fill all fields!");
            return;
        }
        string deviceId =  AndroidDeviceInfo.GetAndroidId();
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");

        // Collect required fields (using trimmed values)
        Record data = new Record(
            deviceId,
            currentDate,
            currentTime,
            drName,
            drEmail,
            drContact,
            mrName,
            mrID,
            country,
            division
        );
FlowHandler.instance.drName = drName;   
        AndroidToaster.ShowToast("Saving...");
        formHandler.loadingPanel.SetActive(true);
        retryQueue.Enqueue(data);

        StartCoroutine(PostRequestWithTimeout(data, 10f));
    }
public FormHandler formHandler;
    // -----------------------------------------
    //  MAIN SENDER WITH TIMEOUT + LOADING UI
    // -----------------------------------------
    IEnumerator PostRequestWithTimeout(Record data, float maxWaitTime)
    {
        isUploading = true;

        // Internet check
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            AndroidToaster.ShowToast("No internet. Retrying later...");
            yield return new WaitForSeconds(5);
            isUploading = false;
            TryNextUpload();
            LoadNextStep();
            yield break;
        }

        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(googleWebAppURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = (int)maxWaitTime;

            float elapsed = 0f;
            var op = request.SendWebRequest();

            while (!op.isDone && elapsed < maxWaitTime)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Upload Success! Response: {request.downloadHandler.text}");
                AndroidToaster.ShowToast("Submitted Successfully!");
                retryQueue.Dequeue();
            }
            else
            {
                // Detailed error logging
                string errorMsg = $"Upload Failed!\n" +
                                 $"Error: {request.error}\n" +
                                 $"Response Code: {request.responseCode}\n" +
                                 $"Response: {request.downloadHandler.text}";
                Debug.LogError(errorMsg);
                
                AndroidToaster.ShowToast("Failed. Will retry in background...");
                yield return new WaitForSeconds(5);
            }
        }

        isUploading = false;
        TryNextUpload();
        LoadNextStep();
    }

    // -----------------------------------
    //  BACKGROUND RETRY
    // -----------------------------------
    private void TryNextUpload()
    {
        if (!isUploading && retryQueue.Count > 0)
        {
            StartCoroutine(RetryUpload(retryQueue.Peek()));
        }
    }

    IEnumerator RetryUpload(Record data)
    {
        isUploading = true;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForSeconds(5);
            isUploading = false;
            TryNextUpload();
            yield break;
        }

        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(googleWebAppURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Retry Success! Response: {request.downloadHandler.text}");
                AndroidToaster.ShowToast("Retry Successful!");
                retryQueue.Dequeue();
            }
            else
            {
                Debug.LogError($"Retry Failed! Error: {request.error}, Code: {request.responseCode}");
                yield return new WaitForSeconds(5);
            }
        }

        isUploading = false;
        TryNextUpload();
        LoadNextStep();
    }

    // Called when data is successfully uploaded
    private void LoadNextStep()
    {
        formHandler.loadingPanel.SetActive(false);
        FlowHandler.instance.GoToNextFlow();
        // Add any additional success handling here
    }
}


// ---------------------------------------------------
// DATA CLASS FOR THE GOOGLE SHEET JSON FORMAT
// ---------------------------------------------------
[System.Serializable]
public class Record
{
    public string deviceId;
    public string date;
    public string time;
    public string drName;
    public string drEmail;
    public string drContact;
    public string mrName;
    public string employeeID;
    public string country;
    public string division;

    public Record(
        string deviceId,
        string date,
        string time,
        string drName,
        string drEmail,
        string drContact,
        string mrName,
        string employeeID,
        string country,
        string division)
    {
        this.deviceId = deviceId;
        this.date = date;
        this.time = time;
        this.drName = drName;
        this.drEmail = drEmail;
        this.drContact = drContact;
        this.mrName = mrName;
        this.employeeID = employeeID;
        this.country = country;
        this.division = division;
    }
}
