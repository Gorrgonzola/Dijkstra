
using System;
using UnityEngine;

[Serializable]
public class Vertex
{
    [HideInInspector]
    [SerializeField]
    private float tentativeDist = Mathf.Infinity;
    [SerializeField]
    private int id;
    [HideInInspector]
    [SerializeField]
    private VertexType type = VertexType.NONE;

    public float TentativeDist { get => tentativeDist; set => tentativeDist = value; }
    public int Id { get => id; set => id = value; }
    public VertexType Type { get => type; set => type = value; }

}