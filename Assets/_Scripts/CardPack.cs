using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CardPack : MonoBehaviour
{
    private Queue<TradingCard> _cards;

    private PackWrapper _packWrapper;

    [SerializeField]
    private TradingCard _showcasingCard;    

    private void Awake()
    {
        CardLibrary.Setup();
    
        this._packWrapper = GetComponentInChildren<PackWrapper>();
        this._cards = new Queue<TradingCard>();

        TradingCard[] cardComponents = GetComponentsInChildren<TradingCard>();
        
        for (int i = 0; i < cardComponents.Length; i++)
        { 
            this._cards.Enqueue(cardComponents[i]);
        }
    }

    private void Start()
    {
        this.GeneratePack();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            if (this._packWrapper.IsClickingPack())
            {
                StartCoroutine(this.OpenPack());
            }

            if (this._showcasingCard != null)
            {
                StartCoroutine(this.DisplayNextCard());
            }
        }
    }

    private IEnumerator OpenPack()
    {
        this._packWrapper.OpenCardPack();

        yield return new WaitForSeconds(0.5f);

        this._showcasingCard = this._cards.Dequeue();
        this._showcasingCard.ShowcaseCard();
    }

    private IEnumerator DisplayNextCard()
    {
        this._showcasingCard.DismissCard();

        //yield return new WaitForSeconds(0.5f);

        yield return null;

        if (this._cards.Count > 0)
        {
            this._showcasingCard = this._cards.Dequeue();
            this._showcasingCard.ShowcaseCard();
        }
        else
        { 
            //Do "end of pack" stuff
        }
    }

    private void GeneratePack()
    {    
        TradingCard[] cardComponents = GetComponentsInChildren<TradingCard>();
        for (int i = 0; i < cardComponents.Length; i++)
        {
            cardComponents[i].GenerateCard();           
        }
    }
}
