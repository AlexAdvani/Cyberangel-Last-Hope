using System.Collections.Generic;

using UnityEngine;

public class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
{
    // Dictionary of Object Pools
    Dictionary<string, ObjectPool> dObjectPools;

    #region Public Properties

    // Dictionary of Object Pools
    public Dictionary<string, ObjectPool> ObjectPools
    {
        get
        {
            if (dObjectPools == null)
            {
                dObjectPools = new Dictionary<string, ObjectPool>();
            }

            return dObjectPools;
        }
    }

    // Number of Object Pools
    public int PoolCount
    {
        get
        {
            if (dObjectPools == null)
            {
                dObjectPools = new Dictionary<string, ObjectPool>();
            }

            return dObjectPools.Count;
        }
    }

    #endregion

    // Initialization
    public override void Awake()
    {
        base.Awake();

        dObjectPools = new Dictionary<string, ObjectPool>();
    }

    #region Pool Management

    // Creates a new Object Pool
    public void CreatePool(string poolName, GameObject poolPrefab, int poolSize, bool expandable = false)
    {
        if (dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError(poolName + " is already an object pool in use.");
            return;
        }

        if (poolPrefab == null)
        {
            Debug.LogError("Cannot create the pool: " + poolName + " because the prefab is null.");
            return;
        }

        if (poolSize < 0)
        {
            Debug.LogError("Cannot create the pool: " + poolName + " because the pool size is not valid.");
            return;
        }

        ObjectPool pool = new ObjectPool(poolPrefab, poolSize, poolName);
        dObjectPools.Add(poolName, pool);
        pool.bExpandable = expandable;
    }

    // Gets an Object Pool and returns it
    public ObjectPool GetObjectPool(string poolName)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError("Cannot return the pool: " + poolName + " because the pool does not exist");
            return null;
        }

        return dObjectPools[poolName];
    }

    // Removes an Object Pool
    public void RemovePool(string poolName)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError("Cannot remove the pool: " + poolName + " because the pool does not exist");
            return;
        }

        dObjectPools[poolName].DestroyPool();
        dObjectPools.Remove(poolName);
    }

    // Clears the dictionary of all Object Pools
    public void ClearPools()
    {
        if (dObjectPools.Count > 0)
        {
            foreach (ObjectPool objectPool in dObjectPools.Values)
            {
                objectPool.DestroyPool();
            }
        }

        dObjectPools.Clear();
    }

    #endregion

    #region Object Management

    // Finds the first free object of an object pool and returns it
    public GameObject GOGetFreeObject(string poolName, out int objID)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            objID = -1;
            Debug.LogError("Cannot get an object from the pool: " + poolName + " because the pool does not exist");
            return null;
        }

        for (int i = 0; i < dObjectPools[poolName].PoolSize; i++)
        {
            if (!dObjectPools[poolName].PooledObjects[i].activeInHierarchy)
            {
                objID = i;
                return dObjectPools[poolName].PooledObjects[i];
            }
        }

        objID = -1;
        return null;
    }

    // Returns the number of free objects in an object pool
    public int IGetFreeObjects(string poolName)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError("Cannot get objects from the pool: " + poolName + " because the pool does not exist");
            return 0;
        }

        int freeObjects = 0;

        for (int i = 0; i < dObjectPools[poolName].PoolSize; i++)
        {
            if (!dObjectPools[poolName].PooledObjects[i].activeInHierarchy)
            {
                freeObjects++;
            }
        }

        return freeObjects;
    }

    // Returns the number of active objects in an object pool
    public int IGetActiveObjects(string poolName)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError("Cannot get objects from the pool: " + poolName + " because the pool does not exist");
            return 0;
        }

        int activeObjects = 0;

        for (int i = 0; i < dObjectPools[poolName].PoolSize; i++)
        {
            if (dObjectPools[poolName].PooledObjects[i].activeInHierarchy)
            {
                activeObjects++;
            }
        }

        return activeObjects;
    }

    // Returns the object at the specified position from an object pool
    public GameObject GOGetObjectAt(string poolName, int id)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError("Cannot get objects from the pool: " + poolName + " because the pool does not exist");
            return null;
        }

        if (id < 0 || id >= dObjectPools[poolName].PoolSize)
        {
            Debug.LogError("Cannot retreive object: " + id + " because object is out of scope");
            return null;
        }

        return dObjectPools[poolName].PooledObjects[id];
    }

    // Spawns an object from an object pool at the specified position and rotation
    public GameObject GOSpawnObject(string poolName, Vector3 position, Quaternion rotation)
    {
        if (!dObjectPools.ContainsKey(poolName))
        {
            Debug.LogError("Cannot get objects from the pool: " + poolName + " because the pool does not exist");
            return null;
        }

        int objectID;
        GameObject spawnObject = GOGetFreeObject(poolName, out objectID);

        if (spawnObject != null)
        {
            spawnObject.transform.SetPositionAndRotation(position, rotation);
            spawnObject.SetActive(true);
        }
        else if (dObjectPools[poolName].bExpandable)
        {
            spawnObject = dObjectPools[poolName].GOCreateNewObject();
        }

        return spawnObject;
    }

    // Spawns an object from an object pool at the specified position and rotation and returns the objects' position in the pool
    public GameObject GOSpawnObject(string poolName, Vector3 position, Quaternion rotation, out int objID)
    {
        GameObject spawnObject = GOGetFreeObject(poolName, out objID);

        if (spawnObject != null)
        {
            spawnObject.transform.SetPositionAndRotation(position, rotation);
            spawnObject.SetActive(true);
        }
        else if (dObjectPools[poolName].bExpandable)
        {
            spawnObject = dObjectPools[poolName].GOCreateNewObject();
        }

        return spawnObject;
    }

    #endregion
}

