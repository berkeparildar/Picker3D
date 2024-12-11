using Cinemachine;
using UnityEngine;

public class RampSection : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraManager cameraManager;

    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private Vector3 resetPosition;

    // This section of variables are used where player is repeatedly tapping.
    [SerializeField] private float powerDecreaseRate;
    [SerializeField] private float minPowerIncrease;
    [SerializeField] private float maxPowerIncrease;
    [SerializeField] private float maxPower;
    [SerializeField] private float currentPower;

    [SerializeField] private float speed;
    [SerializeField] private bool isOnRamp;

    // Transition to ramp movement here
    private void OnEnable()
    {
        isOnRamp = true;
        meshCollider.enabled = false;
        rb.isKinematic = false;
        boxCollider.enabled = true;
        gameManager.IncreaseLevel();
        uiManager.ToggleRampUI();
        cameraManager.ToggleRampCamera();
    }

    private void Update()
    {
        if (!isOnRamp) return;
        PlayerMovement();
        TapPowerUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RampEnd"))
        {
            LeftRamp();
        }
    }

    private void PlayerMovement()
    {
        Vector3 movement = transform.forward * speed + new Vector3(0, 0, currentPower / 2.5f);
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;
    }
    
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
        uiManager.UpdateTapMeter(currentPower, maxPower);
    }

    private void LeftRamp()
    {
        currentPower = 0;
        isOnRamp = false;
        uiManager.ToggleRampUI();
        gameManager.LoadNextLevel();
        transform.position = resetPosition;
        cameraManager.ToggleRampCamera();
        cameraManager.SetCameraLookAt(isFalling: true);
    }
}