using System;

[Serializable]
public class Edge
{
    private Vertex _adjV;
    private int _weight;

    public Vertex AdjacentV => _adjV;
    public int Weight => _weight;

    public Edge(Vertex adjV, int weight)
    {
        _adjV = adjV;
        _weight = weight;
    }
}
