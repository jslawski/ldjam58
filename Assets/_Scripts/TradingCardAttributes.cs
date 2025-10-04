using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TradingCardAttributes", menuName = "ScriptableObjects/TradingCardAttributes", order = 1)]

public class TradingCardAttributes : ScriptableObject
{
    public Material cardMaterial;
    public int moneyValue;
    public int happyValue;
}
