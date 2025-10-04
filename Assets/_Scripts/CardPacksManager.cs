using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardPacksManager : MonoBehaviour 
{
    public static CardPacksManager instance;

    private Queue<Material> _purchasedPackWrappers;
    private List<List<TradingCardAttributes>> _packCards;    

    [SerializeField]
    private GameObject _cardPackPrefab;

    private Vector3 _packSpawnPosition = new Vector3(0.0f, -10.0f, 0.0f);

    public bool allPacksOpened = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        CardLibrary.Setup();
    
        this._purchasedPackWrappers = new Queue<Material>();
        this._packCards = new List<List<TradingCardAttributes>>();
    }

    private void Start()
    {
        Material[] allWrappers = Resources.LoadAll<Material>("PackWrappers");

        for (int i = 0; i < 6; i++)
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

        newPack.gameObject.transform.DOMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
        //newPack.gameObject.transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 0.2f);

        if (this._purchasedPackWrappers.Count <= 0)
        {
            this.allPacksOpened = true;
        }
    }    
}
