using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    [SerializeField] private GameObject defaultCameraPoint;

    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;

    [SerializeField] private bool rampSectionStarted;
    [SerializeField] private bool gameStart;
    [SerializeField] private bool hasLanded;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private bool isTouching;
    [SerializeField] private bool firstTouch;
    [SerializeField] private Vector2 initialTouchPosition;
    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private float xDelta;

    // This section of variables are used in ramp, where player is repeatedly tapping.
    [SerializeField] private float powerDecreaseRate = 30f;
    [SerializeField] private float minPowerIncrease = 10f;
    [SerializeField] private float maxPowerIncrease = 15f;
    [SerializeField] private float maxPower = 100;
    [SerializeField] private float currentPower;
    [SerializeField] private Vector3 rbVelocity;

    private void FixedUpdate()
    {
        Movement();
        rbVelocity = rb.velocity;
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
                Launch();
                break;
            case "LandingTiles":
                Land(other.gameObject.name);
                break;
        }
    }

    private void Update()
    {
        if (rampSectionStarted)
        {
            TapPowerUp();
            return;
        }
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
                    currentPosition = rb.position;
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
        // This movement is only for when climbing the ramp
        if (rampSectionStarted)
        {
            Vector3 movement = transform.forward * speed + new Vector3(0, 0, currentPower / 5);
            movement.y = rb.velocity.y;
            rb.velocity = movement;
            return;
        }
        
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

    // This method changes the rigidbody and collider settings
    // to move accordingly on the ramp
    private void RampTransition()
    {
        if (!rampSectionStarted)
        {
            rampCamera.SetActive(true); // Activate the Ramp's virtual camera
            meshCollider.enabled = false; // Deactivate MeshCollider
            rb.isKinematic = !rb.isKinematic; // Rigidbody is set to dynamic
            boxCollider.enabled = true; // BoxCollider is enabled
            rampSectionStarted = true;
            gameStart = false; // Ramp flag is set
        }
    }
    
    // Called at the end of the ramp for launching the player object
    private void Launch()
    {
        rampSectionStarted = false; // This boolean's purpose is to deactivate any other movement that player object har
        currentPower = 0;
        gameManager.LoadNextLevel();
        transform.position = new Vector3(0, 72, -300);
        rampCamera.SetActive(false);
        defaultCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = fallCameraPoint.transform;
    }

    // This method is for speeding up on the ramp
    private void TapPowerUp()
    {
        currentPower -= powerDecreaseRate * Time.deltaTime;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                currentPower += Random.Range(minPowerIncrease, maxPowerIncrease);
            }
        }
        currentPower = Mathf.Clamp(currentPower, 0, maxPower);
        
        // Update UI Here..
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
            defaultCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = defaultCameraPoint.transform;
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