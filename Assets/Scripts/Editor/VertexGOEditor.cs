using UnityEngine;
using UnityEditor;
using System.Text;

[CustomEditor(typeof(VertexGO))]
public class VertexGOEditor : Editor
{
    private VertexGO _selectedVertexGO;
    private int _weight;

    void OnSceneGUI()
    {
        VertexGO vertex = (VertexGO)serializedObject.targetObject;
        GraphGO graph = vertex.transform.parent.GetComponent<GraphGO>();
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
        _selectedVertexGO = (VertexGO)EditorGUILayout.ObjectField("Neighbour", _selectedVertexGO, typeof(VertexGO), allowSceneObjects: true);
        _weight = EditorGUILayout.IntField("Weight", _weight);
        if (GUILayout.Button("Create"))
        {
            graph.AddEdge(vertex, _selectedVertexGO, _weight);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            foreach (var edge in graph.Graph.Adjacency[vertex.Vertex.Id])
            {
                sb.Append($"Vertex {edge.AdjacentV.Id} with edge weight {edge.Weight}\n");
            }
            Debug.Log(sb.ToString(), vertex.gameObject);
        }

        GUILayout.EndArea();

        Handles.EndGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }


}
