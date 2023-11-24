using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gemCount;
    [SerializeField] private int level;
    [SerializeField] private GameObject gemTiles;
    [SerializeField] private Player player;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material rampMaterial;
    [SerializeField] private int colorIndex;
    [SerializeField] private Color[] levelColors;
    [SerializeField] private GameObject[] normalPlatformObstaclesPrefabs;
    [SerializeField] private GameObject[] specialPlatformObstaclePrefabs;
    [SerializeField] private ObstacleBasket[] obstacleBaskets;
    [SerializeField] private Vector3[] spawnPositions;
    [SerializeField] private GameObject[] obstacleContainers;
    [SerializeField] private GameObject flapActivators;
    [SerializeField] private Vector3[] activatorSpawnPoints;
    public bool levelStarted;
    // Start is called before the first frame update
    void Start()
    {
        gemCount = PlayerPrefs.GetInt("GemCount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextLevel()
    {
        ChangeLevelColors();
        ActivatePlatformObstacles();
        LoadFlapActivators();
        ResetBaskets();
    }

    private void ChangeLevelColors()
    {
        groundMaterial.color = rampMaterial.color;
        rampMaterial.color = levelColors[colorIndex + 1];
    }

    public IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < gemTiles.transform.childCount; i++)
        {
            gemTiles.transform.GetChild(i).DOScale(0, 1);
        }
        yield return new WaitForSeconds(1.5f);
        player.transform.DOMove(new Vector3(0, 0.6f, 0), 2).OnComplete(() =>
        {
            player.GetComponent<RampMovement>().enabled = false;
            player.GetComponent<Player>().enabled = true;
        });
    }

    public void IncreaseGemCount(int amount)
    {
        gemCount += amount;
        PlayerPrefs.SetInt("GemCount", gemCount);
    }

    private void ActivatePlatformObstacles()
    {
        // This for loop creates new obstacles for the first two platforms
        // Randomly selects an obstacle group from normal obstacles array
        // The prefab's name is how many obstacles in that group
        // Then sets that platform's corresponding basket's upper bound according to difficulty
        for (int i = 0; i < 2; i++)
        {
            int prefabLength = normalPlatformObstaclesPrefabs.Length;
            GameObject randomPrefab = normalPlatformObstaclesPrefabs[Random.Range(0, prefabLength)];
            int prefabObstacleCount = int.Parse(randomPrefab.name);
            obstacleBaskets[i].SetUpperBound(SetDifficulty(prefabObstacleCount));
            Instantiate(randomPrefab, spawnPositions[i], Quaternion.identity, obstacleContainers[i].transform);
        }
        // This section is for third platform only, which has special obstacles
        // No upper bound is set for this section, it is flat.
        int specialPrefabLength = specialPlatformObstaclePrefabs.Length;
        GameObject randomSpecialPrefab = specialPlatformObstaclePrefabs[Random.Range(0, specialPrefabLength)];
        Instantiate(randomSpecialPrefab, spawnPositions[2], Quaternion.identity, obstacleContainers[2].transform);
    }

    private void ResetBaskets()
    {
        foreach (var basket in obstacleBaskets)
        {
            basket.ResetBasket();
        }
    }

    private void LoadFlapActivators()
    {
        for (int i = 0; i < flapActivators.transform.childCount; i++)
        {
            GameObject currentActivator = flapActivators.transform.GetChild(i).gameObject;
            currentActivator.transform.position =
                activatorSpawnPoints[Random.Range(0, activatorSpawnPoints.Length)];
            currentActivator.SetActive(true);
        }
    }

    public int SetDifficulty(int obstacleCount)
    {
        // This is a very random difficulty modifier I came up with
        // After a certain Level
        int difficultyLevel = 5;
        if (level <= difficultyLevel)
        {
            return obstacleCount;
        }
        int newCount = obstacleCount - (level - difficultyLevel);
        return newCount;
    }
}
