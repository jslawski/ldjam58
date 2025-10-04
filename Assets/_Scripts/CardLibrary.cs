using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class CardLibrary
{
    public static TradingCardAttributes[] commonCards;
    public static TradingCardAttributes[] uncommonCards;
    public static TradingCardAttributes[] rareCards;
    public static TradingCardAttributes[] ultraRareCards;

    public static void Setup()
    {
        CardLibrary.commonCards = Resources.LoadAll<TradingCardAttributes>("CardAttributes/Common");
        CardLibrary.uncommonCards = Resources.LoadAll<TradingCardAttributes>("CardAttributes/Uncommon");
        CardLibrary.rareCards = Resources.LoadAll<TradingCardAttributes>("CardAttributes/Rare");
        CardLibrary.ultraRareCards = Resources.LoadAll<TradingCardAttributes>("CardAttributes/UltraRare");
    }

    public static TradingCardAttributes GetRandomCard(Rarity rarity)
    {
        int randomIndex = 0;
    
        switch (rarity)
        {
            case Rarity.Common:
                randomIndex = Random.Range(0, CardLibrary.commonCards.Length);
                return CardLibrary.commonCards[randomIndex];
            case Rarity.Uncommon:
                randomIndex = Random.Range(0, CardLibrary.uncommonCards.Length);
                return CardLibrary.uncommonCards[randomIndex];
            case Rarity.Rare:
                randomIndex = Random.Range(0, CardLibrary.rareCards.Length);
                return CardLibrary.rareCards[randomIndex];
            case Rarity.UltraRare:
                randomIndex = Random.Range(0, CardLibrary.ultraRareCards.Length);
                return CardLibrary.ultraRareCards[randomIndex];
            default:
                Debug.LogError("Unknown Rarity: " + rarity);
                return null;
        }
    }
}
