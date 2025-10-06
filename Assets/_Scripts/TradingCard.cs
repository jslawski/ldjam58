using DG.Tweening;
using System.Collections;
using UnityEngine;
using TMPro;

public class TradingCard : MonoBehaviour
{
    private Transform _rootTransform;

    [SerializeField]
    private LayerMask _cardLayerMask;

    [SerializeField]
    private Renderer _cardRenderer;

    [SerializeField]
    private TextMeshProUGUI _moneyValueLabel;
    [SerializeField]
    private TextMeshProUGUI _happyValueLabel;

    [SerializeField]
    private ParticleSystem _rareParticles;
    [SerializeField]
    private ParticleSystem _ultraRareParticles;

    private float lookSpeed = 10.0f;
    private Vector3 _showcasePosition = new Vector3(0.0f, 0.0f, -7.25f);    
    private Vector3 _dismissPosition = new Vector3(-5.0f, 5f, 0.0f);   
    private Vector3 _dismissRotation = new Vector3(0.0f, 0.0f, 80.0f);
    private Vector2 _maxDistanceThresholds = new Vector2(0.1f, 0.3f);    
    private Vector2 _maxRotationThresholds = new Vector3(15.0f, 7.0f);

    private bool _isShowcasing = false;

    public TradingCardAttributes cardAttributes;

    private MouseLooker _mouseLooker;

    private AudioChannelSettings _channelSettings;

    public AudioClip cardLeave;
    public AudioClip cardZoom;

    private void Awake()
    {
        this._rootTransform = this.gameObject.transform;
        this._mouseLooker = GetComponent<MouseLooker>();

        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");
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
        AudioManager.instance.Play(this.cardZoom, this._channelSettings);

        this._rootTransform.DOMove(this._showcasePosition, 0.3f).SetEase(Ease.OutBack).SetLink(this.gameObject).OnComplete(this.PlayParticles());

        yield return new WaitForSeconds(0.3f);

        yield return null;

        this._isShowcasing = true;
    }

    private TweenCallback PlayParticles()
    {
        if (this.cardAttributes.rarity == Rarity.Rare)
        {
            this._rareParticles.Play();        
        }
        else if (this.cardAttributes.rarity == Rarity.UltraRare)
        {
             this._ultraRareParticles.Play();
        }

        return null;
    }

    private TweenCallback StopParticles()
    {
        this._rareParticles.Stop();
        this._ultraRareParticles.Stop();

        return null;
    }

    public void DismissCard()
    {
        this._rootTransform.DOKill();    

        this._rootTransform.DOLocalMove(this._dismissPosition, 0.3f).SetLink(this.gameObject).OnComplete(this.StopParticles());
        this._rootTransform.DOLocalRotate(this._dismissRotation, 0.3f).SetLink(this.gameObject);

        this._isShowcasing = false;

        this._mouseLooker.DisableMouseLook();

        AudioManager.instance.Play(this.cardLeave, this._channelSettings);
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
