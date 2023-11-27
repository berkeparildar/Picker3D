using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 normalTileScale;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CalculateAndGiveLandedAmount();
            StopPlayer();
            StartCoroutine(AnimateTiles());
            boxCollider.enabled = false;
        }
    }

    private void CalculateAndGiveLandedAmount()
    {
        float minDistance = 320;
        GameObject tempTile = null;
        for (int i = 0; i < tileParent.transform.childCount; i++)
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, tileParent.transform.GetChild(i).position);
            if (distanceToPlayer < minDistance)
            {
                minDistance = distanceToPlayer;
                tempTile = tileParent.transform.GetChild(i).gameObject;
            }
        }
        if (tempTile != null)
        {
            int landedAmount = int.Parse(tempTile.name);
            cameraManager.SetCameraLookAt(isFalling: false);
            StartCoroutine(gameManager.GetLandingZoneGemReward(landedAmount));
        }
    }

    private void StopPlayer()
    {
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.velocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
    }

    private IEnumerator AnimateTiles()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < tileParent.transform.childCount; i++)
        {
            tileParent.transform.GetChild(i).DOScale(0, 1);
        }
        yield return new WaitForSeconds(5);
        for (int i = 0; i < tileParent.transform.childCount; i++)
        {
            tileParent.transform.GetChild(i).DOScale(new Vector3(23, 0.1f, 9.9f), 1);
        }
        boxCollider.enabled = true;
    }
}