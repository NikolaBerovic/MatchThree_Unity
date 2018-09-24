using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{

    ///<summary> 
    ///<para>Instaniates amout of prefab into pool</para>
    ///<para>Returns pool transform</para>
    ///</summary>
    public Transform CreatePool(GameObject childPrefab, int amount)
    {
        if (childPrefab == null)
            { Debug.Log("Child prefab not set!"); }

        GameObject poolParent = new GameObject();
        poolParent.transform.SetParent(transform);
        poolParent.name = childPrefab.name + "Pool";

        for (int i = 0; i < amount; i++)
        {
            InstantiateChild(childPrefab, poolParent.transform);
        }

        return poolParent.transform;
    }

    ///<summary> 
    ///<para>Instaniates amount of GameObject with specified T component into pool</para>
    ///<para>Returns pool transform</para>
    ///</summary>
    public Transform CreatePool<T>(string name, int size) where T : Component
    {
        GameObject poolParent = new GameObject();
        poolParent.transform.SetParent(transform);
        poolParent.name = name + "Pool";

        for (int i = 0; i < size; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(poolParent.transform);
            go.name = name;
            go.AddComponent<T>();
            go.SetActive(false);
        }

        return poolParent.transform;
    }

    ///<summary>Instantiates GameObject as a child of pool</summary>
    private GameObject InstantiateChild(GameObject child, Transform pool)
    {
        GameObject instance = Instantiate(child, pool) as GameObject;
        instance.SetActive(false);

        return instance;
    }

    ///<summary>Gets first inactive GameObject from the pool</summary>
    public GameObject GetFromPool(Transform pool)
    {
        int childCount = pool.childCount;

        GameObject child = null;
        for (int i = 0; i < childCount; i++)
        {
            child = pool.GetChild(i).gameObject;
            if (!child.gameObject.activeInHierarchy)
            {
                return child;
            }
        }

        //Instantiates new one if there's no inactive children
        child = InstantiateChild(child, pool);
        Debug.Log(pool.name + " has insufficient number of children! Instantiating...");

        return child;
    }
}
