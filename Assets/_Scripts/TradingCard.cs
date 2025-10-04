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

    public TradingCardAttributes cardAttributes;

    private MouseLooker _mouseLooker;

    private void Awake()
    {
        this._rootTransform = this.gameObject.transform;
        this._mouseLooker = GetComponent<MouseLooker>();
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
            this._mouseLooker.EnableMouseLook();
        }
        else
        {
            this._mouseLooker.DisableMouseLook();
        }
    }
}
