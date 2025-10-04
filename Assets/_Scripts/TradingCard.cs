using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TradingCard : MonoBehaviour
{
    private Transform _rootTransform;

    [SerializeField]
    private Transform _cardTransform;

    [SerializeField]
    private LayerMask _cardLayerMask;

    [SerializeField]
    private Renderer _cardRenderer;

    [SerializeField]
    private TextMeshProUGUI _moneyValueLabel;
    [SerializeField]
    private TextMeshProUGUI _happyValueLabel;

    private float lookSpeed = 10.0f;
    private Vector3 _showcasePosition = new Vector3(0.0f, 0.0f, -5.0f);    
    private Vector3 _dismissPosition = new Vector3(-10.0f, 8.0f, -1.0f);   
    private Vector3 _dismissRotation = new Vector3(0.0f, 0.0f, 80.0f);
    private Vector2 _maxDistanceThresholds = new Vector2(0.1f, 0.3f);    
    private Vector2 _maxRotationThresholds = new Vector3(15.0f, 7.0f);

    private bool _isShowcasing = false;
    
    private Ray _mouseRay;
    private RaycastHit _hitInfo;

    private Tween _resetTween;

    

    public TradingCardAttributes cardAttributes;

    private void Awake()
    {
        this._rootTransform = this.gameObject.transform;
    }

    public void SetupCard(TradingCardAttributes cardAttributes)
    {
        this.cardAttributes = cardAttributes;
    
        this._cardRenderer.material = this.cardAttributes.cardMaterial;
        this._moneyValueLabel.text = this.cardAttributes.moneyValue.ToString();
        this._happyValueLabel.text = this.cardAttributes.happyValue.ToString();
    }

    

    public void ShowcaseCard()
    {
        StartCoroutine(this.SetupShowcase());
    }

    private IEnumerator SetupShowcase()
    {
        this._rootTransform.DOMove(this._showcasePosition, 0.3f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.3f);

        yield return null;

        this._isShowcasing = true;
    }

    public void DismissCard()
    {
        this._rootTransform.DOKill();    

        this._rootTransform.DOLocalMove(this._dismissPosition, 0.6f);
        this._rootTransform.DOLocalRotate(this._dismissRotation, 0.6f);

        this._isShowcasing = false;
    }

    private void Update()
    {
        if (this._isShowcasing == true)
        {
            this._mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(this._mouseRay, out this._hitInfo, 100.0f, this._cardLayerMask) == true)
            {
                this.LookAtMouse();
            }
            else if (this._cardTransform.localRotation != Quaternion.identity && this._resetTween == null)
            {
                this.ResetCardRotation();                               
            }

            if (this._resetTween != null && this._resetTween.IsActive() == false)
            {
                this._resetTween = null;
            }
        }
    }

    private void LookAtMouse()
    {
        this._cardTransform.DOKill();
    
        Vector3 viewportCardPosition = Camera.main.WorldToViewportPoint(this._cardTransform.position);
        Vector3 viewportPointPosition = Camera.main.WorldToViewportPoint(this._hitInfo.point);

        float xDistance = Mathf.Clamp(viewportPointPosition.x - viewportCardPosition.x, -this._maxDistanceThresholds.x, this._maxDistanceThresholds.x);
        float yDistance = Mathf.Clamp(viewportPointPosition.y - viewportCardPosition.y, -this._maxDistanceThresholds.y, this._maxDistanceThresholds.y);

        Vector2 rotationValues = this.GetRotationValues(xDistance, yDistance);

        this._cardTransform.localRotation = Quaternion.Lerp(this._cardTransform.localRotation, Quaternion.Euler(rotationValues.x, rotationValues.y, 0.0f), this.lookSpeed * Time.deltaTime);
    }

    private Vector2 GetRotationValues(float xDistance, float yDistance)
    {
        Vector2 rotationValues = Vector2.zero;

        //Yes this is correct.  The rotation axis and the distance coordinate are two different things
        rotationValues.x = (yDistance / this._maxDistanceThresholds.y) * this._maxRotationThresholds.y;
        rotationValues.y = -(xDistance / this._maxDistanceThresholds.x) * this._maxRotationThresholds.x;        

        return rotationValues;
    }

    public void ResetCardRotation()
    {
        this._resetTween = this._cardTransform.DORotate(Vector3.zero, 0.2f);
    }
}
