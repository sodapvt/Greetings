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

    public void ShowPanel(int panelIndex)
    {
        panels[activePanelIndex].SetActive(false);
        activePanelIndex = panelIndex;
        if (panelIndex >= 0 && panelIndex < panels.Count)
        {
            panels[panelIndex].SetActive(true);
        }
    }
}
