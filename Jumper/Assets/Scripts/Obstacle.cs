using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.localPosition.z < -5f)
        {
            Destroy(gameObject);
        }
    }
}
