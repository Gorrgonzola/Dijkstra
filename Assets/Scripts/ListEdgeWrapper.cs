using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListEdgeWrapper
{
    public List<Edge> InnerList = new List<Edge>();

    public Edge this[int key]
    {
        get
        {
            return InnerList[key];
        }
        set
        {
            InnerList[key] = value;
        }
    }
}

