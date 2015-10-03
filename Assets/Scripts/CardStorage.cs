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
        foreach (var card in Cards)
        {
            var nextCard = Instantiate(CardTemplate);
            nextCard.name = card.ToString();
            nextCard.transform.parent = this.transform;
        }
    }

    void Update()
    {

    }
}
