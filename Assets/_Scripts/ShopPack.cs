using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopPack : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer _packRenderer;

    [SerializeField]
    private LayerMask _colliderLayerMask;

    [SerializeField]
    private LayerMask _bucketLayerMask;

    [SerializeField]
    private TextMeshProUGUI _priceLabel;

    private Material[] _allPackMaterials;

    private Transform _rootTransform;
    private Vector3 _originalPosition = Vector3.zero;

    private Ray _mouseRay;
    private MouseLooker _mouseLooker;

    private Vector3 _dragScale = new Vector3(2.5f, 2.5f, 2.5f);

    private RaycastHit _hitInfo;

    private bool _isDragging = false;

    private float _minDragThreshold = 0.1f;

    private Coroutine _dragCoroutine = null;

    private float _minFollowSpeed = 10.0f;

    private RedeemBucket _highlightedRedeemBucket;

    private int _minPackPrice = 100;

    private int _maxPackPrice = 200;

    public int packPrice = 100;

    private WalletBucket _playerWallet;

    private Vector3 _originalScale;

    private void Awake()
    {
        this._allPackMaterials = Resources.LoadAll<Material>("PackWrappers");
        this._mouseLooker = GetComponent<MouseLooker>();
        this._rootTransform = GetComponent<Transform>();

        this._playerWallet = GameObject.Find("WalletBucket").GetComponent<WalletBucket>();

        this._originalScale = this._rootTransform.localScale;
    }

    private void Start()
    {
        int randomIndex = Random.Range(0, this._allPackMaterials.Length);
        this._packRenderer.material = this._allPackMaterials[randomIndex];

        this.packPrice = this.GetRandomPackPrice();

        this._priceLabel.text = "$" + this.packPrice.ToString();
    }

    private int GetRandomPackPrice()
    { 
        int randomPrice = Random.Range(this._minPackPrice, this._maxPackPrice + 1);

        int reduction = randomPrice % 10; //Ensures that all prices are multiples of 10

        return (randomPrice - reduction);
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
            if (this._isDragging)
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
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (this.IsClickingCard() == true && this._dragCoroutine == null)
            {
                this._dragCoroutine = StartCoroutine(this.HandleDrag());
            }
        }
    }

    private void RedeemCard()
    {
        this.gameObject.GetComponent<Collider>().enabled = false;

        StartCoroutine(this.RedeemAnimation());
        
        this._highlightedRedeemBucket.RevertBucket();

        this._highlightedRedeemBucket = null;

        ShopManager.instance.PurchasePack(this._packRenderer.material);
        this._playerWallet.RemoveMoney(this.packPrice);
    }

    private void ReturnCard()
    {
        this._rootTransform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);
        this._rootTransform.DOMove(this._originalPosition, 0.3f).SetEase(Ease.OutBack);
        this._isDragging = false;
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

    private IEnumerator RedeemAnimation()
    {
        this._rootTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        this._rootTransform.DOMove(this._highlightedRedeemBucket.gameObject.transform.position, 0.2f);

        yield return new WaitForSeconds(0.3f);

        this._rootTransform.position = this._originalPosition;

        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cart")
        {
            this._highlightedRedeemBucket = other.gameObject.GetComponent<RedeemBucket>();

            if (this._playerWallet.currentValue < this.packPrice)
            {
                this._highlightedRedeemBucket = null;
                return;
            }

            this._highlightedRedeemBucket.EmphasizeBucket();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Cart")
        {
            this._highlightedRedeemBucket = other.gameObject.GetComponent<RedeemBucket>();
            this._highlightedRedeemBucket.RevertBucket();
            this._highlightedRedeemBucket = null;
        }
    }
}
