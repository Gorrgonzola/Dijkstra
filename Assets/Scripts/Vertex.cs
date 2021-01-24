
using System;
using UnityEngine;

[Serializable]
public class Vertex
{
    [SerializeField]
    private int id;
    [HideInInspector]
    [SerializeField]
    private VertexType type = VertexType.NONE;

    public int Id { get => id; set => id = value; }
    public VertexType Type { get => type; set => type = value; }

}