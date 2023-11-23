using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    // Rigidbody, MeshCollider and BoxCollider components of the player
    // The reason for two colliders is that on the ramp, the player's rigidbody
    // is no longer kinematic, and mesh colliders on non-kinematic body's are not supported
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshCollider meshCollider;
    
    [SerializeField] private GameObject defaultCamera;
    [SerializeField] private GameObject rampCamera;
    [SerializeField] private GameObject fallCameraPoint;
    
    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;
    
    [SerializeField] private bool isOnRamp;
    [SerializeField] private bool launched;
    [SerializeField] private bool hasLanded;
    
    [SerializeField] private GameManager gameManager;
    
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
                if (!isOnRamp)
                {
                    RampTransition();
                }
                break;
            case "RampEnd":
                Launch();
                break;
            case "LandingTiles":
                Land(other.gameObject.name);
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
            Vector3 movement = transform.forward * speed;
            movement.y = rb.velocity.y;
            rb.velocity = movement;
        }
    }

    // This method changes the rigidbody and collider settings
    // to move accordingly on the ramp
    private void RampTransition()       
    {
        rampCamera.SetActive(true); // Activate the Ramp's virtual camera
        meshCollider.enabled = false; // Deactivate MeshCollider
        rb.isKinematic = !rb.isKinematic; // Rigidbody is set to dynamic
        boxCollider.enabled = true; // BoxCollider is enabled
        isOnRamp = !isOnRamp; // Ramp flag is set
    }

    
    // Called at the end of the ramp for launching the player object
    private void Launch()
    {
        launched = true; // This boolean's purpose is to deactivate any other movement that player object has
        //rb.velocity = Vector3.zero;
        //rb.AddForce(transform.forward / 10, ForceMode.Impulse);
        transform.position = new Vector3(0, 72, -250);
        rampCamera.SetActive(false);
        defaultCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = fallCameraPoint.transform;
    }
    
    private void Land(string gameObjectName)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            int landedAmount = int.Parse(gameObjectName);
            gameManager.IncreaseGemCount(landedAmount);
            StartCoroutine(gameManager.ResetLevel());
        }
    }
    
    // This method is called by the obstacle basket if player manages to 
    // carry the desired number of small obstacles to the basket
    public void ContinueMoving()
    {
        verticalSpeed = 1;
    }

    public void ResetFlags()
    {
        
    }
}
