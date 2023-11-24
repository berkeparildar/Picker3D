using DG.Tweening;
using TMPro;
using UnityEngine;

public class ObstacleBasket : MonoBehaviour
{
    [SerializeField] private int targetCount;
    [SerializeField] private int currentCount;
    [SerializeField] private string sphereTag;
    [SerializeField] private float timer;
    [SerializeField] private bool firstContact;
    [SerializeField] private bool finished;

    [SerializeField] private TextMeshPro countText;
    [SerializeField] private Player player;

    [SerializeField] private GameObject leftGate;
    [SerializeField] private GameObject rightGate;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject temp;

    void Update()
    {
        CountTimer();
        countText.text = currentCount + " / " + targetCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer >= 0 && (other.CompareTag("SmallObstacle")||other.CompareTag("PooledObstacle")))
        {
            Debug.Log("Running from:  " +  gameObject.name);
            if (!firstContact)
            {
                firstContact = true;
            }
            temp = other.gameObject;
            currentCount++;
            timer = 1.0f;
        }
    }

    private void CountTimer()
    {
        if (firstContact && timer >= 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0 && !finished)
        {
            finished = true;
            if (currentCount >= targetCount)
            {
                //Pass
                RaiseGatesAndPlatform();
                RemoveObstacles();
            }
            else
            {
                //Fail
            }
        }
        
    }

    private void RemoveObstacles()
    {
        if (temp.CompareTag("SmallObstacle"))
        {
            GameObject sphereGroupContainer = temp.transform.parent.parent.gameObject;
            Destroy(sphereGroupContainer);
        }
        else
        {
            ObjectPool.SharedInstance.DeactivatePooledObjects();
        }
    }

    private void RaiseGatesAndPlatform()
    {
        leftGate.transform.DORotate(new Vector3(0, 0, -60), 2);
        rightGate.transform.DORotate(new Vector3(0, 0, 60), 2);
        platform.transform.DOMoveY(0, 1).OnComplete(() =>
        {
            player.ContinueMoving();
            currentCount = 0;
        });
    }

    public void ResetBasket()
    {
        firstContact = false;
        currentCount = 0;
        timer = 1.0f;
        platform.transform.DOMoveY(-10, 1);
        leftGate.transform.DORotate(Vector3.zero, 1);
        rightGate.transform.DORotate(Vector3.zero, 1);
    }

    public void SetUpperBound(int target)
    {
        targetCount = target;
    }
}