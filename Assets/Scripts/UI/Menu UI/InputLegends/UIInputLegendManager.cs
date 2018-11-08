using System.Collections.Generic;
using UnityEngine;

using Rewired;

public class UIInputLegendManager : MonoBehaviour
{
    // Dictionary of Legend Input Elements 
    Dictionary<string, UIInputLegendImage> dLegendElements;

    // UI Legend Prefab
    public GameObject goLegendPrefab;
    // Default Legends to Create on Awake
    public UILegendData[] defaultLegends;

    // Initialization
    void Awake ()
    {
        if (dLegendElements != null)
        {
            return;
        }

        dLegendElements = new Dictionary<string, UIInputLegendImage>();

        for (int i = 0; i < defaultLegends.Length; i++)
        {
            int index = i;
            AddLegend(defaultLegends[index].iActionID, defaultLegends[index].sInputText);
        }
	}

    // Get Legend
    public UIInputLegendImage GetLegend(string legendName)
    {
        if (!dLegendElements.ContainsKey(legendName))
        {
            Debug.LogError("UI Legend: " + legendName + " does not exist.");
            return null;
        }

        return dLegendElements[legendName];
    }

    // Add Legend 
    public void AddLegend(int actionID, string inputText)
    {
        if (dLegendElements == null)
        {
            Awake();
        }

        if (dLegendElements.ContainsKey(inputText))
        {
            return;
        }

        GameObject obj = Instantiate(goLegendPrefab, transform);
        UIInputLegendImage legend = obj.GetComponent<UIInputLegendImage>();
        legend.iInputActionID = actionID;
        legend.descriptiveText.text = inputText;

        dLegendElements.Add(inputText, legend);
    }

    // Remove Legend
    public void RemoveLegend(string legendName)
    {
        if (!dLegendElements.ContainsKey(legendName))
        {
            Debug.LogError("UI Legend: " + legendName + " does not exist.");
            return;
        }

        GameObject obj = dLegendElements[legendName].gameObject;
        dLegendElements.Remove(legendName);
        Destroy(obj);
    }

    // Remove All Legends
    public void RemoveAllLegends()
    {
        dLegendElements.Clear();
    }
}

[System.Serializable]
public struct UILegendData
{
    // Input Text
    public string sInputText;
    // Input Action ID
    [ActionIdProperty(typeof(RewiredActions))]
    public int iActionID;
}
