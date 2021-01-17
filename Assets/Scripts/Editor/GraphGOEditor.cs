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
        GraphGO graph = (GraphGO)serializedObject.targetObject;

        if (GUILayout.Button("Create Vertex"))
        {
            graph.AddVertex();
        }
        if (GUILayout.Button("Delete All Vertices"))
        {
            graph.DeleteAllVertices();
        }

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
            var path = graph.FindShortestPath(_start, _end);

            if (path != null)
            {
                graph.SetShortestPath(path);
            }
        }
    }
}
