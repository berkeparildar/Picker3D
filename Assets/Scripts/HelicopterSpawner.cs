using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HelicopterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fan;
    [SerializeField] private bool spawnInitialize;
    [SerializeField] private GameObject player;
    [SerializeField] private int spawnCount;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        fan.transform.DORotate(new Vector3(0, 180, 0), 1).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    // Update is called once per frame
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
            GameObject sphere = ObjectPool.SharedInstance.GetPooledObject(0); 
            if (sphere != null) {
                sphere.transform.position = transform.position;
                sphere.SetActive(true);
            }
            spawnCount++;
            yield return new WaitForSeconds(0.05f);
        }
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
        seqMovement.OnComplete(() =>
        {
            DOTween.Kill(fan.transform);
            DOTween.Kill(transform);
            Destroy(gameObject, 2);
        });
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
        seqMovement.Append(transform.DOMoveY(100, 0.4f));
        seqMovement.OnComplete(() =>
        {
            DOTween.Kill(fan.transform);
            DOTween.Kill(transform);
            Destroy(gameObject, 2);
        });
    }
}
