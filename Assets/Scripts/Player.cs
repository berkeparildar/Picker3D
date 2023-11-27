using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Rigidbody, MeshCollider and BoxCollider components of the player
    // The reason for two colliders is that on the ramp, the player's rigidbody
    // is no longer kinematic, and mesh colliders on non-kinematic body's are not supported
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private GameObject leftFlap;
    [SerializeField] private GameObject rightFlap;

    [SerializeField] private float speed;
    [SerializeField] private float verticalSpeed;

    [SerializeField] private UIManager uiManager;

    [SerializeField] private bool isTouching;
    [SerializeField] private bool firstTouch;
    [SerializeField] private Vector2 initialTouchPosition;
    [SerializeField] private float xDelta;

    private void OnEnable()
    {
        GetComponent<RampSection>().enabled = false;
        rb.isKinematic = true;
        boxCollider.enabled = false;
        meshCollider.enabled = true;
        firstTouch = false;
    }

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
                DeactivateFlaps();
                break;
            case "FlapActivator":
                ActivateFlaps();
                other.gameObject.SetActive(false);
                break;
            case "RampStart":
                GetComponent<RampSection>().enabled = true;
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
                    uiManager.ShowGameUI();
                    return;
                }
                isTouching = true;
                initialTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
    }

    // This is the default method used for moving the player.
    private void Movement()
    {
        if (firstTouch)
        {
            if (isTouching)
            {
                // xDelta is calculated here although we get input in Update()
                // I saw that calculating xDelta in FixedUpdate results in smoother movement, so it is here
                xDelta = (Input.GetTouch(0).position.x - initialTouchPosition.x) / 15;
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

    // This method activates the player's flaps
    // Called once player contacts with a power up.
    private void ActivateFlaps()
    {
        leftFlap.SetActive(true);
        rightFlap.SetActive(true);
        // Scaling for smooth appearance
        leftFlap.transform.DOScale(new Vector3(0.4f, 0.25f, 0.4f), 0.5f);
        rightFlap.transform.DOScale(new Vector3(0.4f, 0.25f, 0.4f), 0.5f);
        // Looped rotation
        leftFlap.transform.DORotate(new Vector3(0, 180, 0), 1).SetEase(Ease.Linear).SetLoops(-1);
        rightFlap.transform.DORotate(new Vector3(0, -180, 0), 1).SetEase(Ease.Linear).SetLoops(-1);
    }

    // This method deactivates the currently active flaps. 
    // This is called after reaching the end of the platform road, before the obstacle basket
    private void DeactivateFlaps()
    {
        // Sets their scale to zero for a fading animation.
        // Resets their rotation and deactivates
        leftFlap.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            DOTween.Kill(leftFlap.transform);
            leftFlap.transform.rotation = Quaternion.Euler(Vector3.zero);
            leftFlap.SetActive(false);
        });
        rightFlap.transform.DOScale(new Vector3(0.001f, 0.001f, 0.001f), 0.5f).OnComplete(() =>
        {
            DOTween.Kill(rightFlap.transform);
            rightFlap.transform.rotation = Quaternion.Euler(Vector3.zero);
            rightFlap.SetActive(false);
        });
    }

    public void ResetPlayer()
    {
        firstTouch = false;
        verticalSpeed = 1;
        transform.position = new Vector3(0, 0.6f, 0);
    }
}