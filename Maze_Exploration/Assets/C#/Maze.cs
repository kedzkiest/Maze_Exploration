﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MapLocation       
{
    public int x;
    public int z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, z);
    }

    public static MapLocation operator +(MapLocation a, MapLocation b)
       => new MapLocation(a.x + b.x, a.z + b.z);

}

public class Maze : MonoBehaviour
{
    public List<MapLocation> directions = new List<MapLocation>() {
                                            new MapLocation(1,0),
                                            new MapLocation(0,1),
                                            new MapLocation(-1,0),
                                            new MapLocation(0,-1) };
    public int width = 30; //x length
    public int depth = 30; //z length
    public byte[,] map;
    public int scale = 6;
    public Material wallMaterial;
    public GameObject player;
    public GameObject goalLight;
    public GameObject runningEnemy;
    public float minimumDistanceToGoal;
    public float maximumDistanceToGoal;
    public int numberOfRunningEnemies;
    
    void Awake()
    {
        InitialiseMap();
        Generate();
        DrawMap();
        GetComponent<NavMeshSurface>().BuildNavMesh();
        InitialisePositions();
    }
    
    void InitialiseMap()
    {
        map = new byte[width,depth];
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                    map[x, z] = 1;     //1 = wall  0 = corridor
            }
    }

    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
               if(Random.Range(0,100) < 50)
                 map[x, z] = 0;     //1 = wall  0 = corridor
            }
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.isStatic = true;
                    wall.GetComponent<Renderer>().material = wallMaterial;
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                    wall.transform.position = pos;
                }
            }
    }
    
    List<MapLocation> emptyPositions = new List<MapLocation>{};
    void InitialisePositions()
    {
        Vector3 playerPos, goalPos;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 targetPos = new Vector3(x, -1.5f, z);
                if(!(Physics.OverlapSphere(targetPos, 0).Length > 0) && map[x, z] == 0)
                {
                    emptyPositions.Add(new MapLocation(x, z));
                }
                
            }
        }
        
        
        int r = Random.Range(0, emptyPositions.Count);
        playerPos = new Vector3(emptyPositions.ElementAt(r).x * scale, -1.7f,
            emptyPositions.ElementAt(r).z * scale);
        Instantiate(player, playerPos, Quaternion.identity);

        int cnt = 0;
        
        while (true)
        {
            r = Random.Range(0, emptyPositions.Count);
            goalPos = new Vector3(emptyPositions.ElementAt(r).x * scale, -3.0f,
                emptyPositions.ElementAt(r).z * scale);
            if (Vector3.Distance(playerPos, goalPos) >= minimumDistanceToGoal &&
                Vector3.Distance(playerPos, goalPos) <= maximumDistanceToGoal)
            {
                Instantiate(runningEnemy, goalPos + new Vector3(0, -3.0f, 0), Quaternion.identity);
                cnt++;
            }

            if (cnt == numberOfRunningEnemies) break;
        }
        
        
        /*
        while (true)
        {
            r = Random.Range(0, emptyPositions.Count);
            goalPos = new Vector3(emptyPositions.ElementAt(r).x * scale, -3.0f,
                emptyPositions.ElementAt(r).z * scale);
            if (Vector3.Distance(playerPos, goalPos) >= minimumDistanceToGoal &&
                Vector3.Distance(playerPos, goalPos) <= maximumDistanceToGoal)
            {
                Instantiate(enemy, goalPos + new Vector3(0, -3.0f, 0), Quaternion.identity);
                cnt++;
            }

            if (cnt == numberOfEnemies) break;
        }
        */
        
        Instantiate(goalLight, goalPos, Quaternion.Euler(-90, 0, 0));
    }

    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x,z) + CountDiagonalNeighbours(x,z);
    }

    public Vector3 FindMostUpperLeftPoint()
    {
        List<MapLocation> emptyLocations_copy = new List<MapLocation>(emptyPositions);
        emptyLocations_copy.Sort((a, b) => (a.x + a.z) - (b.x + b.z));
        MapLocation answerPoint = emptyLocations_copy.ElementAt(0);

        return new Vector3(answerPoint.x * scale, 0, answerPoint.z * scale);
    }
    
    public Vector3 FindMostUpperRightPoint()
    {
        List<MapLocation> emptyLocations_copy = new List<MapLocation>(emptyPositions);
        IOrderedEnumerable<MapLocation> emptyLocations_copy2 = emptyLocations_copy.OrderBy(a => a.x).ThenByDescending(a => a.z);
        MapLocation answerPoint = emptyLocations_copy2.ElementAt(0);

        return new Vector3(answerPoint.x * scale, 0, answerPoint.z * scale);
    }
    public Vector3 FindMostLowerRightPoint()
    {
        List<MapLocation> emptyLocations_copy = new List<MapLocation>(emptyPositions);
        emptyLocations_copy.Sort((a, b) => (a.x + a.z) - (b.x + b.z));
        MapLocation answerPoint = emptyLocations_copy.ElementAt(emptyLocations_copy.Count - 1);

        return new Vector3(answerPoint.x * scale, 0, answerPoint.z * scale);
    }

    public Vector3 FindMostLowerLeftPoint()
    {
        List<MapLocation> emptyLocations_copy = new List<MapLocation>(emptyPositions);
        IOrderedEnumerable<MapLocation> emptyLocations_copy2 = emptyLocations_copy.OrderByDescending(a => a.x).ThenBy(a => a.z);
        MapLocation answerPoint = emptyLocations_copy2.ElementAt(0);

        return new Vector3(answerPoint.x * scale, 0, answerPoint.z * scale);
    }

    public Vector3 FindMostCenterPoint()
    {
        float minDiff = width * depth;
        Vector3 centerPoint = new Vector3(width/2.0f, 0, depth/2.0f);
        Vector3 answerPoint = Vector3.zero;
        
        List<MapLocation> emptyLocations_copy = new List<MapLocation>(emptyPositions);
        foreach (MapLocation m in emptyLocations_copy)
        {
            Vector3 testPoint = new Vector3(m.x, 0, m.z);
            float testDistance = Vector3.Distance(centerPoint, testPoint);
            if (testDistance < minDiff)
            {
                minDiff = testDistance;
                answerPoint = testPoint;
            }
        }

        return answerPoint * scale;
    }
    
    
    
}