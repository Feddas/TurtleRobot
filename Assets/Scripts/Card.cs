using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Determines which image should be shown on the card
/// </summary>
public class Card : MonoBehaviour
{
    public Image Arrow;
    public CardTypeEnum Type;

    // Use this for initialization
    void Start()
    {
        switch (Type)
        {
            case CardTypeEnum.Forward:
                break;
            case CardTypeEnum.TurnLeft:
                Arrow.rectTransform.Rotate(new Vector3(0, 0, 90));
                break;
            case CardTypeEnum.TurnRight:
                Arrow.rectTransform.Rotate(new Vector3(0, 0, -90));
                break;
            case CardTypeEnum.Function:
                Arrow.rectTransform.Rotate(new Vector3(0, 0, 180)); // TODO: replace this with a better "function" icon
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
