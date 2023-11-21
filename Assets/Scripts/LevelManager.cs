using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshCollider meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            transform.position = new Vector3(0, 0.7f, 0);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            boxCollider.enabled = true;
            meshCollider.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward * 10, ForceMode.Impulse);
        }
    }

    /*private void FixedUpdate()
    {
        Vector3 movement = Vector3.forward * speed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }*/

    private void ScrollLevel()
    {
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.left * (horizontalInput * speed * 2 * Time.deltaTime));
    }
}
