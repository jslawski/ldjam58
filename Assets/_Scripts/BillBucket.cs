using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public enum BillStatus { Active, Complete, Failed }

public class BillBucket : RedeemBucket
{
    public BillAttributes billAttributes;

    public int daysRemaining;

    public BillStatus currentStatus;

    [SerializeField]
    private SpriteRenderer _billImage;
    [SerializeField]
    private SpriteRenderer _completedStamp;
    [SerializeField]
    private TextMeshProUGUI _billTitle;
    [SerializeField]
    private TextMeshProUGUI _targetValueLabel;
    [SerializeField]
    private TextMeshProUGUI _currentValueLabel;
    [SerializeField]
    private TextMeshProUGUI _daysRemainingLabel;
    [SerializeField]
    private TextMeshProUGUI _punishmentValueLabel;


    private Vector3 _initialScale;
    private Vector3 _emphasizedScale;

    public void Setup(BillAttributes billAttributes)
    {
        this.billAttributes = billAttributes;
        this.currentValue = 0;
        this.daysRemaining = billAttributes.daysToComplete;
        this.currentStatus = BillStatus.Active;

        this._billImage.sprite = this.billAttributes.billImage;
        this._billTitle.text = this.billAttributes.title;
        this._targetValueLabel.text = "$" + this.billAttributes.targetAmount.ToString();
        this._currentValueLabel.text = "$" + this.currentValue.ToString();
        this._daysRemainingLabel.text = this.daysRemaining.ToString() + " Days Remaining...";
        this._punishmentValueLabel.text = "Punishment: -" + this.billAttributes.punishmentAmount.ToString() + " :(";

        this._initialScale = this._currentValueLabel.rectTransform.localScale;
        this._emphasizedScale = this._initialScale * 1.5f;
    }

    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        this.currentValue += moneyValue;

        StopAllCoroutines();
        StartCoroutine(this.IncrementToCurrentValue(this.currentValue - moneyValue));

        if (this.currentValue >= this.billAttributes.targetAmount)
        {
            this.currentStatus = BillStatus.Complete;
        }
    }

    private IEnumerator IncrementToCurrentValue(int catchUpValue)
    {        
        int amountToIncrement = 5;        

        this._currentValueLabel.rectTransform.DOScale(this._emphasizedScale, 0.2f).SetEase(Ease.OutBack);

        while (catchUpValue < this.currentValue)
        {
            catchUpValue += amountToIncrement;
            this._currentValueLabel.text = "$" + catchUpValue.ToString();
            yield return new WaitForFixedUpdate();
        }

        this._currentValueLabel.rectTransform.DOScale(this._initialScale, 0.2f).SetEase(Ease.OutBack);

        if (this.currentStatus == BillStatus.Complete)
        {
            this._completedStamp.gameObject.SetActive(true);
            this._completedStamp.gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }

    public void DecrementBillDays()
    {
        this.daysRemaining -= 1;
        this._daysRemainingLabel.text = this.daysRemaining.ToString() + " Days Remaining...";

        if (this.daysRemaining <= 0)
        {
            this.currentStatus = BillStatus.Failed;
        }
    }
}
