using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledSpheres;
    public GameObject spherePrefabs;
    public int amount;
    public GameObject objectPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledSpheres = new List<GameObject>();
        GameObject tmp;
        for(int i = 0; i < amount; i++)
        {
            tmp = Instantiate(spherePrefabs, objectPool.transform);
            tmp.SetActive(false);
            pooledSpheres.Add(tmp);
        }
    }
    
    public GameObject GetPooledObject(int index)
    {
        List<GameObject> selectedList = null;
        switch (index)
        {
            case 0:
                selectedList = pooledSpheres;
                break;
        }
        for(int i = 0; i < amount; i++)
        {
            if(!selectedList[i].activeInHierarchy)
            {
                return selectedList[i];
            }
        }
        return null;
    }

    public void DeactivatePooledObjects(int index)
    {
        List<GameObject> selectedList = null;
        switch (index)
        {
            case 0:
                selectedList = pooledSpheres;
                break;
        }
        for(int i = 0; i < amount; i++)
        {
            if(selectedList[i].activeInHierarchy)
            {
                selectedList[i].SetActive(false);
            }
        }
    }
}