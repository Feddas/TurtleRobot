using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Determines which image should be shown on the card
/// </summary>
public class Card : MonoBehaviour
{
    public UiPolygon Icon;
    public CardTypeEnum Type;
    public PolygonSet Polygons
    {
        get
        {
            return Icon.ScriptableObjectPolygons;
        }
        set
        {
            if (Icon.ScriptableObjectPolygons == value)
                return;

            Icon.ScriptableObjectPolygons = value;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
