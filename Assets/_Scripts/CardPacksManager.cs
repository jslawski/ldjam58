using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardPacksManager : MonoBehaviour 
{
    public static CardPacksManager instance;

    public int totalPacks = 1;

    private Queue<Material> _purchasedPackWrappers;
    private List<List<TradingCardAttributes>> _packCards;
    public List<List<bool>> cardRedemptionStatus;

    [SerializeField]
    private GameObject _cardPackPrefab;

    private Vector3 _packSpawnPosition = new Vector3(0.0f, -10.0f, -5.0f);
    private Vector3 _packTargetPosition = new Vector3(0.0f, 0.0f, -5.0f);

    public bool allPacksOpened = false;
    
    [SerializeField]
    private GameObject _summaryManagerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        CardLibrary.Setup();
    
        this._purchasedPackWrappers = new Queue<Material>();
        this._packCards = new List<List<TradingCardAttributes>>();
        this.cardRedemptionStatus = new List<List<bool>>();
    }

    private void Start()
    {
        Material[] allWrappers = Resources.LoadAll<Material>("PackWrappers");

        for (int i = 0; i < this.totalPacks; i++)
        {
            int randomWrapperIndex = Random.Range(0, allWrappers.Length);
            this.PurchasePack(allWrappers[randomWrapperIndex]);
        }

        this.PrepareNextPack();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void PurchasePack(Material purchasePackMaterial)
    { 
        this._purchasedPackWrappers.Enqueue(purchasePackMaterial);
    }

    public void PrepareNextPack()
    {
        CardPack newPack = Instantiate(this._cardPackPrefab, this._packSpawnPosition, new Quaternion()).GetComponent<CardPack>();
        newPack.GeneratePack(this._purchasedPackWrappers.Dequeue());

        newPack.gameObject.transform.DOMove(this._packTargetPosition, 0.2f).SetEase(Ease.OutBack);

        if (this._purchasedPackWrappers.Count <= 0)
        {
            this.allPacksOpened = true;
        }
    }

    public void AddCardsToPackCards(List<TradingCardAttributes> cards)
    {
        this._packCards.Add(cards);
        
        List<bool> redemptionList = new List<bool>();
        for (int i = 0; i < cards.Count; i++) 
        {
            redemptionList.Add(false);
        }

        this.cardRedemptionStatus.Add(redemptionList);
    }

    public List<TradingCardAttributes> GetPackCards(int index)
    {
        return this._packCards[index];
    }

    public int GetNumPacksOpened()
    {
        return this._packCards.Count;
    }

    public void SetupSummaryManager()
    {
        Instantiate(this._summaryManagerPrefab);
    }
}
