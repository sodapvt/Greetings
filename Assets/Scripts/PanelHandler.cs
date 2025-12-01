using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandler : MonoBehaviour
{
    public List<GameObject> panels;
    public int activePanelIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
    private void EnableDefaultPanel()
    {
        if (panels.Count > 0)
        {
            panels[0].SetActive(true);
            activePanelIndex = 0;
        }
    }

    public void ShowPanel(int panelIndex)
    {
        AudioHandler.instance.PlaySFX("pop");
        panels[activePanelIndex].SetActive(false);
        activePanelIndex = panelIndex;
        if (panelIndex >= 0 && panelIndex < panels.Count)
        {
            panels[panelIndex].SetActive(true);
        }
    }
    public void ClosePanel()
    {
        panels[activePanelIndex].SetActive(false);
        activePanelIndex = 0;
    }
}
