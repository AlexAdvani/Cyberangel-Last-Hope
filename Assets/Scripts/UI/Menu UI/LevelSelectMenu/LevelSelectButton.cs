using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    // Level ID 
    int iLevelID;

    // Level Button Component
    Button levelButton;
    // Level Image
    public Image levelImage;
    // Level Number Text
    public TextMeshProUGUI levelNumberText;

    #region Public Properties

    // Level ID
    public int LevelID
    {
        get { return iLevelID; }
    }

    // Level Button Component
    public Button LevelButton
    { 
        get { return levelButton; }
    }

    #endregion

    // On Destroy 
    void OnDestroy()
    {
        GetComponent<EventTrigger>().triggers.Clear();
    }

    // Initialize Level Button
    public void InitializeButton(Sprite image, int levelID, UnityAction<int> clickAction, 
        UnityAction<int> mouseEnterAction, UnityAction mouseExitAction)
    {
        // Level Data
        iLevelID = levelID;
        levelImage.sprite = image;
        levelNumberText.text = (levelID + 1).ToString();

        //Button Click
        levelButton = gameObject.GetComponent<Button>();
        levelButton.onClick.AddListener(delegate { clickAction(levelID); });

        // Button Pointer Enter
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((eventData) => { mouseEnterAction(levelID); });
        eventTrigger.triggers.Add(enterEntry);

        // Button Pointer Exit
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((eventData) => { mouseExitAction(); });
        eventTrigger.triggers.Add(exitEntry);
    }
}
