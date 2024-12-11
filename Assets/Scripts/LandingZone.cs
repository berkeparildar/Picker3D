using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
    // This is the script for handling operations related to Landing Zone
    // Landing zone is the long platform at the end of the level, where player will land to after 
    // launching off the ramp.
    
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject player;

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
        // I calculate the landed amount by checking the distance of each individual tile to player
        // However this is not ideal, because the player's anchor is not at its center.
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
            // The amount are stored in the tiles' names
            int landedAmount = int.Parse(tempTile.name);
            cameraManager.SetCameraLookAt(isFalling: false);
            StartCoroutine(gameManager.GetLandingZoneGemReward(landedAmount));
        }
    }

    // Method for stopping the player so that it does not slide to eternity
    private void StopPlayer()
    {
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.linearVelocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
    }

    // Shrinks the tiles after landing. Resets after tiles are no longer in camera's frame
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