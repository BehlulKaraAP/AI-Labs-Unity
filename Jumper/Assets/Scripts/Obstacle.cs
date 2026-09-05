using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;
    public ObstacleSpawner spawner;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.localPosition.z < -5f)
        {
            Destroy(gameObject);
        }
    }
    public void DestroySelf()
    {
        if (spawner != null)
        {
            spawner.RemoveObstacle(this);
        }
        Destroy(gameObject);
    }
}
