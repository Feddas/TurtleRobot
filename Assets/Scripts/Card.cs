using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Determines which image should be shown on the card
/// </summary>
public class Card : MonoBehaviour
{
    public UiDiamond Arrow;
    public CardTypeEnum Type;

    // Use this for initialization
    void Start()
    {
        switch (Type)
        {
            case CardTypeEnum.Forward:
                Arrow.yMin = 0;
                break;
            case CardTypeEnum.TurnLeft:
                Arrow.xMax = 0;
                break;
            case CardTypeEnum.TurnRight:
                Arrow.xMin = 0;
                break;
            case CardTypeEnum.Function:
                Arrow.SetBounds(.5f, .5f, .5f, .5f);  // TODO: replace this with a better "function" icon
                break;
            case CardTypeEnum.Error:
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
