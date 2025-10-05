using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BillAttributes", menuName = "ScriptableObjects/BillAttributes", order = 1)]

public class BillAttributes : ScriptableObject
{
    public string title;
    public Sprite billImage;
    public int targetAmount;
    public int daysToComplete;
    public int punishmentAmount;
}
