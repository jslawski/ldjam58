using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionBucket : RedeemBucket
{
    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        HappinessManager.instance.AddHealth(happyValue);        
    }
}
