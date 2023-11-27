using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // This is the object pool script that is used for smaller objects that are used on the third platform, 
    // gem images that pop up for UI and particles that "jump" after obstacles are destroyed in obstacle basket
    
    // For platform one and two, I decided to not use object pooling, because of them having so many obstacles 
    // that you need to track and reset the state of, I thought that it was not worth it
    
    public static ObjectPool SharedInstance;
    private List<List<GameObject>> pooledObstacleLists;
    
    public List<GameObject> pooledSpheres;
    public List<GameObject> pooledCubes;
    public List<GameObject> pooledCapsules;
    public List<GameObject> pooledPyramids;
    public List<GameObject> pooledGemImages;
    public List<GameObject> pooledParticles;
    
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public GameObject capsulePrefab;
    public GameObject pyramidPrefab;
    public GameObject gemImagePrefab;
    public GameObject particlePrefab;
    
    public GameObject spherePoolContainer;
    public GameObject cubePoolContainer;
    public GameObject capsulePoolContainer;
    public GameObject pyramidPoolContainer;
    public GameObject gemImageContainer;
    public GameObject particleContainer;
    
    public int obstacleAmount;
    public int gemImageAmount;
    public int particleAmount;

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
        pooledGemImages = new List<GameObject>();
        pooledParticles = new List<GameObject>();
        pooledObstacleLists = new List<List<GameObject>>()
        {
            pooledSpheres, pooledCubes, pooledCapsules, pooledPyramids,
        };
        
        for (int i = 0; i < obstacleAmount; i++)
        {
            FillPool(spherePrefab, pooledSpheres, spherePoolContainer);
            FillPool(cubePrefab, pooledCubes, cubePoolContainer);
            FillPool(capsulePrefab, pooledCapsules, capsulePoolContainer);
            FillPool(pyramidPrefab, pooledPyramids, pyramidPoolContainer);
        }
        
        for (int i = 0; i < gemImageAmount; i++)
        {
            FillPool(gemImagePrefab, pooledGemImages, gemImageContainer);
        }

        for (int i = 0; i < particleAmount; i++)
        {
            FillPool(particlePrefab, pooledParticles, particleContainer);
        }
    }

    private void FillPool(GameObject prefab, List<GameObject> pool, GameObject container)
    {
        GameObject tmp = Instantiate(prefab, container.transform);
        tmp.SetActive(false);
        pool.Add(tmp);
    }

    public GameObject GetPooledObstacle(int index)
    {
        for (int i = 0; i < obstacleAmount; i++)
        {
            if (!pooledObstacleLists[index][i].activeInHierarchy)
            {
                return pooledObstacleLists[index][i];
            }
        }
        return null;
    }
    
    public GameObject GetPooledImage()
    {
        for (int i = 0; i < gemImageAmount; i++)
        {
            if (!pooledGemImages[i].activeInHierarchy)
            {
                return pooledGemImages[i];
            }
        }
        return null;
    }
    
    public GameObject GetPooledParticle()
    {
        for (int i = 0; i < particleAmount; i++)
        {
            if (!pooledParticles[i].activeInHierarchy)
            {
                return pooledParticles[i];
            }
        }
        return null;
    }
}