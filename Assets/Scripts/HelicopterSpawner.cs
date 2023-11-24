using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelicopterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fan;
    [SerializeField] private bool spawnInitialize;
    [SerializeField] private GameObject player;
    [SerializeField] private int listIndex;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        fan.transform.DORotate(new Vector3(0, 180, 0), 1).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        listIndex = Random.Range(0, 4);
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
        int movementChoice = Random.Range(0, 2);
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
            GameObject obstacle = ObjectPool.SharedInstance.GetPooledObject(listIndex); 
            if (obstacle != null) {
                obstacle.transform.position = transform.position;
                obstacle.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(10);
        Deactivate();
    }

    private void SineMovement()
    {
        Sequence seqMovement = DOTween.Sequence();
        seqMovement.Append(transform.DOMoveX(-6, 0.25f).SetEase(Ease.InOutSine));
        seqMovement.Append(transform.DOMoveX(6, 0.5f).SetEase(Ease.InOutSine));
        seqMovement.Append(transform.DOMoveX(-6, 0.5f).SetEase(Ease.InOutSine));
        seqMovement.Append(transform.DOMoveX(0, 0.25f).SetEase(Ease.InOutSine));
        seqMovement.Insert(0, transform.DOMoveZ(380, seqMovement.Duration()).SetEase(Ease.Linear));
        seqMovement.Append(transform.DOMoveY(100, 0.5f));
    }

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
        seqMovement.Append(transform.DOMoveY(100, 0.5f));
    }

    private void Deactivate()
    {
        DOTween.Kill(fan.transform);
        DOTween.Kill(transform);
        Destroy(gameObject, 2);
    }
}
