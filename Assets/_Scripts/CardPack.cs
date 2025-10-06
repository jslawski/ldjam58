using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum Rarity { Common, Uncommon, Rare, UltraRare }

public class CardPack : MonoBehaviour
{
    private Queue<TradingCard> _cards;

    private Transform _rootTransform;

    [SerializeField]
    private Transform _packTransform;

    [SerializeField]
    private TradingCard _showcasingCard;

    private TradingCard[] _cardComponents;

    private List<TradingCardAttributes> _cardAttributes;

    [SerializeField]
    private SkinnedMeshRenderer packRenderer;

    private Collider _collider;

    [SerializeField]
    private LayerMask _packLayerMask;

    [SerializeField]
    private GameObject _particleObject;
    [SerializeField]
    private ParticleSystem _burstParticleSystem;

    private float _uncommonPullChance = 0.30f;
    private float _rarePullChance = 0.15f;
    private float _ultraRarePullChance = 0.05f;

    private float _timeToOpenPack = 0.3f;

    private void Awake()
    {
        this._rootTransform = this.gameObject.transform;
        this._collider = GetComponent<Collider>();
                
        GetComponent<MouseLooker>().EnableMouseLook();
       
        this._cards = new Queue<TradingCard>();

        this._cardComponents = GetComponentsInChildren<TradingCard>();

        this._cardAttributes = new List<TradingCardAttributes>();

        for (int i = 0; i < this._cardComponents.Length; i++)
        { 
            this._cards.Enqueue(this._cardComponents[i]);
        }

        DOTween.Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) == true)
        {
            if (this.IsClickingPack())
            {
                StartCoroutine(this.OpenPackSequence());
            }

            if (this._showcasingCard != null)
            {
                StartCoroutine(this.DisplayNextCard());
            }
        }
    }

    private void PlayTweenSequence()
    {
        Tweener shakeTween = this._rootTransform.DOShakePosition(0.75f, 0.15f, 25, 90, false, false).SetLink(this.gameObject);
        Tweener shrinkTween = this._rootTransform.DOScale(0.75f, 0.75f).SetLink(this.gameObject);

        float blendShapeWeight = 0.0f;
        Tweener growTween = this._rootTransform.DOScale(1.0f, this._timeToOpenPack).SetEase(Ease.OutBack, 20.0f).SetLink(this.gameObject);

        float dissolveAmount = 0.0f;

        Sequence openSequence = DOTween.Sequence();

        openSequence.Append(shakeTween)
        .Insert(0.0f, shrinkTween)
        .Append(growTween)
        .Insert(0.75f, DOTween.To(() => blendShapeWeight, x => blendShapeWeight = x, 100, this._timeToOpenPack).SetLink(this.gameObject).OnUpdate(() => { this.packRenderer.SetBlendShapeWeight(0, blendShapeWeight); }))
        .AppendInterval(0.2f)
        .Append(DOTween.To(() => dissolveAmount, x => dissolveAmount = x, 1.0f, 0.5f).SetLink(this.gameObject).OnUpdate(() => { this.packRenderer.material.SetFloat("_Dissolve_Amount", dissolveAmount); })).SetLink(this.gameObject);

        openSequence.Play();
    }

    private IEnumerator OpenPackSequence()
    {
        GetComponent<MouseLooker>().DisableMouseLook();
        this._collider.enabled = false;

        this.PlayTweenSequence();

        yield return new WaitForSeconds(0.75f);

        this.ActivateParticles();
        this.ActivateCards();

        yield return new WaitForSeconds(0.1f);

        this._burstParticleSystem.Play();

        yield return new WaitForSeconds(1.0f);

        this.ShowcaseFirstCard();
    }

    private void ActivateCards()
    {
        for (int i = 0; i < this._cardComponents.Length; i++)
        {
            this._cardComponents[i].gameObject.SetActive(true);
        }
    }

    private void ActivateParticles()
    {
        this._particleObject.SetActive(true);
    }

    private void ShowcaseFirstCard()
    {
        this._showcasingCard = this._cards.Dequeue();
        this._showcasingCard.ShowcaseCard();

        this._burstParticleSystem.Play();
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

            this._burstParticleSystem.Stop();
            this._burstParticleSystem.Play();
        }
        else
        {
            if (CardPacksManager.instance.allPacksOpened == false)
            {
                CardPacksManager.instance.PrepareNextPack();
            }
            else
            {
                CardPacksManager.instance.SetupSummaryManager();
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

        //this.SetWrapperMaterial(packWrapper);

        CardPacksManager.instance.AddCardsToPackCards(this._cardAttributes);
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
            if (this._cardAttributes[i] == currentCard)
            {
                return true;
            }
        }

        return false;
    }

    public void SetWrapperMaterial(Material wrapperRenderer)
    {
        //this.packRenderer.material = wrapperRenderer;
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

        this.OpenTopOfPack();
                
        this._collider.enabled = false;
    }

    private void OpenTopOfPack()
    {
       
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
