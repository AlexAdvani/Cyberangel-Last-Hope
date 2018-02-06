using UnityEngine;

// A generic MonoBehaviour class that allows any class inheriting it to have a static singleton instance
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	// Static Instance
	static T instance;

	// Instance - Public Read Only
	public static T Instance
	{
		get
		{
			// If there is no instance, find one in the current scene
			if (instance == null)
			{
				instance = FindObjectOfType<T>();

				// If there is no instance in the scene then log an error in the console
				if (instance == null)
				{
					Debug.Log("There is currently no object with a " + typeof(T) + " component present in the scene. Creating a new one");
					instance = CreateNewSingleton();
				}
			}

			return instance;
		}
	}

	// Initialization
	public virtual void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (instance == null)
		{
			instance = this as T;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	// Creates a new singleton object and returns the component reference. Returns null if game is being closed
	private static T CreateNewSingleton()
	{
        GameObject newObj = Resources.Load<GameObject>("Prefabs/Managers/" + typeof(T).ToString());

        if (newObj == null)
        {
            newObj = new GameObject();
            newObj.name = typeof(T).ToString();
            newObj.AddComponent<T>();
            newObj.transform.parent = GameObject.Find("GlobalManagers").transform;
        }
        else
        {
            newObj = Instantiate(newObj, GameObject.Find("GlobalManagers").transform);
        }

		return newObj.GetComponent<T>();
	}

    // Checks the instance exists and creates one if it doesn't. Used to ensure a singleton is created when it is needed.
    public static void CheckInstanceExists()
    {
        if (instance == null)
        {
            // If there is no instance, find one in the current scene
			if (instance == null)
			{
				instance = FindObjectOfType<T>();

				// If there is no instance in the scene then log an error in the console
				if (instance == null)
				{
					Debug.Log("There is currently no object with a " + typeof(T) + " component present in the scene. Creating a new one");
					instance = CreateNewSingleton();
				}
			}
        }
    }

	// Destroys object and clears the static instance
	protected void DestroySingleton()
	{
		if (instance == null)
		{
			return;
		}

		instance = null;
		Destroy(gameObject);
	}
}