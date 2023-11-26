using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledSpheres;
    public List<GameObject> pooledCubes;
    public List<GameObject> pooledCapsules;
    public List<GameObject> pooledPyramids;
    public List<GameObject> gemImages;
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public GameObject capsulePrefab;
    public GameObject pyramidPrefab;
    public GameObject spherePoolContainer;
    public GameObject cubePoolContainer;
    public GameObject capsulePoolContainer;
    public GameObject pyramidPoolContainer;
    public int amount;
    public GameObject gemImagePrefab;
    public int gemImageCount;
    public GameObject gemImageContainer;

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
        gemImages = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            FillPool(spherePrefab, pooledSpheres, spherePoolContainer);
            FillPool(cubePrefab, pooledCubes, cubePoolContainer);
            FillPool(capsulePrefab, pooledCapsules, capsulePoolContainer);
            FillPool(pyramidPrefab, pooledPyramids, pyramidPoolContainer);
        }

        for (int i = 0; i < gemImageCount; i++)
        {
            FillPool(gemImagePrefab, gemImages, gemImageContainer);
        }
    }

    public void FillPool(GameObject prefab, List<GameObject> pool, GameObject container)
    {
        GameObject tmp = Instantiate(prefab, container.transform);
        tmp.SetActive(false);
        pool.Add(tmp);
    }

    public GameObject GetPooledObject(int index)
    {
        List<GameObject> selectedList = null;
        int currentAmount = 0;
        switch (index)
        {
            case 0:
                selectedList = pooledSpheres;
                currentAmount = amount;
                break;
            case 1:
                selectedList = pooledCubes;
                currentAmount = amount;
                break;
            case 2:
                selectedList = pooledCapsules;
                currentAmount = amount;
                break;
            case 3:
                selectedList = pooledPyramids;
                currentAmount = amount;
                break;
            case 4:
                selectedList = gemImages;
                currentAmount = gemImageCount;
                break;
        }

        for (int i = 0; i < currentAmount; i++)
        {
            if (!selectedList[i].activeInHierarchy)
            {
                return selectedList[i];
            }
        }
        return null;
    }

    // This needs refactoring later
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