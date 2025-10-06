using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryCard : MonoBehaviour
{
    private Transform _rootTransform;
    private Vector3 _originalPosition = Vector3.zero;
    private Vector3 _showcasePosition = new Vector3(0.0f, 0.0f, -6.0f);

    private Vector3 _dragScale = new Vector3(1.2f, 1.2f, 1.2f);

    [SerializeField]
    private Renderer _cardRenderer;
    [SerializeField]
    private TextMeshProUGUI _moneyValueLabel;
    [SerializeField]
    private TextMeshProUGUI _happyValueLabel;

    [SerializeField]
    private LayerMask _colliderLayerMask;

    [SerializeField]
    private LayerMask _bucketLayerMask;

    private Ray _mouseRay;

    public TradingCardAttributes cardAttributes;

    private MouseLooker _mouseLooker;

    private RaycastHit _hitInfo;

    private bool _isDragging = false;

    public bool _isShowcasing = false;

    private float _minDragThreshold = 0.1f;

    private Coroutine _dragCoroutine = null;

    private float _minFollowSpeed = 10.0f;

    private RedeemBucket _highlightedRedeemBucket;

    public bool redeemed = false;

    private int _packNum = 0;
    private int _cardNum = 0;

    private void Awake()
    {
        this._mouseLooker = GetComponent<MouseLooker>();
        this._rootTransform = GetComponent<Transform>();        
    }

    void Update()
    {
        this._mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(this._mouseRay, out this._hitInfo, 100.0f, this._colliderLayerMask) == true)
        {
            if (this._hitInfo.collider.gameObject.name == this.gameObject.name)
            {            
                this._mouseLooker.EnableMouseLook();
            }
            else
            {
                this._mouseLooker.DisableMouseLook();
            }
        }
        else
        {
            this._mouseLooker.DisableMouseLook();
        }

        this.HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonUp(0) == true)
        {
            //Also check here if the card is over a bucket
            if (this._isShowcasing == true)
            {
                this.ReturnCard();
            }
            else if (this._isDragging)
            {
                if (this._highlightedRedeemBucket == null)
                {
                    this.ReturnCard();
                }
                else
                {
                    this.RedeemCard();
                }
            }
            else if (this.IsClickingCard() == true)
            {
                this.ShowcaseCard();
            }            
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (this._isShowcasing == false && this.IsClickingCard() == true && this._dragCoroutine == null)
            {
                this._dragCoroutine = StartCoroutine(this.HandleDrag());
            }
        }
    }

    private void ShowcaseCard()
    {
        this._originalPosition = this._rootTransform.position;
        this._rootTransform.DOMove(this._showcasePosition, 0.3f).SetEase(Ease.OutBack);
        this._isShowcasing = true;
    }

    private void ReturnCard()
    {
        this._rootTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).SetEase(Ease.OutBack);
        this._rootTransform.DOMove(this._originalPosition, 0.3f).SetEase(Ease.OutBack);
        this._isShowcasing = false;
        this._isDragging = false;
    }

    public void SetupCard(TradingCardAttributes cardAttributes, int packNum, int cardNum)
    {       
        this.gameObject.GetComponent<Collider>().enabled = true;
        this._highlightedRedeemBucket = null;

        this.cardAttributes = cardAttributes;

        this._cardRenderer.material = this.cardAttributes.cardMaterial;
        this._moneyValueLabel.text = this.cardAttributes.moneyValue.ToString();
        this._happyValueLabel.text = this.cardAttributes.happyValue.ToString();

        this._packNum = packNum;
        this._cardNum = cardNum;
    }

    public bool IsClickingCard()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, 100, this._colliderLayerMask) == true)
        {
            if (this._hitInfo.collider.gameObject.name == this.gameObject.name)
            {
                return true;
            }            
        }

        return false;
    }

    public IEnumerator HandleDrag()
    { 
        //Wait for a drag to reach the theshold before changing the _isDragging flag
        Vector3 initialViewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector3 currentViewportPosition = initialViewportPosition;

        while (Input.GetMouseButton(0) == true &&
            Vector3.Distance(initialViewportPosition, currentViewportPosition) < this._minDragThreshold)
        {
            yield return null;

            currentViewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0) == true)
        {
            this._originalPosition = this._rootTransform.position;

            this._isDragging = true;
            this._isShowcasing = false;

            this._rootTransform.DOScale(this._dragScale, 0.2f).SetEase(Ease.OutBack);
        }

        while (Input.GetMouseButton(0) == true)
        {
            Vector3 adjustedMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
            
            Vector3 currentMouseWorldPosition = Camera.main.ScreenToWorldPoint(adjustedMousePosition);
            Vector3 targetPosition = new Vector3(currentMouseWorldPosition.x, currentMouseWorldPosition.y, this._originalPosition.z - 0.5f);
            float currentDistance = Vector3.Distance(this._rootTransform.position, targetPosition);

            float moveSpeed = Mathf.Max(Mathf.Log(currentDistance, 10.0f), this._minFollowSpeed);

            this._rootTransform.position = Vector3.Lerp(this._rootTransform.position, targetPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }

        this._dragCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bucket")
        {        
            this._highlightedRedeemBucket = other.gameObject.GetComponent<RedeemBucket>();

            //Ignore redeem buckets that are completed bills
            BillBucket potentialBill = this._highlightedRedeemBucket.GetComponent<BillBucket>();
            if (potentialBill != null && potentialBill.currentStatus == BillStatus.Complete)
            {
                this._highlightedRedeemBucket = null;
                return;
            }

            this._highlightedRedeemBucket.EmphasizeBucket();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Bucket")
        {
            this._highlightedRedeemBucket = other.gameObject.GetComponent<RedeemBucket>();
            this._highlightedRedeemBucket.RevertBucket();
            this._highlightedRedeemBucket = null;
        }
    }

    private void RedeemCard()
    {    
        this.gameObject.GetComponent<Collider>().enabled = false;

        StartCoroutine(this.RedeemAnimation());

        this._highlightedRedeemBucket.RedeemCardValue(this.cardAttributes.moneyValue, this.cardAttributes.happyValue);
        this._highlightedRedeemBucket.RevertBucket();

        CollectionBucket potentialCollection = this._highlightedRedeemBucket.GetComponent<CollectionBucket>();
        if (potentialCollection != null)
        {
            CollectionManager.AddCardToCollection(this.cardAttributes);
        }

        this._highlightedRedeemBucket = null;

        CardPacksManager.instance.cardRedemptionStatus[this._packNum][this._cardNum] = true;
    }

    private IEnumerator RedeemAnimation()
    {
        this._rootTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        this._rootTransform.DOMove(this._highlightedRedeemBucket.gameObject.transform.position, 0.2f);

        yield return new WaitForSeconds(0.3f);

        this._rootTransform.position = this._originalPosition;

        CardSummaryManager.instance.IncrementRedeemedCardCount();

        if (CardSummaryManager.instance.AllCardsRedeemed() == true)
        {
            CardSummaryManager.instance.TriggerEndDay();
        }

        this.gameObject.SetActive(false);
    }
}
