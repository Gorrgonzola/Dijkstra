using System.Collections.Generic;
using UnityEngine;

public class VertexGO : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _edgeGraphics = null;
    [SerializeField]
    private Material[] _materials = null; //TODO serializable dictionary

    public LineRenderer EdgeGraphics { get => _edgeGraphics;}

    public Vertex Vertex { get; set; }

    public void SetColorByType()
    {
        GetComponent<MeshRenderer>().sharedMaterial = _materials[(int)Vertex.Type];
    }
}
