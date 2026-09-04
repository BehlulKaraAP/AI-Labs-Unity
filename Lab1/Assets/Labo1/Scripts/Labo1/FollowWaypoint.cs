using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform[] waypoints;

    public float speed = 5f;

    public float rotSpeed = 5f;

    public float reachDistance = 1f;

    private int currentWaypoint = 0;
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Length == 0)
        {
            return;
        }

        Transform target = waypoints[currentWaypoint];

        Vector3 direction = target.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotSpeed * Time.deltaTime
            );
        }

        transform.Translate(0, 0, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < reachDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }
    }
}
