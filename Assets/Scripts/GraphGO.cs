using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class GraphGO : MonoBehaviour
{
    #region Fields/Properties
    [SerializeField]
    private VertexGO _vertexGraphics = null;
    [SerializeField]
    private GraphSO _graph = null;

    public GraphSO Graph { get => _graph; set => _graph = value; }
    public Dictionary<Vertex, VertexGO> VerticesGO { get; set; } = new Dictionary<Vertex, VertexGO>();

    private LineRenderer _pathGraphics;
    private Vertex[] _path;
    #endregion

    private void OnEnable()
    {
        _pathGraphics = GetComponent<LineRenderer>();
        _pathGraphics.positionCount = 0;

        foreach (var vOldGO in GetComponentsInChildren<VertexGO>())
        {
            DestroyImmediate(vOldGO.gameObject);
        }

        DeleteAllVertices();
    }

    #region Add Edge/Vertex
    public void AddEdge(VertexGO v1, VertexGO v2, int weight)
    {
        if (Graph.Adjacency[v1.Vertex.Id].Any(e => e.Item1 == v2.Vertex))
        {
            Graph.AddEdge(v1.Vertex, v2.Vertex, weight);
            return;
        }

        Graph.AddEdge(v1.Vertex, v2.Vertex, weight);
        if (v1.Vertex.Id < v2.Vertex.Id)
        {
            v1.EdgeGraphics.positionCount += 2;
        }
        else if (v1.Vertex.Id > v2.Vertex.Id)
        {
            v2.EdgeGraphics.positionCount += 2;
        }
    }
    public void AddVertex()
    {
        var vGo = Instantiate(_vertexGraphics, transform.position, Quaternion.identity, transform);
        vGo.Vertex = Graph.AddVertex();
        vGo.name = vGo.Vertex.Id.ToString();
        VerticesGO.Add(vGo.Vertex, vGo);
    }
    #endregion

    #region Drawing
    private void OnDrawGizmos()
    {
        DrawEdges();
        if (_path != null)
        {
            DrawShortestPath();
        }
    }

    private void DrawShortestPath()
    {
        _pathGraphics.positionCount = _path.Length;
        _pathGraphics.startColor = Color.blue;
        _pathGraphics.endColor = Color.red;
        int i = 0;
        while (i < _path.Length)
        {
            var vertex = _path[i];
            _pathGraphics.SetPosition(i, VerticesGO[vertex].transform.position);
            i++;
        }
    }

    public void SetShortestPath(Stack<Vertex> path)
    {
        _path = path.ToArray();
    }

    /// <summary>
    /// Draws(very badly) lines using LineRenderer(LR). Every even point in a LR is a transform.position
    /// </summary>
    public void DrawEdges()
    {
        if (Graph == null || Graph.Adjacency.All(neighbours => neighbours.Count <= 0))
            return;
        foreach (var v in Graph.Vertices)
        {
            var vGO = VerticesGO[v];
            int lineCount = 0;
            foreach (var edge in Graph.Adjacency[v.Id])
            {
                if (lineCount >= vGO.EdgeGraphics.positionCount)
                    break;
                var vNeighbourGO = VerticesGO[edge.Item1];

                var neighbourPos = vNeighbourGO.transform.position;
                vGO.EdgeGraphics.SetPosition(lineCount, neighbourPos);
                vGO.EdgeGraphics.SetPosition(lineCount + 1, vGO.transform.position);
                lineCount += 2;
            }
        }
    }
    #endregion

    public void DeleteAllVertices()
    {
        foreach (var kv in VerticesGO)
        {
            var vGO = kv.Value;
            if (vGO != null)
                DestroyImmediate(vGO.gameObject);
        }
        VerticesGO.Clear();

        Graph.DeleteAllVertices();
    }

    public Stack<Vertex> FindShortestPath(VertexGO start, VertexGO end)
    {
        return Graph.FindShortestPath(start.Vertex, end.Vertex);
    }
}
