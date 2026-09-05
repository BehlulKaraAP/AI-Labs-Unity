using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnInterval = 2.0f;
    private float timer;
    public float currentObstacleSpeed = 5f;
    public List<Obstacle> activeObstacles = new List<Obstacle>();

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    public void ResetEpisodeSpawner()
    {
        currentObstacleSpeed = Random.Range(4f, 10f);
        timer = 0f;

        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] != null)
            {
                Destroy(activeObstacles[i].gameObject);
            }
        }
        activeObstacles.Clear();
    }

    private void SpawnObstacle()
    {
        GameObject obstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity);
        Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();
        if (obstacleScript != null)
        {
            obstacleScript.speed = currentObstacleSpeed;
            obstacleScript.spawner = this;
            activeObstacles.Add(obstacleScript);
        }
    }
    public void RemoveObstacle(Obstacle obs)
    {
        if (activeObstacles.Contains(obs))
        {
            activeObstacles.Remove(obs);
        }
    }
}
