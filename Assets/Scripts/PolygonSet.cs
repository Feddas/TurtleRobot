using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu()]
public class PolygonSet : ScriptableObject
{
    public List<Polygon> Polygons;
}

[System.Serializable]
public class Polygon
{
    public string Name;

    public bool MirrorHorizontally;

    public bool IsHidden;

    [Tooltip("Vertices are percent based, use 0 to 1")]
    public List<Vector2> Vertices;
}