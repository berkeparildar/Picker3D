using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private void FixedUpdate()
    {
        Vector3 movementVector = new Vector3(Input.GetAxis("Horizontal"), 0, verticalSpeed) * speed;
        rb.MovePosition(rb.position + movementVector * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndWall"))
        {
            verticalSpeed = 0;
        }
    }
    
    // This method is called by the obstacle basket if player manages to 
    // carry the desired number of small obstacles to the basket
    public void ContinueMoving()
    {
        verticalSpeed = 1;
    }
}
