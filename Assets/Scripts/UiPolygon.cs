using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Vertex
{
    public Vector2 Position;
    public Vector2 Uv;

    public float x
    {
        get { return Position.x; }
    }

    public float y
    {
        get { return Position.y; }
    }

    public Vertex(float x, float y, Vector2 uv)
    {
        Position = new Vector2(x, y);
        Uv = uv;
    }
}

/// <summary>
/// Draws concave polygons on a Canvas that obeys the anchor points of the RectTransform
/// </summary>
public class UiPolygon : Graphic
{
    [Tooltip("Warning: after set, modifying Polygons will also modify the source ScriptableObject")]
    public PolygonSet ScriptableObjectPolygons;

    [Tooltip("Polygons must be concave/regular. Otherwise triangles will be drawn outside of the polygon.")]
    public List<Polygon> Polygons;

    /// <summary> vertices grouped by the polygon they belong to </summary>
    private IEnumerable<List<Vector2>> allVertices;

    protected override void OnPopulateMesh(Mesh m)
    {
        if (isPolygonMissing())
            return;

        using (var vh = new VertexHelper())
        {
            // add vertices
            foreach (var vertex in mapVertices())
            {
                vh.AddVert(new Vector3(vertex.x, vertex.y), color, vertex.Uv);
            }

            // add triangles
            int polygonVerticeStart = 0; // the index where the current polygon has its first vertex
            foreach (var polygon in allVertices)
            {
                polygonVerticeStart = addTriangles(vh, polygon, polygonVerticeStart);
            }

            // draw to mesh
            vh.FillMesh(m);
        }
    }

    /// <summary> Polygon data is required to have something to populate the mesh with </summary>
    private bool isPolygonMissing()
    {
        if (ScriptableObjectPolygons != null)
            Polygons = ScriptableObjectPolygons.Polygons;
            //ScriptableObjectPolygons.Polygons = Polygons; //use this line to copy inspector data to a ScriptableObject

        return (Polygons == null || Polygons.Count == 0);
    }

    /// <summary> Maps vertices from percent values into the width and height of the rectTransform </summary>
    private List<Vertex> mapVertices()
    {
        List<Vertex> result = new List<Vertex>();
        float pivotX = rectTransform.pivot.x;
        float pivotY = rectTransform.pivot.y;

        allVertices = Polygons.Select(p => p.Vertices);
        foreach (var polygon in allVertices)
        {
            // restrict to valid values
            for (int index = 0; index < polygon.Count; index++)
            {
                // restrict to inside bounds of rectTransform
                float xMapped = polygon[index].x;
                float yMapped = polygon[index].y;

                // account for pivot
                xMapped -= pivotX;
                yMapped -= pivotY;

                // map to pixels show on screen
                xMapped *= rectTransform.rect.width;
                yMapped *= rectTransform.rect.height;

                result.Add(new Vertex(xMapped, yMapped, polygon[index]));
            }
        }

        return result;
    }

    /// <param name="startsAtIndex">index where the passed in polygon has its first vertex out of VertexHelper.verts</param>
    /// <returns>index where the next polygon has its first vertex</returns>
    private int addTriangles(VertexHelper vh, List<Vector2> polygon, int startsAtIndex)
    {
        int i;

        // connect vertices in a peacock fashion where startsAtIndex is the root of all the peacock feathers: http://stackoverflow.com/questions/13369452/given-an-irregular-polygons-vertex-list-how-to-create-internal-triangles-to-bu
        for (i = 2; i < polygon.Count; i++)
        {
            vh.AddTriangle(startsAtIndex, startsAtIndex + i - 1, startsAtIndex + i);
        }
        return startsAtIndex + i;
    }
}
