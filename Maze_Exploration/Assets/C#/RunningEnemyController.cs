using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.EditorTools;
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
    private bool isDestinationChanged;

    public float normalSensingDistance;
    public float strongSensingDistance;
    private float sensingDistance;
    public float normalSensingFOV;
    public float strongSensingFOV;
    private float sensingFOV;
    public Transform rayOrigin;
    public bool isChasing;

    private LightScript lightScript;

    private UIController UIController;

    public float normalMoveSpeed;
    public float chasingMoveSpeed;

    public float infoUpdateFreq;
    private float distToPlayer;
    private float angleToPlayer;
    private float elapsedTime;

    private readonly string[] deathReason =
    {
        "走り回る者に轢かれた",
        "走り回る者と交通事故を起こした",
        "走り回る者と正面衝突した",
        "走り回る者に襲われた"
    };

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

        /* check position
        GameObject UpperLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UpperLeft.transform.position = mostUpperLeftPoint;
        
        GameObject UpperRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        UpperRight.transform.position = mostUpperRightPoint;
        
        GameObject LowerRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        LowerRight.transform.position = mostLowerRightPoint;
        
        GGameObject.CreatePrimitive(PrimitiveType.Cube);
        
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
        currentDestination = waypointsList.ElementAt(currentDestinationIndex);
        waypointsList.RemoveAt(currentDestinationIndex);
        pastDestination = currentDestination;
        agent.SetDestination(currentDestination);

        isChasing = false;

        lightScript = GameObject.FindGameObjectWithTag("FlashLight").GetComponent<LightScript>();

        UIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        Vector3 thisPos = gameObject.transform.position;
        Vector3 playerPos = player.transform.position - new Vector3(0, 2.0f, 0);

        if(elapsedTime > infoUpdateFreq)
        {
            elapsedTime = 0;
            distToPlayer = Vector3.Distance(thisPos, playerPos);
            angleToPlayer = Vector3.Angle(gameObject.transform.forward, playerPos - thisPos);
        }



        if (distToPlayer < sensingDistance && angleToPlayer < sensingFOV)
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
            agent.speed = chasingMoveSpeed;
            
            if(distToPlayer > sensingDistance || angleToPlayer > sensingFOV)
            {
                RaycastHit hit;
            
                if (Physics.Raycast(rayOrigin.position, playerPos - thisPos, out hit, sensingDistance))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        isChasing = false;
                        currentDestinationIndex = Random.Range(0, waypointsList.Count);
                        currentDestination = waypointsList.ElementAt(currentDestinationIndex);
                        waypointsList.RemoveAt(currentDestinationIndex);
                        waypointsList.Add(pastDestination);
                        pastDestination = currentDestination;
                        agent.SetDestination(currentDestination);
                    }
                }
            }
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.speed = normalMoveSpeed;
            
            if (agent.remainingDistance < 1.0f && !isDestinationChanged)
            {
                currentDestinationIndex = Random.Range(0, waypointsList.Count);
                currentDestination = waypointsList.ElementAt(currentDestinationIndex);
                waypointsList.RemoveAt(currentDestinationIndex);
                waypointsList.Add(pastDestination);
                pastDestination = currentDestination;
                agent.SetDestination(currentDestination);
                isDestinationChanged = true;
            }

            if(agent.remainingDistance >= 1.0f)
            {
                isDestinationChanged = false;
            }
        }
        
        // change sensing distance and FOV depend on player's light state
        if (lightScript.isLightOn)
        {
            sensingDistance = strongSensingDistance;
            sensingFOV = strongSensingFOV;
        }
        else
        {
            sensingDistance = normalSensingDistance;
            sensingFOV = normalSensingFOV;
        }

        // Display where the enemy going
        Debug.DrawRay(transform.position, currentDestination - transform.position, Color.red);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !UIController.canEscape)
        {
            GameOver.reason = "あなたは" + deathReason[Random.Range(0, 4)] + "...";
            SceneManager.LoadScene("GameOver");
        }
    }

    public void VisitDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        waypointsList.Add(pastDestination);
    }
}
