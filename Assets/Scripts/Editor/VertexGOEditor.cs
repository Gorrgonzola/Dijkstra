using UnityEngine;
using UnityEditor;
using System;
using System.Text;

[CustomEditor(typeof(VertexGO))]
public class VertexGOEditor : Editor
{
    VertexGO selectedVertexGO;
    int weight;

    void OnSceneGUI()
    {
        VertexGO vertex = (VertexGO)serializedObject.targetObject;
        GraphGO graph = vertex.transform.parent?.GetComponent<GraphGO>();
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));


        Handles.BeginGUI();

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        var pos = vertex.transform.position + Vector3.up * 1.5f;
        Handles.Label(pos,
            vertex.name,
            style
        );

        GUIStyle boxStyle = new GUIStyle("box");

        GUILayout.BeginArea(new Rect(10, 10, 200, 100), boxStyle);

        GUILayout.Label("Add edge");
        selectedVertexGO = (VertexGO)EditorGUILayout.ObjectField("Neighbour", selectedVertexGO, typeof(VertexGO), allowSceneObjects: true);
        weight = EditorGUILayout.IntField("Weight", weight);
        if (GUILayout.Button("Create"))
        {
            graph.AddEdge(vertex, selectedVertexGO, weight);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            foreach (var edge in graph.Graph.Adjacency[vertex.Vertex.Id])
            {
                sb.Append($"Vertex {edge.Item1.Id} with edge weight {edge.Item2}\n");
            }
            Debug.Log(sb.ToString());
        }

        /*var graphSO = new SerializedObject(graph.graph);
        var edges = graphSO.FindProperty("Adjacency");
        if (edges.arraySize > 0)
        {
            EditorGUILayout.PropertyField(edges, new GUIContent("List of edges"), true);
        }*/

        GUILayout.EndArea();

        Handles.EndGUI();
        /*Vector3 mousePos = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        mousePos = ray.origin;
        Handles.DrawLine(vertex.transform.position, mousePos);*/

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }


}
