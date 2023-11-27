using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int platformCount;
    [SerializeField] public int level;
    [SerializeField] private int uniqueLevels;
    [SerializeField] private int obstacleIndex;
    [SerializeField] private Player player;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material rampMaterial;
    [SerializeField] private Color[] levelColors;

    [SerializeField] private GameObject[] firstPlatformObstacles;
    [SerializeField] private GameObject[] secondPlatformObstacles;
    [SerializeField] private GameObject[] thirdPlatformObstacles;
    [SerializeField] private List<GameObject[]> obstaclePrefabsLists;
    [SerializeField] private ObstacleBasket[] obstacleBaskets;
    [SerializeField] private Vector3[] spawnPositions;
    [SerializeField] private GameObject[] obstacleContainers;
    [SerializeField] private GameObject flapActivators;
    [SerializeField] private Vector3[] activatorSpawnPoints;

    [SerializeField] public int gemCount;
    [SerializeField] private int maxGemReward;
    [SerializeField] private int minGemReward;


    private void Awake()
    {
        obstaclePrefabsLists = new List<GameObject[]>() { firstPlatformObstacles, secondPlatformObstacles, thirdPlatformObstacles };
        gemCount = PlayerPrefs.GetInt("GemCount", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        obstacleIndex = PlayerPrefs.GetInt("ObstacleIndex", 0);
        GenerateNewLevel();
    }

    public void LoadNextLevel()
    {
        //ChangeLevelColors();
        GenerateNewLevel();
        LoadFlapActivators();
        ResetBaskets();
    }

    private void ChangeLevelColors()
    {
        groundMaterial.color = rampMaterial.color;
        rampMaterial.color = levelColors[level + 1];
    }

    private void MovePlayerToStartPosition()
    {
        player.transform.DOMove(new Vector3(0, 0.6f, 0), 2).OnComplete(GetEndLevelGemReward);
    }

    public void ReadyPlayer()
    {
        player.enabled = true;
        uiManager.ShowStartUI();
    }

    private void GetEndLevelGemReward()
    {
        int randomGemAward = Random.Range(minGemReward, maxGemReward);
        uiManager.ShowEndUI(randomGemAward, gemCount);
        gemCount += randomGemAward;
    }

    public IEnumerator GetLandingZoneGemReward(int landedRewardTile)
    {
        IncreaseGemCount(landedRewardTile);
        StartCoroutine(uiManager.ShowScorePopUp(landedRewardTile));
        yield return StartCoroutine(uiManager.ShowGemPopUp());
        MovePlayerToStartPosition();
    }

    public void IncreaseGemCount(int amount)
    {
        gemCount += amount;
        PlayerPrefs.SetInt("GemCount", gemCount);
    }

    private void GenerateNewLevel()
    {
        for (int i = 0; i < platformCount; i++)
        {
            GenerateObstacle(i);
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
        obstacleIndex++;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("ObstacleIndex", obstacleIndex);
        uiManager.UpdateLevelIndicators();
    }

    public void RetryLevel()
    {
        ResetBaskets();
        player.ResetPlayer();
        GenerateMissingObstacle();
        uiManager.RestartLevel();
    }

    private void GenerateObstacle(int platformIndex)
    {
        GameObject obstacle = null;
        if (level <= uniqueLevels)
        {
            obstacle = obstaclePrefabsLists[platformIndex][obstacleIndex];
        }
        else
        {
            obstacleIndex = Random.Range(0, uniqueLevels);
            obstacle = obstaclePrefabsLists[platformIndex][obstacleIndex];
            PlayerPrefs.SetInt("ObstacleIndex", obstacleIndex);
        }

        int prefabObstacleAmount = int.Parse(obstacle.tag);
        obstacleBaskets[platformIndex].SetUpperBound(SetDifficulty(prefabObstacleAmount));
        Instantiate(obstacle, spawnPositions[platformIndex], Quaternion.identity,
            obstacleContainers[platformIndex].transform);
    }

    private void GenerateMissingObstacle()
    {
        for (int i = 0; i < platformCount; i++)
        {
            if (obstacleContainers[i].transform.childCount != 0)
            {
                Destroy(obstacleContainers[i].transform.GetChild(0).gameObject);
            }
            GenerateObstacle(i);
        }
    }
}