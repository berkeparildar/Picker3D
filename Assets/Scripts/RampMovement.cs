using Cinemachine;
using UnityEngine;

public class RampMovement : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshCollider meshCollider;
    
    [SerializeField] private GameObject defaultCamera;
    [SerializeField] private GameObject rampCamera;
    [SerializeField] private GameObject defaultCameraPoint;
    [SerializeField] private GameObject fallCameraPoint;
    
    // This section of variables are used in ramp, where player is repeatedly tapping.
    [SerializeField] private float powerDecreaseRate = 30f;
    [SerializeField] private float minPowerIncrease = 10f;
    [SerializeField] private float maxPowerIncrease = 15f;
    [SerializeField] private float maxPower = 100;
    [SerializeField] private float currentPower;
    
    [SerializeField] private float speed;
    [SerializeField] private bool isOnRamp;

    private void OnEnable()
    {
        isOnRamp = true;
        rampCamera.SetActive(true); // Activate the Ramp's virtual camera
        meshCollider.enabled = false; // Deactivate MeshCollider
        rb.isKinematic = false; // Rigidbody is set to dynamic
        boxCollider.enabled = true; // BoxCollider is enabled
    }
    
    void Update()
    {
        if (isOnRamp)
        {
            Movement();
            TapPowerUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "RampEnd":
                Launch();
                break;
            case "LandingTiles":
                Land(other.gameObject.name);
                break;
        }
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

        // Update UI Here..
    }
    
    private void Launch()
    {
        currentPower = 0;
        isOnRamp = false;
        gameManager.LoadNextLevel();
        transform.position = new Vector3(0, 72, -300);
        rampCamera.SetActive(false);
        defaultCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = fallCameraPoint.transform;
    }
    
    private void Movement()
    {
        Vector3 movement = transform.forward * speed + new Vector3(0, 0, currentPower / 5);
        movement.y = rb.velocity.y;
        rb.velocity = movement;
    }

    private void Land(string gameObjectName)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        int landedAmount = int.Parse(gameObjectName);
        gameManager.IncreaseGemCount(landedAmount);
        defaultCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = defaultCameraPoint.transform;
        StartCoroutine(gameManager.ResetLevel());
    }
}