using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gemCount;
    [SerializeField] private int level;
    [SerializeField] private GameObject landingZoneTiles;
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
    [SerializeField] private int levelObstacleIndex;
    public bool levelStarted;
    // Start is called before the first frame update
    void Start()
    {
        gemCount = PlayerPrefs.GetInt("GemCount", 0);
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
        for (int i = 0; i < landingZoneTiles.transform.childCount; i++)
        {
            landingZoneTiles.transform.GetChild(i).DOScale(0, 1);
        }
        yield return new WaitForSeconds(1.5f);
        player.transform.DOMove(new Vector3(0, 0.6f, 0), 2).OnComplete(() =>
        {
            player.GetComponent<RampMovement>().enabled = false;
            player.GetComponent<Player>().enabled = true;
            for (int i = 0; i < landingZoneTiles.transform.childCount; i++)
            {
                landingZoneTiles.transform.GetChild(i).DOScale(new Vector3(23, 0.1f, 9.9f), 1);
            }
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
        if (levelObstacleIndex <= normalPlatformObstaclesPrefabs.Length - 2)
        {
            GameObject firstObstacle = normalPlatformObstaclesPrefabs[levelObstacleIndex];
            int prefabFirstCount = int.Parse(firstObstacle.tag);
            levelObstacleIndex++;
            obstacleBaskets[0].SetUpperBound(SetDifficulty(prefabFirstCount));
            Instantiate(firstObstacle, spawnPositions[0], Quaternion.identity, obstacleContainers[0].transform);
            GameObject secondObstacle = normalPlatformObstaclesPrefabs[levelObstacleIndex];
            int prefabSecondCount = int.Parse(secondObstacle.tag);
            obstacleBaskets[1].SetUpperBound(SetDifficulty(prefabSecondCount));
            levelObstacleIndex++;
            Instantiate(secondObstacle, spawnPositions[1], Quaternion.identity, obstacleContainers[1].transform);
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                int prefabLength = normalPlatformObstaclesPrefabs.Length;
                GameObject randomPrefab = normalPlatformObstaclesPrefabs[Random.Range(0, prefabLength)];
                int prefabObstacleCount = int.Parse(randomPrefab.tag);
                obstacleBaskets[i].SetUpperBound(SetDifficulty(prefabObstacleCount));
                Instantiate(randomPrefab, spawnPositions[i], Quaternion.identity, obstacleContainers[i].transform);
            }
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

    private int SetDifficulty(int obstacleCount)
    {
        // This is a very random difficulty modifier I came up with
        if (level < 5)
        {
            if (obstacleCount == 20)
            {
                return 10;
            }
            return 20;
        }
        else
        {
            int difficultyVariable = level - 5;
            if (difficultyVariable > 10)
            {
                difficultyVariable = 10;
            }
            if (obstacleCount == 20)
            {
                return (10 + Random.Range(0, difficultyVariable));
            }
            return (20 + Random.Range(0, difficultyVariable));
        }
    }
}