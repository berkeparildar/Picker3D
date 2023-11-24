using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    // Rigidbody, MeshCollider and BoxCollider components of the player
    // The reason for two colliders is that on the ramp, the player's rigidbody
    // is no longer kinematic, and mesh colliders on non-kinematic body's are not supported
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshCollider meshCollider;

    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;

    [SerializeField] private bool gameStart;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private bool isTouching;
    [SerializeField] private bool firstTouch;
    [SerializeField] private Vector2 initialTouchPosition; 
    [SerializeField] private float xDelta;
    
    private void FixedUpdate()
    {
        Movement();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "EndWall":
                verticalSpeed = 0;
                break;
            case "RampStart":
                GetComponent<RampMovement>().enabled = true;
                GetComponent<Player>().enabled = false;
                break;
        }
    }

    private void Update()
    {
        HorizontalMovement();
    }

    private void HorizontalMovement()
    {
        xDelta = 0;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (!firstTouch)
                {
                    firstTouch = true;
                    uiManager.LevelStart();
                }
                else
                {
                    isTouching = true;
                    initialTouchPosition = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
    }

    // This is the default method used for moving tool.
    private void Movement()
    {
        if (gameStart)
        {
            // Regular movement outside of ramp
            if (isTouching)
            {
                // xDelta is calculated here although we get input in Update()
                // I saw that calculating xDelta in FixedUpdate results in smoother movement, so it is here
                xDelta = (Input.GetTouch(0).position.x - initialTouchPosition.x) / 10;
                initialTouchPosition = Input.GetTouch(0).position;
            }
            Vector3 forwardMovement = new Vector3(xDelta, 0, verticalSpeed) * speed;
            Vector3 newPosition = rb.position + forwardMovement * Time.fixedDeltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, -7.5f, 7.5f);
            rb.MovePosition(newPosition);
        }
    }
    
    public void ContinueMoving()
    {
        verticalSpeed = 1;
    }
    
    public void PlatformTransition()
    {
        rb.isKinematic = true; 
        boxCollider.enabled = false; 
        meshCollider.enabled = true; 
        gameStart = false;
        firstTouch = false;
    }
}