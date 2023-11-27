using UnityEngine;

public class ExplodingObstacles : MonoBehaviour
{
    // This is the script for third platform obstacles that "explode" to smaller pieces
    // There are two variations, if player is close enough, one moves towards the player and explodes
    // the other falls down to the platform and explodes. I used the same script for both since they are fundamentally similar


    [SerializeField] private GameObject player;
    [SerializeField] private int shapeIndex; // This sets the shape of smaller pieces from object pools
    [SerializeField] private int amount; // amount of smaller pieces
    [SerializeField] private bool isDrop; // flag to check whether it is a drop one or move one
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ObstacleBasket obstacleBasket;
    [SerializeField] private Collider cd;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        obstacleBasket = GameObject.Find("ObstacleBasketThird").GetComponent<ObstacleBasket>();
        if (CompareTag("Drop"))
        {
            isDrop = true;
        }
        amount = 10;
    }

    private void Update()
    {
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        if (transform.position.z - player.transform.position.z < 35)
        {
            if (isDrop)
            {
                // For drop, gravity is enabled if player is close enough, explodes on contact with ground
                rb.useGravity = true;
            }
            else
            {
                transform.Translate(Vector3.back * (10 * Time.deltaTime));
                if (transform.position.z - player.transform.position.z < 10)
                {
                    // This is where "moving" ones explode. I checked this at collision too, but it kept being
                    // called more than once no matter what I did so I moved it here
                    Explode();
                    cd.isTrigger = true;
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isDrop)
        {
            Explode();
            cd.isTrigger = true;
            Destroy(gameObject);
        }
    }

    // This gets the given shape from object pool, and places them very close to exploding object
    private void Explode()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obstacle = ObjectPool.SharedInstance.GetPooledObstacle(shapeIndex);
            if (obstacle != null)
            {
                Vector3 currentPosition = transform.position;
                obstacle.transform.position = new Vector3(currentPosition.x + Random.Range(-1f, 1f),
                    currentPosition.y, currentPosition.z + Random.Range(-1f, 1f));
                obstacle.SetActive(true);
                obstacleBasket.pooledObjects.Add(obstacle);
            }
        }
    }
}