using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PropBasket : MonoBehaviour
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
    [SerializeField] private List<GameObject> contacts;

    void Update()
    {
        CountTimer();
        countText.text = currentCount + " / " + targetCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer >= 0)
        {
            if (!firstContact)
            {
                sphereTag = other.tag;
                firstContact = true;
            }

            contacts.Add(other.gameObject);
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
        if (sphereTag == "SmallObstacle")
        {
            GameObject temp = contacts[0];
            GameObject sphereGroupContainer = temp.transform.parent.parent.gameObject;
            for (int i = 0; i < contacts.Capacity; i++)
            {
                // Initialize particle
            }

            Destroy(sphereGroupContainer);
        }
        else
        {
            for (int i = 0; i < contacts.Capacity; i++)
            {
                if (contacts[i].gameObject.activeSelf)
                {
                    // InitializE Particle
                    contacts[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void RaiseGatesAndPlatform()
    {
        leftGate.transform.DORotate(new Vector3(0, 0, -75), 3);
        rightGate.transform.DORotate(new Vector3(0, 0, 75), 3);
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
        platform.transform.DOMoveY(-5, 1);
        leftGate.transform.DORotate(Vector3.zero, 1);
        rightGate.transform.DORotate(Vector3.zero, 1);
    }
}