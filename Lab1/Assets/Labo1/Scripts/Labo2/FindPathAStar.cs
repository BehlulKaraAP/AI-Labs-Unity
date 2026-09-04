using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class FindPathAStar : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;
    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    PathMarker startNode;
    PathMarker goalNode;
    PathMarker lastPos;
    bool done = false;
    bool hasStarted = false;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    List<PathMarker> path = new List<PathMarker>();

    void RemoveAllMarkers()
    {

        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");

        foreach (GameObject m in markers) Destroy(m);

        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // Destroy(goal);
        if (player != null) Destroy(player);
    }

    void BeginSearch()
    {

        done = false;
        hasStarted = true;

        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();

        for (int z = 1; z < maze.depth - 1; ++z)
        {
            for (int x = 1; x < maze.width - 1; ++x)
            {

                if (maze.map[x, z] != 1)
                {
                    locations.Add(new MapLocation(x, z));
                }
            }
        }
        locations.Shuffle();

        //Vector3 startLocation = new Vector3(1, 0.5f, 1);
        //startNode = new PathMarker(new MapLocation(1, 1),
        //    0.0f, 0.0f, 0.0f, Instantiate(start, startLocation, Quaternion.identity), null);

        //Vector3 endLocation = new Vector3(Random.Range(5, 8), 0.5f, Random.Range(5, 8));
        //goalNode = new PathMarker(new MapLocation((int)endLocation.x, (int)endLocation.z),
        //    0.0f, 0.0f, 0.0f, Instantiate(end, endLocation, Quaternion.identity), null);

        MapLocation startLocation = locations[0];
        MapLocation endLocation = locations[1];

        startNode = new PathMarker(
            startLocation,
            0.0f,
            0.0f,
            0.0f,
            Instantiate(
                start,
                new Vector3(startLocation.x * maze.scale, 0.5f,
                            startLocation.z * maze.scale),
                Quaternion.identity
            ),
            null
        );

        goalNode = new PathMarker(
            endLocation,
            0.0f,
            0.0f,
            0.0f,
            Instantiate(
                end,
                new Vector3(endLocation.x * maze.scale, 0.5f,
                            endLocation.z * maze.scale),
                Quaternion.identity
            ),
            null
        );

        open.Clear();
        closed.Clear();
        path.Clear();

        open.Add(startNode);
        lastPos = startNode;

        StartCoroutine(Searching());
    }

    void Search(PathMarker thisNode)
    {

        if (thisNode.Equals(goalNode))
        {

            done = true;

            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            Debug.Log("Direction: " + dir.x + ", " + dir.z);
            Debug.Log("Current: " + thisNode.location.x + ", " + thisNode.location.z);
            MapLocation neighbour = dir + thisNode.location;

            //if (neighbour.x < 1 || neighbour.x > maze.width || neighbour.z < 1 || neighbour.z > maze.depth) continue;
            if (neighbour.x < 0 || neighbour.x >= maze.width || neighbour.z < 0 || neighbour.z >= maze.depth) continue;

            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (IsClosed(neighbour)) continue;

            float g = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float h = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float f = g + h;

            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0.0f, neighbour.z * maze.scale), Quaternion.identity);

            if (!UpdateMarker(neighbour, g, h, f, thisNode))
            {

                open.Add(new PathMarker(neighbour, g, h, f, pathBlock, thisNode));
            }
        }

        if (open.Count() == 0)
        {
            Debug.LogWarning("Geen pad meer zoeken gestopt");
            done = true;
            return;
        }
        open = open.OrderBy(p => p.F).ToList<PathMarker>();
        PathMarker pm = (PathMarker)open.ElementAt(0);
        closed.Add(pm);

        open.RemoveAt(0);
        //pm.marker.GetComponent<Renderer>().material = closedMaterial;

        lastPos = pm;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {

        foreach (PathMarker p in open)
        {

            if (p.location.Equals(pos))
            {

                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker)
    {

        foreach (PathMarker p in closed)
        {

            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    void Start()
    {
        BeginSearch();
    }

    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.P))
        //{

        //    BeginSearch();
        //    hasStarted = true;
        //}


        //if (hasStarted)
        //    if (Input.GetKeyDown(KeyCode.C)) Search(lastPos);
    }

    // The coroutine function
    //bool searchingHasFinished = false;
    IEnumerator Searching()
    {
        Debug.Log("searching started!");

        while (!done)
        {
            // Perform some task
            Debug.Log("Coroutine is running...");
            Search(lastPos);
            yield return null;
            // Wait for the next frame
            //            yield return true;
        }

        //searchingHasFinished = true;
        //yield return null;

        Debug.Log("Coroutine finished!");
        ReconstructPath();
        StartCoroutine(MovePlayer());
    }

    //bool PathHasConstructed = false;
    void ReconstructPath()
    {

        //path.Add(closed[closed.Count - 1]);
        //var p = closed[closed.Count - 1].parent;
        //while (p != startNode)
        //{
        //    path.Insert(0, p);
        //    p = p.parent;
        //}
        //path.Insert(0, startNode);

        path.Clear();

        PathMarker current = lastPos;

        // Loop terug via de parents tot we bij het begin zijn
        while (current != null)
        {
            path.Insert(0, current);
            current = current.parent;
        }

        Debug.Log("Pad gereconstrueerd! Aantal stappen: " + path.Count);

        // Teken het berekende pad in de scene
        foreach (PathMarker node in path)
        {
            Instantiate(
                pathP,
                new Vector3(
                    node.location.x * maze.scale,
                    0.1f, // Teken iets lager zodat het de speler niet blokkeert
                    node.location.z * maze.scale
                ),
                Quaternion.identity
            );
        }
    }

    IEnumerator MovePlayer()
    {
        GameObject player = startNode.marker;

        foreach (PathMarker node in path)
        {
            Vector3 targetPosition = new Vector3(
                node.location.x * maze.scale,
                0.5f,
                node.location.z * maze.scale
            );

            player.transform.position = targetPosition;

            yield return new WaitForSeconds(1f);
        }


    }
}
