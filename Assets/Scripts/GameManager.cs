using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int gemCount;
    [SerializeField] public int level;
    [SerializeField] private Player player;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private GameObject landingZoneTiles;

    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material rampMaterial;
    [SerializeField] private int colorIndex;
    [SerializeField] private Color[] levelColors;

    [SerializeField] private GameObject[] normalPlatformObstaclesPrefabs;
    [SerializeField] private GameObject[] specialPlatformObstaclePrefabs;
    [SerializeField] private GameObject[] firstPlatformObstacles;
    [SerializeField] private GameObject[] secondPlatformObstacles;
    [SerializeField] private GameObject[] thirdPlatformObstacles;
    [SerializeField] private List<GameObject[]> obstaclePrefabsLists;
    [SerializeField] private ObstacleBasket[] obstacleBaskets;
    [SerializeField] private Vector3[] spawnPositions;
    [SerializeField] private GameObject[] obstacleContainers;
    [SerializeField] private GameObject flapActivators;
    [SerializeField] private Vector3[] activatorSpawnPoints;
    [SerializeField] private int levelObstacleIndex;

    [SerializeField] private int maxGemReward;
    [SerializeField] private int minGemReward;

    [SerializeField] private int uniqueLevels;
    [SerializeField] private int platformCount;

    private void Awake()
    {
        obstaclePrefabsLists = new List<GameObject[]>()
            { firstPlatformObstacles, secondPlatformObstacles, thirdPlatformObstacles };
        gemCount = PlayerPrefs.GetInt("GemCount", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        levelObstacleIndex = PlayerPrefs.GetInt("ObstacleIndex", 0);
        ActivatePlatformObstacles();
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
            int randomGemAward = Random.Range(minGemReward, maxGemReward);
            uiManager.ShowEndUI(randomGemAward, gemCount);
            gemCount += randomGemAward;
            player.GetComponent<RampMovement>().enabled = false;
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
        for (int i = 0; i < platformCount; i++)
        {
                GenerateObstacle(i, levelObstacleIndex);
        }
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

    public void IncreaseLevel()
    {
        level++;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("ObstacleIndex", levelObstacleIndex);
        uiManager.ShowNextLevel();
    }

    public void EnablePlayer()
    {
        player.GetComponent<Player>().enabled = true;
    }

    public void RetryLevel()
    {
        ResetBaskets();
        player.ResetPlayer();
        for (int i = 0; i < platformCount; i++)
        {
            if (obstacleContainers[i].transform.childCount == 0)
            {
                GenerateObstacle(i, levelObstacleIndex);
            }
        }
        uiManager.RestartLevel();
    }

    private void GenerateObstacle(int platformIndex, int obstacleIndex)
    {
        GameObject obstacle = null;
        if (obstacleIndex < uniqueLevels)
        {
            obstacle = obstaclePrefabsLists[platformIndex][obstacleIndex];
        }
        else
        {
            obstacle = obstaclePrefabsLists[platformIndex][Random.Range(0, uniqueLevels)];
        }
        int prefabObstacleAmount = int.Parse(obstacle.tag);
        obstacleBaskets[platformIndex].SetUpperBound(SetDifficulty(prefabObstacleAmount));
        Instantiate(obstacle, spawnPositions[platformIndex], Quaternion.identity, obstacleContainers[platformIndex].transform);
    }
}