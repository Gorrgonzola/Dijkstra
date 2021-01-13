﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName ="Graph")]
public class GraphSO : ScriptableObject
{
    [HideInInspector]
    public int NumOfVertices = 0;
    [HideInInspector]
    public List<Vertex> Vertices = new List<Vertex>();
    public List<List<Tuple<Vertex, int>>> Adjacency = new List<List<Tuple<Vertex, int>>>();

    #region Add Vertex/Edge
    public Vertex AddVertex()
    {
        var v = new Vertex
        {
            Id = NumOfVertices,
            Type = VertexType.NONE
        };
        Vertices.Add(v);
        Adjacency.Add(new List<Tuple<Vertex, int>>());
        NumOfVertices++;
        return v;
    }
    public void AddEdge(Vertex v1, Vertex v2, int weight)
    {
        if (v1.Equals(v2))
        {
            Debug.LogWarning($"You picked the same vertex. This is not available.");
            return;
        }

        var oldEdgeValue = Adjacency[v1.Id].Where(e => e.Item1 == v2).FirstOrDefault();
        if (oldEdgeValue != null)
        {
            Adjacency[v1.Id].Remove(oldEdgeValue);
        }
        oldEdgeValue = Adjacency[v2.Id].Where(e => e.Item1 == v1).FirstOrDefault();
        if (oldEdgeValue != null)
        {
            Adjacency[v2.Id].Remove(oldEdgeValue);
        }

        Adjacency[v1.Id].Add(new Tuple<Vertex, int>(v2, weight));
        Adjacency[v2.Id].Add(new Tuple<Vertex, int>(v1, weight));

        Adjacency[v1.Id].Sort((x1, x2) => x2.Item1.Id - x1.Item1.Id);
        Adjacency[v2.Id].Sort((x1, x2) => x2.Item1.Id - x1.Item1.Id);

        Debug.Log("Edge created");
    }
    #endregion

    /// <summary>
    /// Dijkstra's algorithm
    /// </summary>
    /// <param name="start">Source</param>
    /// <param name="end">Destination</param>
    /// <returns>Vertex path from start to end.</returns>
    public Stack<Vertex> FindShortestPath(Vertex start, Vertex end)
    {
        HashSet<Vertex> vertexSet = new HashSet<Vertex>();
        Dictionary<Vertex, Vertex> prevVertex = new Dictionary<Vertex, Vertex>();

        start.TentativeDist = 0;
        prevVertex.Add(start, null);
        vertexSet.Add(start);

        foreach (var vertex in Vertices)
        {
            if (vertex.Equals(start))
                continue;
            vertex.TentativeDist = Mathf.Infinity;
            prevVertex.Add(vertex, null);
            vertexSet.Add(vertex);
        }

        Vertex minV = null;
        while (vertexSet.Count > 0)
        {
            minV = vertexSet.Aggregate((curMin, v) => curMin.TentativeDist < v.TentativeDist ? curMin : v);
            vertexSet.Remove(minV);

            if (minV.Equals(end))
                break;

            foreach (var edge in Adjacency[minV.Id])
            {
                var v = edge.Item1;
                if (!vertexSet.Contains(v))
                    continue;
                var dist = minV.TentativeDist + edge.Item2;
                if (dist < v.TentativeDist)
                {
                    v.TentativeDist = dist;
                    prevVertex[v] = minV;
                }
            }

            
        }

        var stack = new Stack<Vertex>();
        if (prevVertex[minV] == null || minV.Equals(start))
        {
            Debug.LogWarning("Couldn't find traversable path from end to start");
            return null;
        }
        while (minV != null)
        {
            stack.Push(minV);
            //Debug.Log(minV.Id);
            minV = prevVertex[minV];
        }

        return stack;
    }

    #region Deletion
    public void DeleteVertex(Vertex v)
    {
        Vertices.Remove(v);
        Adjacency.RemoveAt(v.Id);
        foreach (var adj in Adjacency)
        {
            foreach (var edge in adj)
            {
                if (edge.Item1 == v)
                    adj.Remove(edge);
            }
        }
    }

    public void DeleteAllVertices()
    {
        NumOfVertices = 0;
        Vertices.Clear();
        Adjacency.Clear();
    }
    #endregion

    /*
private void OnValidate()
{
    var allVertices = FindObjectsOfType<VertexGO>();
    if (allVertices.Length != _vertices.Count)
    {
        for (int i = allVertices.Length-1; i >= 0; i--)
        {
            var v = allVertices[i];
            Destroy(v);
        }
    }
}
*/
}
