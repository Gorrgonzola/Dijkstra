using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
[CreateAssetMenu(menuName = "Graph")]
public class GraphSO : ScriptableObject
{
    [HideInInspector]
    public List<Vertex> Vertices;
    public List<ListEdgeWrapper> Adjacency;

    public int NumOfVertices { get => _numOVertices; set => _numOVertices = value; }

    [HideInInspector]
    [SerializeField]
    private int _numOVertices = 0;

    private void OnEnable()
    {
        if (Vertices == null)
            Vertices = new List<Vertex>();
        if (Adjacency == null)
            Adjacency = new List<ListEdgeWrapper>();
    }

    #region Add Vertex/Edge
    public Vertex AddVertex()
    {
        var v = new Vertex
        {
            Id = NumOfVertices,
            Type = VertexType.NONE
        };
        Vertices.Add(v);
        Adjacency.Add(new ListEdgeWrapper());
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

        Adjacency[v1.Id].InnerList.RemoveAll(e => e.AdjacentV == v2);
        Adjacency[v2.Id].InnerList.RemoveAll(e => e.AdjacentV == v1);

        Adjacency[v1.Id].InnerList.Add(new Edge(v2, weight));
        Adjacency[v2.Id].InnerList.Add(new Edge(v1, weight));

        Adjacency[v1.Id].InnerList.Sort((x1, x2) => x2.AdjacentV.Id - x1.AdjacentV.Id);
        Adjacency[v2.Id].InnerList.Sort((x1, x2) => x2.AdjacentV.Id - x1.AdjacentV.Id);
    }
    #endregion

    /// <summary>
    /// Dijkstra's algorithm
    /// </summary>
    /// <param name="start">Source</param>
    /// <param name="end">Destination</param>
    /// <returns>Vertex path from start to end.</returns>
    public Vertex[] FindShortestPath(Vertex start, Vertex end)
    {
        HashSet<int> unvisited = new HashSet<int>();
        Dictionary<int, int> prevVertex = new Dictionary<int, int>();
        Dictionary<int, float> tentativeDistances = new Dictionary<int, float>();

        tentativeDistances.Add(start.Id, 0);
        prevVertex.Add(start.Id, -1);
        unvisited.Add(start.Id);

        foreach (var vertex in Vertices)
        {
            if (vertex.Equals(start))
                continue;
            tentativeDistances.Add(vertex.Id, Mathf.Infinity);
            prevVertex.Add(vertex.Id, -1);
            unvisited.Add(vertex.Id);
        }

        int minId = -1;
        while (unvisited.Count > 0)
        {
            minId = unvisited.Aggregate((curMin, v) => tentativeDistances[curMin] < tentativeDistances[v] ? curMin : v);
            unvisited.Remove(minId);


            if (minId.Equals(end.Id))
                break;

            foreach (var edge in Adjacency[minId].InnerList)
            {
                var adjV = edge.AdjacentV;
                if (!unvisited.Contains(adjV.Id))
                {
                    continue;
                }
                var dist = tentativeDistances[minId] + edge.Weight;
                if (dist < tentativeDistances[adjV.Id])
                {
                    tentativeDistances[adjV.Id] = dist;
                    prevVertex[adjV.Id] = minId;
                }
            }
        }

        if (prevVertex[minId] == -1 || minId.Equals(start.Id))
        {
            Debug.LogWarning("Couldn't find traversable path from end to start");
            return null;
        }
        var path = new Stack<Vertex>();
        while (minId != -1)
        {
            var minV = Vertices.Find(v => v.Id.Equals(minId));
            path.Push(minV);
            minId = prevVertex[minId];
        }

        return path.ToArray();
    }

    #region Deletion
    public void DeleteVertex(Vertex v)
    {
        Adjacency.ForEach(neighbours => neighbours.InnerList.RemoveAll(edge => edge.AdjacentV.Id.Equals(v.Id)));
        Vertices.Remove(v);
    }
    public void DeleteAllVertices()
    {
        NumOfVertices = 0;
        Vertices.Clear();
        Adjacency.Clear();
    }
    #endregion
}
