using System.Collections;

using UnityEngine;

using BeautifulTransitions.Scripts.Transitions;

public class SaveNotificationUI : MonoBehaviour
{
    // Static Instance
    static SaveNotificationUI instance;

    // Base Transform Object
    public GameObject goBaseObject;
    // Notification Dismiss Coroutine
    Coroutine dismissCoroutine;

    #region Public Properties

    // Static Instance
    public static SaveNotificationUI Instance
    {
        get { return instance; }
    }

    #endregion

    // Initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(goBaseObject);
        }
    }

    // Opens the Save Notification
    public static void OpenSaveNotification()
    {
        if (instance == null)
        {
            return;
        }

        instance.gameObject.SetActive(true);
        instance.TransitionSaveNotification();
    }

    // Begins the Transition In and starts the Dismiss Coroutine
    private void TransitionSaveNotification()
    {
        TransitionHelper.TransitionIn(gameObject);

        if (dismissCoroutine != null)
        {
            StopCoroutine(dismissCoroutine);
            dismissCoroutine = null;
        }

        dismissCoroutine = StartCoroutine(DismissSaveNotification());
    }

    // Dismisses the save notification after 1 second
    private IEnumerator DismissSaveNotification()
    {
        yield return new WaitForSecondsRealtime(1);

        TransitionHelper.TransitionOut(gameObject, CloseSaveNotification);
    }

    // Disables the notification gameobject
    private void CloseSaveNotification()
    {
        gameObject.SetActive(false);
    }
}
