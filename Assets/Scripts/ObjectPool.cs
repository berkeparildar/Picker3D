using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledSpheres;
    public List<GameObject> pooledCubes;
    public List<GameObject> pooledCapsules;
    public List<GameObject> pooledPyramids;
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public GameObject capsulePrefab;
    public GameObject pyramidPrefab;
    public int amount;
    public GameObject objectPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledSpheres = new List<GameObject>();
        pooledCapsules = new List<GameObject>();
        pooledCubes = new List<GameObject>();
        pooledPyramids = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            FillPool(spherePrefab, pooledSpheres);
            FillPool(cubePrefab, pooledCubes);
            FillPool(capsulePrefab, pooledCapsules);
            FillPool(pyramidPrefab, pooledPyramids);
        }
    }

    public void FillPool(GameObject prefab, List<GameObject> pool)
    {
        GameObject tmp = Instantiate(prefab, objectPool.transform);
        tmp.SetActive(false);
        pool.Add(tmp);
    }

    public GameObject GetPooledObject(int index)
    {
        List<GameObject> selectedList = null;
        switch (index)
        {
            case 0:
                selectedList = pooledSpheres;
                break;
            case 1:
                selectedList = pooledCubes;
                break;
            case 2:
                selectedList = pooledCapsules;
                break;
            case 3:
                selectedList = pooledPyramids;
                break;
        }

        for (int i = 0; i < amount; i++)
        {
            if (!selectedList[i].activeInHierarchy)
            {
                return selectedList[i];
            }
        }
        return null;
    }

    public void DeactivatePooledObjects()
    {
        for (int i = 0; i < amount; i++)
        {
            if (pooledSpheres[i].activeInHierarchy)
            {
                pooledSpheres[i].SetActive(false);
            }
            if (pooledCubes[i].activeInHierarchy)
            {
                pooledCubes[i].SetActive(false);
            }
            if (pooledCapsules[i].activeInHierarchy)
            {
                pooledCapsules[i].SetActive(false);
            }
            if (pooledPyramids[i].activeInHierarchy)
            {
                pooledPyramids[i].SetActive(false);
            }
        }
    }
}