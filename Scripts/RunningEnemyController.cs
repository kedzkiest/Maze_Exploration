using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class RunningEnemyController : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent agent;

    private Recursive maze;

    private Vector3 mostUpperLeftPoint;
    private Vector3 mostUpperRightPoint;
    private Vector3 mostLowerRightPoint;
    private Vector3 mostLowerLeftPoint;
    private Vector3 centerPoint;
    private Vector3[] waypoints = new Vector3[5];
    private List<Vector3> waypointsList;
    private int currentDestinationIndex;
    private Vector3 currentDestination;
    private Vector3 pastDestination;

    public float sensingDistance;
    public float sensingFOV;
    public Transform rayOrigin;
    public bool isChasing;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        maze = GameObject.FindGameObjectWithTag("Maze").GetComponent<Recursive>();
        
        mostUpperLeftPoint = maze.FindMostUpperLeftPoint();
        mostUpperRightPoint = maze.FindMostUpperRightPoint();
        mostLowerRightPoint = maze.FindMostLowerRightPoint();
        mostLowerLeftPoint = maze.FindMostLowerLeftPoint();
        centerPoint = maze.FindMostCenterPoint();
        
        /* position check
        GameObject UpperLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UpperLeft.transform.position = mostUpperLeftPoint;
        
        GameObject UpperRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UpperRight.transform.position = mostUpperRightPoint;
        
        GameObject LowerRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        LowerRight.transform.position = mostLowerRightPoint;
        
        GameObject LowerLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        LowerLeft.transform.position = mostLowerLeftPoint;
        
        GameObject Center = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Center.transform.position = centerPoint;
        */
            
        waypoints[0] = mostUpperLeftPoint;
        waypoints[1] = mostUpperRightPoint;
        waypoints[2] = mostLowerRightPoint;
        waypoints[3] = mostLowerLeftPoint;
        waypoints[4] = centerPoint;

        waypointsList = waypoints.ToList();

        currentDestinationIndex = Random.Range(0, waypointsList.Count);
        currentDestination = waypoints.ElementAt(currentDestinationIndex);
        waypointsList.RemoveAt(currentDestinationIndex);
        pastDestination = currentDestination;
        agent.SetDestination(waypoints[currentDestinationIndex]);

        isChasing = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 thisPos = gameObject.transform.position;
        Vector3 playerPos = player.transform.position - new Vector3(0, 2.0f, 0);

    
        if (Vector3.Distance(thisPos, playerPos) < sensingDistance &&
            Vector3.Angle(gameObject.transform.forward, playerPos - thisPos) < sensingFOV)
        {
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin.position, playerPos - thisPos, out hit))
            {
                Debug.DrawRay(rayOrigin.position, (playerPos - thisPos) * 10, Color.green, 0.1f);
                if (hit.collider.CompareTag("Player"))
                {
                    isChasing = true;
                }
            }
        }


        if (isChasing)
        {
            if(Vector3.Distance(thisPos, playerPos) > sensingDistance ||
               Vector3.Angle(gameObject.transform.forward, playerPos - thisPos) > sensingFOV)
            {
                RaycastHit hit;
            
                if (Physics.Raycast(rayOrigin.position, playerPos - thisPos, out hit, sensingDistance))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        isChasing = false;
                        currentDestinationIndex = Random.Range(0, waypointsList.Count);
                        currentDestination = waypoints.ElementAt(currentDestinationIndex);
                        waypointsList.RemoveAt(currentDestinationIndex);
                        waypointsList.Add(pastDestination);
                        pastDestination = currentDestination;
                        agent.SetDestination(waypoints[currentDestinationIndex]);
                    }
                }
            }
            agent.SetDestination(player.transform.position);
        }
        else
        {
            if (agent.remainingDistance < 1.0f)
            {
                currentDestinationIndex = Random.Range(0, waypointsList.Count);
                currentDestination = waypoints.ElementAt(currentDestinationIndex);
                waypointsList.RemoveAt(currentDestinationIndex);
                waypointsList.Add(pastDestination);
                pastDestination = currentDestination;
                agent.SetDestination(waypoints[currentDestinationIndex]);;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
