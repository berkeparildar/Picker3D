using UnityEngine;

public class ExplodingObstacles : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private int shapeIndex;
    [SerializeField] private int amount;
    [SerializeField] private bool isDrop;
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
                rb.useGravity = true;
            }
            else
            {
                transform.Translate(Vector3.back * (10 * Time.deltaTime));
                if (transform.position.z - player.transform.position.z < 10)
                {
                    Debug.Log("Called");
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
            Debug.Log("Called");
            Explode();
            cd.isTrigger = true;
            Destroy(gameObject);
        }
    }

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
