using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private bool isOnRamp;
    [SerializeField] private bool launched;
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
        if (!isOnRamp)
        {
            Movement();
        }
        else
        {
            RampMovement();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "EndWall":
                verticalSpeed = 0;
                break;
            case "RampStart":
                RampTransition();
                break;
            case "RampEnd":
                speed = 0;
                launched = true;
                rb.AddForce(transform.forward * 100, ForceMode.Impulse);
                break;
        }
    }

    // This is the default method used for moving tool.
    private void Movement()
    {
        Vector3 movementVector = new Vector3(Input.GetAxis("Horizontal"), 0, verticalSpeed) * speed;
        rb.MovePosition(rb.position + movementVector * Time.fixedDeltaTime);
    }

    private void RampMovement()
    {
        if (!launched)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    // This method changes the rigidbody and collider settings
    // to move accordingly on the ramp
    private void RampTransition()
    {
        meshCollider.enabled = !meshCollider.enabled;
        rb.isKinematic = !rb.isKinematic;
        boxCollider.enabled = !boxCollider.enabled;
        isOnRamp = !isOnRamp;
    }
    
    // This method is called by the obstacle basket if player manages to 
    // carry the desired number of small obstacles to the basket
    public void ContinueMoving()
    {
        verticalSpeed = 1;
    }
}
