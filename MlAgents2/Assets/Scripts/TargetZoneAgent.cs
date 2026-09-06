using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TargetZoneAgent : Agent
{
    public float speedMultiplier = 0.1f;
    public float rotationMultiplier = 5f;

    private bool hasFoundTarget = false;

    public Transform targetTransform; 
    public Transform zoneTransform;

    public override void OnEpisodeBegin()
    {
        hasFoundTarget = false;

        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

        if (targetTransform != null)
        {
            targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
            targetTransform.gameObject.SetActive(true);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(hasFoundTarget ? 1.0f : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);

        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

        AddReward(-0.001f);

        if (transform.localPosition.y < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasFoundTarget && collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Target geraakt");
            hasFoundTarget = true;
            AddReward(0.5f);
            targetTransform.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Zone") && !hasFoundTarget)
        {
            Debug.Log("Zone gevonden");
            //AddReward(0.5f);
        }
        else if (hasFoundTarget && collision.gameObject.CompareTag("Zone"))
        {
            Debug.Log("Zone gevonden met target");
            AddReward(2.0f);
            EndEpisode();
        }
    }

    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    var c = actionsOut.ContinuousActions;

    //    // W/S (Vertical) = forward/back
    //    c[0] = Input.GetAxis("Vertical");

    //    // A/D (Horizontal) = turn left/right
    //    c[1] = Input.GetAxis("Horizontal");
    //}
}
