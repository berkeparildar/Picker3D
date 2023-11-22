using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDemo : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        rb.velocity = transform.forward * speed;
    }
}
