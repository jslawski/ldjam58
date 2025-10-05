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
    private TextMeshProUGUI _billTitle;
    [SerializeField]
    private TextMeshProUGUI _targetValueLabel;
    [SerializeField]
    private TextMeshProUGUI _currentValueLabel;
    [SerializeField]
    private TextMeshProUGUI _daysRemainingLabel;

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
        this._daysRemainingLabel.text = this.daysRemaining.ToString() + " Remaining...";
    }

    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        this.currentValue += moneyValue;

        StopAllCoroutines();
        StartCoroutine(this.IncrementToCurrentValue());

        if (this.currentValue >= this.billAttributes.targetAmount)
        {
            this.currentStatus = BillStatus.Complete;
        }
    }

    private IEnumerator IncrementToCurrentValue()
    {
        int labelValue = Int32.Parse(this._currentValueLabel.text);
        int amountToIncrement = 5;

        Vector3 originalScale = this._currentValueLabel.rectTransform.localScale;

        this._currentValueLabel.rectTransform.DOScale(this._currentValueLabel.rectTransform.localScale * 1.2f, 0.2f).SetEase(Ease.OutBack);

        while (labelValue < this.currentValue)
        {
            labelValue += amountToIncrement;
            this._currentValueLabel.text = labelValue.ToString();
            yield return new WaitForFixedUpdate();
        }

        this._currentValueLabel.rectTransform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    public void DecrementBillDays()
    {
        this.daysRemaining -= 1;
        this._daysRemainingLabel.text = this.daysRemaining.ToString() + " Remaining...";

        if (this.daysRemaining <= 0)
        {
            this.currentStatus = BillStatus.Failed;
        }
    }
}
