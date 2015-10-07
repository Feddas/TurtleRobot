using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stores how many cards of each type are in a deck/container
/// </summary>
public class CardStorage : MonoBehaviour
{
    public Card CardTemplate;
    public List<CardTypeEnum> Cards;

    void Start()
    {
        foreach (var cardType in Cards)
        {
            var nextCard = Instantiate(CardTemplate);
            nextCard.name = cardType.ToString();
            nextCard.transform.SetParent(this.transform);

            nextCard.Type = cardType;
        }
    }

    void Update()
    {

    }
}
