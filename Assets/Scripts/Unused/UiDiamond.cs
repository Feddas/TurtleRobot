using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// creates a diamond the fills a percent of a RectTransform
/// based off of diamond from http://forum.unity3d.com/threads/onfillvbo-to-onpopulatemesh-help.353977/
/// </summary>
[ExecuteInEditMode]
public class UiDiamond : Graphic
{
    [Range(0, 1)]
    public float xMin;

    [Range(0, 1)]
    public float xMax;

    [Range(0, 1)]
    public float yMin;

    [Range(0, 1)]
    public float yMax;

    public void SetBounds(float newXMin, float newXMax, float newYMin, float newYMax)
    {
        xMin = newXMin;
        xMax = newXMax;
        yMin = newYMin;
        yMax = newYMax;
    }

    protected override void OnPopulateMesh(Mesh m)
    {
        // restrict to valid values
        xMin = Mathf.Clamp(xMin, 0, 1);
        yMax = Mathf.Clamp(yMax, 0, 1);
        xMax = Mathf.Clamp(xMax, 0, 1);
        yMin = Mathf.Clamp(yMin, 0, 1);

        // map percent values to the pixel size of the recTransform
        float wHalf = rectTransform.rect.width / 2;
        float hHalf = rectTransform.rect.height / 2;
        var xMinMapped = -wHalf * xMin;
        var yMaxMapped = hHalf * yMax;
        var xMaxMapped = wHalf * xMax;
        var yMinMapped = -hHalf * yMin;

        // define what point the diamond will be centered around
        var center = new Vector2((xMaxMapped + xMinMapped) / 2, (yMaxMapped + yMinMapped) / 2);

        using (var vh = new VertexHelper())
        {
            // place diamond vertices
            add(vh, xMinMapped, 0, center, new Vector2(0f, 0f));
            add(vh, 0, yMaxMapped, center, new Vector2(0f, 1f));
            add(vh, xMaxMapped, 0, center, new Vector2(1f, 1f));
            add(vh, 0, yMinMapped, center, new Vector2(1f, 0f));

            // connect vertices
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            // draw to mesh
            vh.FillMesh(m);
        }
    }

    private void add(VertexHelper vh, float x, float y, Vector2 center, Vector2 uv0)
    {
        // account for center while adding the vertex
        vh.AddVert(new Vector3(x - center.x, y - center.y), color, uv0);
    }
}
