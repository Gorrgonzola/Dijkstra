
using UnityEngine;

public class Vertex
{ 
    public float TentativeDist { get; set; } = Mathf.Infinity;
    public int Id { get; set; }
    public VertexType Type { get; set; } = VertexType.NONE;


}