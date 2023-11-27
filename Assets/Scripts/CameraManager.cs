using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera defaultCamera;
    [SerializeField] private GameObject rampCamera;
    [SerializeField] private GameObject defaultCameraPoint;
    [SerializeField] private GameObject fallCameraPoint;

    // Called when player enters and leaves the ramp
    public void ToggleRampCamera()
    {
        rampCamera.SetActive(!rampCamera.activeSelf);
    }

    public void SetCameraLookAt(bool isFalling)
    {
        if (isFalling)
        {
            defaultCamera.m_LookAt =
                fallCameraPoint.transform; // The look target is changed when player is falling of the ramp
        }
        else
        {
            defaultCamera.m_LookAt = defaultCameraPoint.transform;
        }
    }
}