using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HelicopterSpawner : MonoBehaviour
{
    // this is the script for the "helicopter" looking spawner that appears on the third platform
    [SerializeField] private GameObject fan;
    [SerializeField] private bool spawnInitialize;
    [SerializeField] private GameObject player;
    [SerializeField] private int shapeIndex;
    [SerializeField] private ObstacleBasket obstacleBasket;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        obstacleBasket = GameObject.Find("ObstacleBasketThird").GetComponent<ObstacleBasket>();
        fan.transform.DORotate(new Vector3(0, 180, 0), 1).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        shapeIndex = Random.Range(0, 4); //Gets the obstalce it will spawn randomly from pools
    }

    void Update()
    {
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        if (transform.position.z - player.transform.position.z < 35 && !spawnInitialize)
        {
            spawnInitialize = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        int movementChoice = Random.Range(0, 2); // Randomly choose movement
        if (movementChoice == 0)
        {
            LinearMovement();
        }
        else
        {
            SineMovement();
        }
        while (transform.position.z < 380)
        {
            GameObject obstacle = ObjectPool.SharedInstance.GetPooledObstacle(shapeIndex); 
            if (obstacle != null) {
                obstacle.transform.position = transform.position;
                obstacle.SetActive(true);
                obstacleBasket.pooledObjects.Add(obstacle);
            }
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(10);
        Deactivate();
    }

    // Tween sequence that makes heli to move like a sine graph
    private void SineMovement()
    {
        Sequence seqMovement = DOTween.Sequence();
        seqMovement.Append(transform.DOMoveX(-6, 0.25f).SetEase(Ease.InOutSine));
        seqMovement.Append(transform.DOMoveX(6, 0.5f).SetEase(Ease.InOutSine));
        seqMovement.Append(transform.DOMoveX(-6, 0.5f).SetEase(Ease.InOutSine));
        seqMovement.Append(transform.DOMoveX(0, 0.25f).SetEase(Ease.InOutSine));
        seqMovement.Insert(0, transform.DOMoveZ(380, seqMovement.Duration()).SetEase(Ease.Linear));
        seqMovement.Append(transform.DOMoveY(100, 2));
    }

    // Tween sequence that makes heli to move straight
    private void LinearMovement()
    {
        Sequence seqMovement = DOTween.Sequence();
        seqMovement.Append(transform.DOMoveX(-6, 0.2f).SetEase(Ease.Linear));
        seqMovement.Append(transform.DOMoveZ(20, 0.4f).SetEase(Ease.Linear).SetRelative());
        seqMovement.Append(transform.DOMoveX(6, 0.4f).SetEase(Ease.Linear));
        seqMovement.Append(transform.DOMoveZ(20, 0.4f).SetEase(Ease.Linear).SetRelative());
        seqMovement.Append(transform.DOMoveX(-6, 0.4f).SetEase(Ease.Linear));
        seqMovement.Append(transform.DOMoveZ(20, 0.4f).SetEase(Ease.Linear).SetRelative());
        seqMovement.Append(transform.DOMoveX(6, 0.4f).SetEase(Ease.Linear));
        seqMovement.Append(transform.DOMoveZ(10, 0.2f).SetEase(Ease.Linear).SetRelative());
        seqMovement.Append(transform.DOMoveY(100, 2));
    }

    private void Deactivate()
    {
        DOTween.Kill(fan.transform);
        DOTween.Kill(transform);
        Destroy(gameObject, 2);
    }
}
