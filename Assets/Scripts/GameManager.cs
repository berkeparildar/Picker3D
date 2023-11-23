using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gemCount;
    [SerializeField] private GameObject gemTiles;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {
        gemCount = PlayerPrefs.GetInt("GemCount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
