using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CardIcon
{
    public string Name;
    public CardTypeEnum CardType;
    public PolygonSet PolygonData;
}

/// <summary>
/// Stores how many cards of each type are in a deck/container
/// </summary>
public class CardStorage : MonoBehaviour
{
    public Card CardTemplate;
    public List<CardIcon> Icons;
    public List<CardTypeEnum> Cards;

    void Start()
    {
        foreach (var cardType in Cards)
        {
            var nextCard = Instantiate(CardTemplate);
            nextCard.name = cardType.ToString();
            nextCard.transform.SetParent(this.transform);

            nextCard.Type = cardType;

            // TODO: find a better way to map card types to icon data
            nextCard.Polygons = Icons.Where(i => i.CardType == cardType)
                .Select(i => i.PolygonData)
                .FirstOrDefault();
        }
    }

    void Update()
    {

    }
}
