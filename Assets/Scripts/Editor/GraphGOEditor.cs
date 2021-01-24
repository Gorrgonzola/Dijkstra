using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

[CustomEditor(typeof(GraphGO))]
public class GraphGOEditor : Editor
{
    private VertexGO _start, _end;

    protected virtual void OnSceneGUI()
    {
        GraphGO graph = (GraphGO)serializedObject.targetObject;

        if (graph == null)
        {
            return;
        }

        Handles.color = Color.white;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        var pos = graph.transform.position + Vector3.up * 1.5f;
        Handles.Label(pos,
            graph.name,
            style
        );
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Graph Editing", EditorStyles.boldLabel);

        GraphGO graph = (GraphGO)serializedObject.targetObject;
        var graphSO = graph.Graph;
        
        if (GUILayout.Button("Create Graph Asset"))
        {
            graph.CreateGraph();
        }
        if (GUILayout.Button("Load Graph Asset"))
        {
            graph.LoadGraph();
        }
        if (graphSO == null)
            return;
        if (GUILayout.Button("Create Vertex"))
        {
            graph.AddVertex();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Delete All Vertices"))
        {
            graph.DeleteAllVertices();
        }

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Dijkstra's algorithm", EditorStyles.boldLabel);
        _start = (VertexGO)EditorGUILayout.ObjectField("Start", _start, typeof(VertexGO), allowSceneObjects: true);
        _end = (VertexGO)EditorGUILayout.ObjectField("End", _end, typeof(VertexGO), allowSceneObjects: true);
        if (_start != null && _start.Vertex != null)
        {
            _start.Vertex.Type = VertexType.START;
            _start.SetColorByType();
        }
        if (_end != null && _end.Vertex != null)
        {
            _end.Vertex.Type = VertexType.END;
            _end.SetColorByType();
        }


        if (GUILayout.Button("Find Shortest Path"))
        {
            graph.FindShortestPath(_start, _end);
        }
    }
}