// Pool of Gameobjects
public class ObjectPool
{
    // Object Pool Prefab 
    GameObject goPoolPrefab;

    // Pool Parent Transform
    Transform tParentTransform;

    // Pooled Object List
    List<GameObject> lgoPooledObjects;
    // Pool Size
    int iPoolSize;
    // Expandable flag
    public bool bExpandable = false;

    #region Public Properties

    // Pooled Object List
    public List<GameObject> PooledObjects
    {
        get { return lgoPooledObjects; }
    }

    // Pool Size
    public int PoolSize
    {
        get { return iPoolSize; }
    }

    #endregion

    // Constructor
    public ObjectPool(GameObject prefab, int poolSize, string poolName) 
    {
        InitializePool(prefab, poolSize, poolName);
    }

    // Initializes the Object Pool and populates it with objects
    private void InitializePool(GameObject prefab, int poolSize, string poolName)
    {
        lgoPooledObjects = new List<GameObject>();
        iPoolSize = poolSize;
        goPoolPrefab = prefab;

        GameObject parentObj = new GameObject();
        parentObj.name = poolName;
        parentObj.transform.SetParent(ObjectPoolManager.Instance.transform);
        tParentTransform = parentObj.transform;

        for (int i = 0; i < iPoolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(goPoolPrefab, tParentTransform);
            obj.SetActive(false);

            lgoPooledObjects.Add(obj);
        }
    }

    // Sets a new pool size
    public void SetPoolSize(int newSize)
    {
        if (newSize < 0 || newSize == lgoPooledObjects.Count)
        {
            return;
        }

        iPoolSize = newSize;

        if (newSize < lgoPooledObjects.Count)
        {
            for (int i = 0; i < lgoPooledObjects.Count - newSize; i++)
            {
                GameObject.Destroy(lgoPooledObjects[i]);
            }

            lgoPooledObjects.RemoveRange(newSize - 1, lgoPooledObjects.Count - newSize);
        }
        else
        {
            for (int i = 0; i < newSize - lgoPooledObjects.Count; i++)
            { 
                GOCreateNewObject();
            }     
        }
    }

    // Creates a new object in the pool if there is room for one or if the pool is expandable
    public GameObject GOCreateNewObject()
    {
        if (!bExpandable && lgoPooledObjects.Count < iPoolSize)
        {
            return null;
        }

        GameObject obj = GameObject.Instantiate(goPoolPrefab, tParentTransform);
        obj.SetActive(false);

        lgoPooledObjects.Add(obj);

        if (lgoPooledObjects.Count > iPoolSize)
        {
            iPoolSize++;
        }

        return obj;
    }

    // Clears the pool of all objects 
    public void ClearPool()
    {
        lgoPooledObjects.Clear();
        SetPoolSize(0);
    }

    // Destroys the Object Pool Parent
    public void DestroyPool()
    {
        GameObject.Destroy(tParentTransform.gameObject);
    }
}