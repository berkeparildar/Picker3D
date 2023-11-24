using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExplodingObstacles : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private int shapeIndex;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private int amount;
    [SerializeField] private bool isDrop;
    [SerializeField] private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (CompareTag("Drop"))
        {
            shapeIndex = Random.Range(0, 4);
            isDrop = true;
        }
        else
        {
            shapeIndex = 0;
        }
        GetComponent<MeshFilter>().mesh = meshes[shapeIndex];
        amount = Random.Range(5, 9);
    }

    private void Update()
    {
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        if (transform.position.z - player.transform.position.z < 50)
        {
            if (isDrop)
            {
                rb.useGravity = true;
            }
            else
            {
                transform.Translate(Vector3.back * (10 * Time.deltaTime));
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isDrop)
        {
            Explode();
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Explode();
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obstacle = ObjectPool.SharedInstance.GetPooledObject(shapeIndex);
            if (obstacle != null)
            {
                Vector3 currentPosition = transform.position;
                obstacle.transform.position = new Vector3(currentPosition.x + Random.Range(-1f, 1f), 
                    currentPosition.y, currentPosition.z + Random.Range(-1f, 1f));
                obstacle.SetActive(true);
                /*Rigidbody rb = obstacle.GetComponent<Rigidbody>();
                rb.AddForce(new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)),
                    ForceMode.Impulse);*/
            }
        }
    }
}
