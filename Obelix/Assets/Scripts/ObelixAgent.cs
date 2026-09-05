using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class ObelixAgent : Agent
{
    public GameObject menhirPrefab;
    public GameObject destinationPrefab;

    private List<GameObject> menhirs = new List<GameObject>();
    private List<GameObject> destinations = new List<GameObject>();

    public int amount = 3;

    public float minX = -10f; 
    public float maxX = 10f; 
    public float minZ = -10f; 
    public float maxZ = 10f;

    public float speedMultiplier = 0.1f;
    public float rotationMultiplier = 5f;

    private bool hasMenhir = false;
    public override void OnEpisodeBegin()
    {
        foreach (GameObject menhir in menhirs)
        {
            Destroy(menhir);
        }
        menhirs.Clear();

        foreach (GameObject destination in destinations)
        {
            Destroy(destination);
        }
        destinations.Clear();

        // reset de positie en orientatie als de agent gevallen is
        if (this.transform.localPosition.y < 0)
        {

            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            this.transform.localRotation = Quaternion.identity;
        }

        Vector3 areaCenter = this.transform.position;

        for (int i = 0; i < amount; i++)
        {
            // Genereer een willekeurige offset rondom HET EIGEN MIDDEN van deze area
            float x = Random.Range(areaCenter.x - 5f, areaCenter.x + 5f);
            float z = Random.Range(areaCenter.z - 5f, areaCenter.z + 5f);

            Vector3 spawnPosition = new Vector3(x, 0.5f, z);
            GameObject newMenhir = Instantiate(menhirPrefab, spawnPosition, Quaternion.identity);
            menhirs.Add(newMenhir);
        }

        for (int i = 0; i < amount; i++)
        {
            // Genereer een willekeurige offset rondom HET EIGEN MIDDEN van deze area
            float x = Random.Range(areaCenter.x - 5f, areaCenter.x + 5f);
            float z = Random.Range(areaCenter.z - 5f, areaCenter.z + 5f);

            Vector3 spawnPosition = new Vector3(x, 0.5f, z);
            GameObject newDestination = Instantiate(destinationPrefab, spawnPosition, Quaternion.identity);
            destinations.Add(newDestination);
        }

        //for (int i = 0; i < amount; i++) 
        //{
        //    float x = Random.Range(minX, maxX); 
        //    float z = Random.Range(minZ, maxZ); 
        //    Vector3 spawnPosition = new Vector3(x, 0.5f, z); 
        //    GameObject newMenhir = Instantiate(menhirPrefab, spawnPosition, Quaternion.identity);
        //    menhirs.Add(newMenhir);
        //} 

        //for (int i = 0; i < amount; i++)
        //{
        //    float x = Random.Range(minX, maxX);
        //    float z = Random.Range(minZ, maxZ);
        //    Vector3 spawnPosition = new Vector3(x, 0.5f, z);
        //    GameObject newDestination = Instantiate(destinationPrefab, spawnPosition, Quaternion.identity);
        //    destinations.Add(newDestination);
        //}

        hasMenhir = false;

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward);

        sensor.AddObservation(hasMenhir ? 1.0f : 0.0f);

        Vector3 nearestTargetPos = GetNearestTargetPosition();
        sensor.AddObservation(nearestTargetPos - this.transform.localPosition);
    }

    Vector3 GetNearestTargetPosition()
    {
        float minDistance = Mathf.Infinity;
        Vector3 nearestPos = this.transform.localPosition;

        if (!hasMenhir)
        {
            foreach (GameObject m in menhirs)
            {
                if (m != null)
                {
                    float dist = Vector3.Distance(this.transform.localPosition, m.transform.localPosition);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestPos = m.transform.localPosition;
                    }
                }
            }
        }
        else
        {
            foreach (GameObject d in destinations)
            {
                if (d != null)
                {
                    float dist = Vector3.Distance(this.transform.localPosition, d.transform.localPosition);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestPos = d.transform.localPosition;
                    }
                }
            }
        }
        return nearestPos;
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);

        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);
        
        AddReward(-0.002f);

        if (transform.localPosition.y < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Menhir") && hasMenhir == false)
        {
            Debug.Log("Menhir geraakt zonder een menhir");
            hasMenhir = true;
            AddReward(0.5f);

            menhirs.Remove(collision.gameObject);
            Destroy(collision.gameObject);

        }
        if (collision.gameObject.CompareTag("Destination") && hasMenhir == true)
        {
            Debug.Log("Destination geraakt met een menhir");
            hasMenhir = false;
            AddReward(1.5f);
           
            destinations.Remove(collision.gameObject);
            Destroy(collision.gameObject);

            if (destinations.Count == 0)
            {
                AddReward(2f);
                EndEpisode();
            }
        }
        if (collision.gameObject.CompareTag("Menhir") && hasMenhir == true)
        {
            Debug.Log("Menhir geraakt met menhir");
            AddReward(-0.1f);
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
