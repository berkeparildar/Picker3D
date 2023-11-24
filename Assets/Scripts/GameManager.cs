using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gemCount;
    [SerializeField] private GameObject gemTiles;
    [SerializeField] private Player player;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material rampMaterial;
    [SerializeField] private int colorIndex;
    [SerializeField] private Color[] levelColors;
    [SerializeField] private GameObject[] platformContainers;
    [SerializeField] private PropBasket[] propBaskets;
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
        player.transform.DOMove(new Vector3(0, 0.6f, 0), 2);
    }

    public void IncreaseGemCount(int amount)
    {
        gemCount += amount;
        PlayerPrefs.SetInt("GemCount", gemCount);
    }

    private void ActivatePlatformObstacles()
    {
        foreach (var container in platformContainers)
        {
            int containerChildCount = container.transform.childCount;
            int randomIndex = Random.Range(0, containerChildCount);
            container.transform.GetChild(randomIndex).gameObject.SetActive(true);
        }
    }

    private void ResetBaskets()
    {
        foreach (var basket in propBaskets)
        {
            basket.ResetBasket();
        }
    }
}
