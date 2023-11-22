using DG.Tweening;
using TMPro;
using UnityEngine;

public class PropBasket : MonoBehaviour
{
    [SerializeField] private int targetCount;
    [SerializeField] private int currentCount;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private float timer;
    [SerializeField] private bool firstContact;
    [SerializeField] private Player player;
    [SerializeField] private GameObject leftGate;
    [SerializeField] private GameObject rightGate;
    [SerializeField] private GameObject platform;
    
    void Update()
    {
        CountTimer();
        countText.text = currentCount + " / " + targetCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer >= 0 && other.CompareTag("SmallObstacle"))
        {
            if (!firstContact)
            {
                firstContact = true;
            }
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
        else
        {
            if (currentCount >= targetCount)
            {
                //Pass
                RaiseGatesAndPlatform();
                firstContact = false;
            }
            else
            {
                //Fail
            }
        }
    }

    private void RaiseGatesAndPlatform()
    {
        leftGate.transform.DORotate(new Vector3(0, 0, -90), 3);
        rightGate.transform.DORotate(new Vector3(0, 0, 90), 3);
        platform.transform.DOMoveY(0, 1).OnComplete(() =>
        {
            player.ContinueMoving();
            currentCount = 0;
        });
    }
}
