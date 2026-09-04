using TMPro;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public WPManager wpManager;
    public TMP_Dropdown dropdown;

    public float speed = 5f;
    public float rotSpeed = 5f;

    private GameObject currentNode;
    private int currentPathIndex = 0;
    private bool isMoving = false;
    
    void Start()
    {
        if (wpManager.waypoints.Length > 0)
        {
            currentNode = wpManager.waypoints[0];
        }

        dropdown.onValueChanged.AddListener(delegate
        {
            CalculateRoute(dropdown.value);
        });
    }

    void CalculateRoute(int targetIndex)
    {
        GameObject targetNode = wpManager.waypoints[targetIndex];

        if (wpManager.graph.AStar(currentNode, targetNode))
        {
            currentPathIndex = 0;
            isMoving = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving || wpManager.graph.pathList.Count == 0) return;

        if (currentPathIndex < wpManager.graph.pathList.Count)
        {
            Transform target = wpManager.graph.pathList[currentPathIndex].getID().transform;
            Vector3 direction = target.position - transform.position;
            direction.y = 0; 

            if (direction.magnitude > 1f)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime);
                transform.Translate(0, 0, speed * Time.deltaTime);
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= wpManager.graph.pathList.Count)
                {
                    isMoving = false;
                    currentNode = target.gameObject; 
                }
            }
        }
    }
}
