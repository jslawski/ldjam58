using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionManager
{
    public static List<TradingCardAttributes> collectedCards;    

    public static void Setup()
    {
        CollectionManager.collectedCards = new List<TradingCardAttributes>();
    }

    public static void AddCardToCollection(TradingCardAttributes cardAttributes)
    { 
        CollectionManager.collectedCards.Add(cardAttributes);
    }

    public static int GetCollectionTotalMoneyValue()
    {
        int totalValue = 0;

        for (int i = 0; i < CollectionManager.collectedCards.Count; i++) 
        {
            totalValue += CollectionManager.collectedCards[i].moneyValue;    
        }

        return totalValue;
    }

    public static int GetCollectionTotalHappyValue()
    {
        int totalValue = 0;

        for (int i = 0; i < CollectionManager.collectedCards.Count; i++)
        {
            totalValue += CollectionManager.collectedCards[i].happyValue;
        }

        return totalValue;
    }
}
