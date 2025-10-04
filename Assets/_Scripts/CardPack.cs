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

    [SerializeField]
    private TradingCard _showcasingCard;

    private TradingCard[] _cardComponents;

    private List<TradingCardAttributes> _cardAttributes;

    [SerializeField]
    private Renderer wrapperRenderer;

    private Collider _collider;

    [SerializeField]
    private LayerMask _packLayerMask;

    private float _uncommonPullChance = 0.30f;
    private float _rarePullChance = 0.15f;
    private float _ultraRarePullChance = 0.05f;    

    private void Awake()
    {
        this._collider = GetComponent<Collider>();
                
        GetComponent<MouseLooker>().EnableMouseLook();
       
        this._cards = new Queue<TradingCard>();

        this._cardComponents = GetComponentsInChildren<TradingCard>();

        this._cardAttributes = new List<TradingCardAttributes>();

        for (int i = 0; i < this._cardComponents.Length; i++)
        { 
            this._cards.Enqueue(this._cardComponents[i]);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) == true)
        {
            if (this.IsClickingPack())
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
        this.UnwrapPack();

        yield return new WaitForSeconds(0.2f);

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
            if (CardPacksManager.instance.allPacksOpened == false)
            {
                CardPacksManager.instance.PrepareNextPack();
            }
            else
            { 
                //All Packs Opened trigger here
            }

            Destroy(this.gameObject);
        }
    }

    public void GeneratePack(Material packWrapper)
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
            this._cardComponents[i].gameObject.SetActive(false);
        }

        this.SetWrapperMaterial(packWrapper);
    }

    private TradingCardAttributes GenerateCard()
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
            if (this._cardAttributes[i].cardMaterial == currentCard.cardMaterial)
            {
                return true;
            }
        }

        return false;
    }

    public void SetWrapperMaterial(Material wrapperRenderer)
    {
        this.wrapperRenderer.material = wrapperRenderer;
    }
    public bool IsClickingPack()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, 100, this._packLayerMask) == true)
        {
            return true;
        }

        return false;
    }

    public void UnwrapPack()
    {
        for (int i = 0; i < this._cardComponents.Length; i++) 
        {
            this._cardComponents[i].gameObject.SetActive(true);
        }
    
        this.wrapperRenderer.enabled = false;
        this._collider.enabled = false;
    }

    private void ShowCards()
    {
        for (int i = 0; i < this._cardComponents.Length; i++)
        {
            this._cardComponents[i].gameObject.SetActive(true);
        }
    }

    private void HideCards()
    {
        for (int i = 0; i < this._cardComponents.Length; i++)
        {
            this._cardComponents[i].gameObject.SetActive(false);
        }
    }
}
