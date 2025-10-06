using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;

    [SerializeField]
    private TextMeshProUGUI _titleLabel;
    [SerializeField]
    private TextMeshProUGUI _financialValueLabel;
    [SerializeField]
    private TextMeshProUGUI _happinessCollectedLabel;

    //Game Over triggers when    
    //Your happiness reaches 0 at the end of the day

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void TriggerGameOver()
    { 
        
    }
}
