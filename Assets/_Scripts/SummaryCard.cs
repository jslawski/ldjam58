using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryCard : MonoBehaviour
{
    [SerializeField]
    private Renderer _cardRenderer;
    [SerializeField]
    private TextMeshProUGUI _moneyValueLabel;
    [SerializeField]
    private TextMeshProUGUI _happyValueLabel;

    [SerializeField]
    private LayerMask _colliderLayerMask;

    private Ray _mouseRay;

    public TradingCardAttributes cardAttributes;

    private MouseLooker _mouseLooker;

    private RaycastHit _hitInfo;

    private void Awake()
    {
        this._mouseLooker = GetComponent<MouseLooker>();
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
    }

    public void SetupCard(TradingCardAttributes cardAttributes)
    {
        this.cardAttributes = cardAttributes;

        this._cardRenderer.material = this.cardAttributes.cardMaterial;
        this._moneyValueLabel.text = this.cardAttributes.moneyValue.ToString();
        this._happyValueLabel.text = this.cardAttributes.happyValue.ToString();
    }
}
