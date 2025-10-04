using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class CardSummaryManager : MonoBehaviour
{
    private SummaryCard[] _summaryCards;
    private int _currentPage = 0;

    [SerializeField]
    private TextMeshProUGUI _pageLabel;
    [SerializeField]
    private Button _prevButton;
    [SerializeField]
    private Button _nextButton;

    private void Awake()
    {
        this._summaryCards = GetComponentsInChildren<SummaryCard>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.DisplayCurrentPack();

        this._prevButton.interactable = false;

        if (CardPacksManager.instance.GetNumPacksOpened() > 1)
        {
            this._nextButton.interactable = true;
        }
    }

    private void DisplayCurrentPack()
    {
        List<TradingCardAttributes> packCards = CardPacksManager.instance.GetPackCards(this._currentPage);

        for (int i = 0; i < this._summaryCards.Length; i++)
        {
            this._summaryCards[i].SetupCard(packCards[i]);
            this._summaryCards[i].transform.position = new Vector3(this._summaryCards[i].transform.position.x, this._summaryCards[i].transform.position.y, -1.0f);
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
        this._pageLabel.text = "Pack #" + (this._currentPage + 1);
    }
}
