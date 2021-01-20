using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName ="Graph")]
public class GraphSO : ScriptableObject
{
    [HideInInspector]
    public List<Vertex> Vertices = new List<Vertex>();
    public List<List<Edge>> Adjacency = new List<List<Edge>>();

    public int NumOfVertices { get => _numOVertices; set => _numOVertices = value; }

    private int _numOVertices = 0;

    #region Add Vertex/Edge
    public Vertex AddVertex()
    {
        var v = new Vertex
        {
            Id = NumOfVertices,
            Type = VertexType.NONE
        };
        Vertices.Add(v);
        Adjacency.Add(new List<Edge>());
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

        Adjacency[v1.Id].RemoveAll(e => e.AdjacentV == v2);
        Adjacency[v2.Id].RemoveAll(e => e.AdjacentV == v1);

        Adjacency[v1.Id].Add(new Edge(v2, weight));
        Adjacency[v2.Id].Add(new Edge(v1, weight));

        Adjacency[v1.Id].Sort((x1, x2) => x2.AdjacentV.Id - x1.AdjacentV.Id);
        Adjacency[v2.Id].Sort((x1, x2) => x2.AdjacentV.Id - x1.AdjacentV.Id);
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
                var v = edge.AdjacentV;
                if (!vertexSet.Contains(v))
                    continue;
                var dist = minV.TentativeDist + edge.Weight;
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
            minV = prevVertex[minV];
        }

        return stack;
    }

    #region Deletion
    public void DeleteAllVertices()
    {
        NumOfVertices = 0;
        Vertices.Clear();
        Adjacency.Clear();
    }
    #endregion
}
