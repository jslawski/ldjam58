using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class CardSummaryManager : MonoBehaviour
{
    public static CardSummaryManager instance;

    private SummaryCard[] _summaryCards;
    private int _currentPage = 0;

    [SerializeField]
    private TextMeshProUGUI _pageLabel;
    [SerializeField]
    private Button _prevButton;
    [SerializeField]
    private Button _nextButton;

    private Vector3 _initialScale = new Vector3(1.5f, 1.5f, 1.5f);

    private int _totalRedeemedCards = 0;

    private void Awake()
    {
        this._summaryCards = GetComponentsInChildren<SummaryCard>();

        instance = this;        
    }

    // Start is called before the first frame update
    void Start()
    {
        this.DisplayCurrentPack();

        this._prevButton.interactable = false;        
        this._nextButton.interactable = false;

        if (CardPacksManager.instance.GetNumPacksOpened() > 1)
        {
            this._nextButton.interactable = true;
        }

        if (CardPacksManager.instance.GetNumPacksOpened() <= 0)
        {
            this.TriggerEndDay();
        }
    }

    private void DisplayCurrentPack()
    {
        if (CardPacksManager.instance.GetNumPacksOpened() <= 0)
        {
            for (int i = 0; i < this._summaryCards.Length; i++)
            {
                this._summaryCards[i].gameObject.SetActive(false);
            }

            return;
        }
        List<TradingCardAttributes> packCards = CardPacksManager.instance.GetPackCards(this._currentPage);

        for (int i = 0; i < this._summaryCards.Length; i++)
        {
            if (CardPacksManager.instance.cardRedemptionStatus[this._currentPage][i] == true)
            {
                this._summaryCards[i].gameObject.SetActive(false);
            }
            else
            {
                this._summaryCards[i].gameObject.SetActive(true);
            }
        
            this._summaryCards[i].SetupCard(packCards[i], this._currentPage, i);
            this._summaryCards[i].transform.position = new Vector3(this._summaryCards[i].transform.position.x, this._summaryCards[i].transform.position.y, -1.0f);
            this._summaryCards[i].transform.localScale = this._initialScale;
            this._summaryCards[i].DOKill();
            this._summaryCards[i].transform.DOMoveZ(0.0f, 0.3f).SetEase(Ease.OutBack);
        }

        this.UpdatePageLabel();
    }

    public void GoToNextPack()
    {    
        this._currentPage++;

        this.DisplayCurrentPack();

        if (this._currentPage >= (CardPacksManager.instance.GetNumPacksOpened() - 1))
        {
            this._nextButton.interactable = false;
        }
        else
        {
            this._nextButton.interactable = true;
        }

        this._prevButton.interactable = true;
    }

    public void GoToPreviousPack()
    {
        this._currentPage--;

        this.DisplayCurrentPack();

        if (this._currentPage <= 0)
        {
            this._prevButton.interactable = false;
        }
        else
        {
            this._prevButton.interactable = true;
        }

        this._nextButton.interactable = true;
    }

    private void UpdatePageLabel()
    {
        this._pageLabel.text = (this._currentPage + 1) + " / " + CardPacksManager.instance.GetNumPacksOpened().ToString();
    }

    public void IncrementRedeemedCardCount()
    {
        this._totalRedeemedCards++;
    }

    public bool AllCardsRedeemed()
    {     
        return (this._totalRedeemedCards >= (CardPacksManager.instance.GetNumPacksOpened() * this._summaryCards.Length));
    }

    public void TriggerEndDay()
    {
        Debug.LogError("ALL CARDS REDEEMED, END OF DAY");

        MusicManager.instance.FadeToEndOfDayMusic();

        StartCoroutine(this.StartEndOfDaySequence());
    }

    private IEnumerator StartEndOfDaySequence()
    {
        CardPacksManager.instance.endOfDayObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        CardPacksManager.instance.endOfDayObject.SetActive(false);

        BillManager.instance.TriggerExpiredBills();

        Destroy(this.gameObject);
    }
}
