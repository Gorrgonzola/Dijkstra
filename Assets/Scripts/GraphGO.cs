using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GraphGO : MonoBehaviour
{
    #region Fields/Properties
    [Range(0, 10f)]
    [SerializeField]
    private float _spawnBoxBounds = 8f;
    [SerializeField]
    private VertexGO _vertexGraphics = null;
    [SerializeField]
    private LineRenderer _pathGraphics = null;
    [Space(20)]
    [SerializeField]
    private GraphSO _graphToLoad = null;

    public GraphSO Graph { get => _graph; private set => _graph = value; }
    public Dictionary<int, VertexGO> VerticesGO { get; set; } = new Dictionary<int, VertexGO>();

    private Vertex[] _path;
    [HideInInspector]
    [SerializeField]
    private GraphSO _graph;
    #endregion

    #region Graph Asset Handling
    private void OnEnable()
    {
        UnloadGraph();
    }
    public void CreateGraph()
    {
        if (Graph != null)
            UnloadGraph();
        GraphSO asset = ScriptableObject.CreateInstance<GraphSO>();
        AssetDatabase.CreateAsset(asset, $"Assets/ScriptableObjects/Graph{asset.GetInstanceID()}.asset");
        AssetDatabase.SaveAssets();

        Graph = asset;
    }

    public void LoadGraph()
    {
        if (_graphToLoad == null)
        {
            Debug.LogError("No graph asset provided.");
            return;
        }

        UnloadGraph();
        Graph = _graphToLoad;
        foreach (var v in Graph.Vertices)
        {
            var randOffset = new Vector3(Random.Range(-_spawnBoxBounds, _spawnBoxBounds), Random.Range(-_spawnBoxBounds, _spawnBoxBounds), Random.Range(-_spawnBoxBounds, _spawnBoxBounds));
            var vGO = Instantiate(_vertexGraphics, transform.position + randOffset, Quaternion.identity, transform) as VertexGO;
            vGO.Vertex = v;
            vGO.name = v.Id.ToString();
            v.Type = VertexType.NONE;
            vGO.SetColorByType();
            VerticesGO.Add(v.Id, vGO);
            vGO.EdgeGraphics.positionCount = Mathf.Max(1, Graph.Adjacency[v.Id].InnerList.Count - v.Id) * 2;
        }
    }
    private void UnloadGraph()
    {
        foreach (var vGO in GetComponentsInChildren<VertexGO>())
        {
            DestroyImmediate(vGO.gameObject);
        }
        VerticesGO.Clear();
        _pathGraphics.positionCount = 0;
        _path = null;
        Graph = null;
    }
    #endregion

    #region Add Edge/Vertex
    public void AddEdge(VertexGO v1, VertexGO v2, int weight)
    {
        if (Graph.Adjacency[v1.Vertex.Id].InnerList.Any(e => e.AdjacentV == v2.Vertex))
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

        EditorUtility.SetDirty(Graph);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void AddVertex()
    {
        if (Graph == null)
        {
            Debug.LogError("Graph is null. Create or load one first.");
            return;
        }
        var randOffset = new Vector3(Random.Range(-_spawnBoxBounds, _spawnBoxBounds), Random.Range(-_spawnBoxBounds, _spawnBoxBounds), Random.Range(-_spawnBoxBounds, _spawnBoxBounds));
        var vGo = Instantiate(_vertexGraphics, transform.position + randOffset, Quaternion.identity, transform);
        vGo.Vertex = Graph.AddVertex();
        vGo.name = vGo.Vertex.Id.ToString();
        VerticesGO.Add(vGo.Vertex.Id, vGo);

        EditorUtility.SetDirty(Graph);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
            _pathGraphics.SetPosition(i, VerticesGO[vertex.Id].transform.position);
            i++;
        }
    }

    public void SetShortestPath(Stack<Vertex> path)
    {
        _path = path.ToArray();
    }

    /// <summary>
    /// Draws(badly) lines using LineRenderer(LR). Every even point in a LR is a transform.position
    /// </summary>
    public void DrawEdges()
    {
        
        if (Graph == null || Graph.Adjacency.All(neighbours => neighbours.InnerList?.Count <= 0))
            return;
        foreach (var v in Graph.Vertices)
        {
            var vGO = VerticesGO[v.Id];
            int lineCount = 0;
            foreach (var edge in Graph.Adjacency[v.Id].InnerList)
            {
                if (lineCount >= vGO.EdgeGraphics.positionCount)
                    break;
                var vNeighbourGO = VerticesGO[edge.AdjacentV.Id];

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
        if (Graph == null)
            return;
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
