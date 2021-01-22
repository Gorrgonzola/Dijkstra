using System;
using UnityEngine;

[Serializable]
public class Edge
{
    [SerializeField]
    private Vertex _adjV;
    [SerializeField]
    private int _weight;

    public Vertex AdjacentV => _adjV;
    public int Weight => _weight;

    public Edge(Vertex adjV, int weight)
    {
        _adjV = adjV;
        _weight = weight;
    }
}
