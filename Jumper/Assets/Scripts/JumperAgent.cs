using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class JumperAgent : Agent
{
    private Rigidbody rb;
    public float jumpForce = 7f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    public ObstacleSpawner spawner;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        this.MaxStep = 2500;
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y < 0 || transform.localPosition.z < -2f)
        {
            transform.localPosition = new Vector3(0f, 0.5f, 0f);
            transform.localRotation = Quaternion.identity;
            rb.linearVelocity = Vector3.zero;
        }

        if (spawner != null)
        {
            spawner.ResetEpisodeSpawner();
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(isGrounded ? 1.0f : 0.0f);

        sensor.AddObservation(rb.linearVelocity.y);

        if (spawner != null)
        {
            sensor.AddObservation(spawner.currentObstacleSpeed);
        }
        else
        {
            sensor.AddObservation(5f);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        int jumpAction = actions.DiscreteActions[0];

        if (jumpAction == 1 && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        AddReward(0.001f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            Debug.Log("Obstakel geraakt");
            AddReward(-1.0f);
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.Space))
        {
            action[0] = 1;
        }
        else
        {
            action[0] = 0;
        }
    }
}
