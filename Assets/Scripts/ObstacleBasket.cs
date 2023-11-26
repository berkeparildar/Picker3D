using DG.Tweening;
using TMPro;
using UnityEngine;

public class ObstacleBasket : MonoBehaviour
{
    [SerializeField] private int targetCount;
    [SerializeField] private int currentCount;
    [SerializeField] private float timer;
    [SerializeField] private bool firstContact;
    [SerializeField] private bool finished;
    [SerializeField] private UIManager uiManager;

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
        // count triggers only if time is not finished
        if (timer >= 0 && (other.CompareTag("SmallObstacle") || other.CompareTag("PooledObstacle")))
        {
            if (!firstContact)  // start the timer if this is the first contact
            {
                firstContact = true;
                // I have this temp game object here to later check whether this basket is for the first and second
                // platforms or the third
                // Removing the obstacles after evaluating differs for two, so I check this temp object's tag to differ
                temp = other.gameObject;
            }
            currentCount++;
            timer = 1.0f; // time resets after each contact
        }
    }
    // This method's purpose is to limit the amount of time where player can push an obstacle in
    private void CountTimer()
    {
        // After the first contact the timer starts
        if (firstContact && timer >= 0)
        {
            timer -= Time.deltaTime;
        }
        //Once time finishes, it evaluates according to counts
        if (timer <= 0 && !finished)
        {
            finished = true;
            // success
            if (currentCount >= targetCount)
            {
                RaiseGatesAndPlatform();
                uiManager.FillProgressImage();
            }
            // fail
            else
            {
                uiManager.ShowFailUI();
            }
            RemoveObstacles();
        }
    }

    // Called after time is finished
    private void RemoveObstacles()
    {
        // For the first and second platform obstacles, destroys their parent since not pooled
        if (temp.CompareTag("SmallObstacle"))
        {
            GameObject sphereGroupContainer = temp.transform.parent.parent.gameObject;
            Destroy(sphereGroupContainer);
        }
        // for the third platform, deactivates them from the pool
        else
        {
            ObjectPool.SharedInstance.DeactivatePooledObjects();
        }
    }

    // Sets the visuals after hitting the target count
    // Lets player go
    private void RaiseGatesAndPlatform()
    {
        leftGate.transform.DORotate(new Vector3(0, 0, -60), 2);
        rightGate.transform.DORotate(new Vector3(0, 0, 60), 2);
        platform.transform.DOMoveY(0, 1).OnComplete(() =>
        {
            player.ContinueMoving();
        });
    }
    
    // Resets the current state of the basket to default
    // Called when level is being reset, after the player launches of the ramp
    public void ResetBasket()
    {
        firstContact = false;
        finished = false;
        currentCount = 0;
        timer = 1.0f;
        platform.transform.DOMoveY(-10, 1);
        leftGate.transform.DORotate(Vector3.zero, 1);
        rightGate.transform.DORotate(Vector3.zero, 1);
    }
    
    // This is called by Game Manager setting the difficulty
    public void SetUpperBound(int target)
    {
        targetCount = target;
    }
}