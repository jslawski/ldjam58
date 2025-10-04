using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum Rarity { Common, Uncommon, Rare, UltraRare }

public class CardPack : MonoBehaviour
{
    private Queue<TradingCard> _cards;

    private PackWrapper _packWrapper;

    [SerializeField]
    private TradingCard _showcasingCard;

    private TradingCard[] _cardComponents;

    private List<TradingCardAttributes> _cardAttributes;

    private float _uncommonPullChance = 0.30f;
    private float _rarePullChance = 0.15f;
    private float _ultraRarePullChance = 0.05f;

    private void Awake()
    {
        CardLibrary.Setup();
    
        this._packWrapper = GetComponentInChildren<PackWrapper>();
        this._cards = new Queue<TradingCard>();

        this._cardComponents = GetComponentsInChildren<TradingCard>();

        this._cardAttributes = new List<TradingCardAttributes>();

        for (int i = 0; i < this._cardComponents.Length; i++)
        { 
            this._cards.Enqueue(this._cardComponents[i]);
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
        TradingCardAttributes newCard;
    
        for (int i = 0; i < this._cardComponents.Length; i++)
        {
            newCard = this.GenerateCard();

            //Regenerate card if it is a duplicate
            while (this.IsDuplicateCard(newCard, i) == true)
            {
                newCard = this.RerollDuplicate(newCard.rarity);
            }

            this._cardAttributes.Add(newCard);
        }

        this._cardAttributes = this._cardAttributes.OrderBy(card => (int)card.rarity).ToList();

        for (int i = 0; i < this._cardComponents.Length; i++)
        {
            this._cardComponents[i].SetupCard(this._cardAttributes[i]);
        }
    }

    public TradingCardAttributes GenerateCard()
    {
        Rarity cardRarity = this.GenerateRarity();

        return CardLibrary.GetRandomCard(cardRarity);
    }

    public TradingCardAttributes RerollDuplicate(Rarity rarity)
    {
        return CardLibrary.GetRandomCard(rarity);
    }

    private Rarity GenerateRarity()
    {
        float randomRoll = Random.Range(0.0f, 1.0f);

        if (randomRoll <= this._ultraRarePullChance)
        {
            return Rarity.UltraRare;
        }
        else if (randomRoll <= this._rarePullChance)
        {
            return Rarity.Rare;
        }
        else if (randomRoll <= this._uncommonPullChance)
        {
            return Rarity.Uncommon;
        }
        else
        {
            return Rarity.Common;
        }
    }

    private bool IsDuplicateCard(TradingCardAttributes currentCard, int currentIndex)
    {
        for (int i = 0; i < currentIndex; i++)
        {
            if (this._cardComponents[i].cardAttributes == currentCard)
            {
                return true;
            }
        }

        return false;
    }
}
