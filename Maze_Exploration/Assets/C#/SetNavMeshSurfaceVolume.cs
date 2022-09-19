using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-102)]
public class SetNavMeshSurfaceVolume : MonoBehaviour
{
    public Recursive maze;

    private NavMeshSurface ns;
    
    void Awake()
    {
        ns = GetComponent<NavMeshSurface>();
        ns.size = new Vector3((maze.width - 1) * maze.scale, 0, (maze.depth - 1) * maze.scale);
        ns.center = new Vector3((maze.width - 1) * maze.scale / 2.0f, -3.0f, (maze.depth - 1) * maze.scale / 2.0f);
    }

}
