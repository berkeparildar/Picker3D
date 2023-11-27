using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera defaultCamera;
    [SerializeField] private GameObject rampCamera;
    [SerializeField] private GameObject defaultCameraPoint;
    [SerializeField] private GameObject fallCameraPoint;

    public void ToggleRampCamera()
    {
        rampCamera.SetActive(!rampCamera.activeSelf);
    }

    public void SetCameraLookAt(bool isFalling)
    {
        if (isFalling)
        {
            defaultCamera.m_LookAt = fallCameraPoint.transform;
        }
        else
        {
            defaultCamera.m_LookAt = defaultCameraPoint.transform;
        }
    }
}
