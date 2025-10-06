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
    private SpriteRenderer _completedStamp;
    [SerializeField]
    private SpriteRenderer _failedStamp;
    [SerializeField]
    private TextMeshProUGUI _billTitle;
    [SerializeField]
    private TextMeshProUGUI _currentValueLabel;
    [SerializeField]
    private TextMeshProUGUI _daysRemainingLabel;
    [SerializeField]
    private TextMeshProUGUI _punishmentValueLabel;


    private Vector3 _initialScale;
    private Vector3 _emphasizedScale;

    public override void DisplayBucket()
    {
        this.bucketTransform.localScale = Vector3.zero;
        this.bucketTransform.DOScale(Vector3.one * 9.0f, 0.2f).SetEase(Ease.OutBack);
    }

    public void Setup(BillAttributes billAttributes)
    {
        this.billAttributes = billAttributes;
        this.currentValue = 0;
        this.daysRemaining = billAttributes.daysToComplete;
        this.currentStatus = BillStatus.Active;
        this.currentValue = this.billAttributes.targetAmount;

        this._billTitle.text = this.billAttributes.title;
        this._currentValueLabel.text = "$" + this.billAttributes.targetAmount.ToString();
        this._daysRemainingLabel.text = this.daysRemaining.ToString();
        this._punishmentValueLabel.text = "-" + this.billAttributes.punishmentAmount.ToString();

        this._initialScale = this._currentValueLabel.rectTransform.localScale;
        this._emphasizedScale = this._initialScale * 1.3f;

        this._completedStamp.gameObject.SetActive(false);
        this._completedStamp.gameObject.transform.localScale = Vector3.one * 0.75f;

        this._failedStamp.gameObject.SetActive(false);
        this._failedStamp.gameObject.transform.localScale = Vector3.one * 0.75f;
    }

    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        this.currentValue = this.currentValue - moneyValue;

        int catchUpValue = this.currentValue + moneyValue;

        if (this.currentValue < 0)
        {
            this.currentValue = 0;
        }

        StopAllCoroutines();
        StartCoroutine(this.DecrementToCurrentValue(catchUpValue));

        if (this.currentValue <= 0)
        {
            this.currentStatus = BillStatus.Complete;
        }
    }

    private IEnumerator DecrementToCurrentValue(int catchUpValue)
    {        
        int amountToDecrement = 5;        

        this._currentValueLabel.rectTransform.DOScale(this._emphasizedScale, 0.2f).SetEase(Ease.OutBack);

        while (catchUpValue > this.currentValue)
        {
            catchUpValue -= amountToDecrement;
            this._currentValueLabel.text = "$" + catchUpValue.ToString();
            yield return new WaitForFixedUpdate();
        }

        this._currentValueLabel.rectTransform.DOScale(this._initialScale, 0.2f).SetEase(Ease.OutBack);

        if (this.currentStatus == BillStatus.Complete)
        {
            this._completedStamp.gameObject.SetActive(true);
            this._completedStamp.gameObject.transform.DOScale(Vector3.one * 0.35f, 0.2f).SetEase(Ease.OutBack);
        }
    }

    public void DecrementBillDays()
    {
        this.daysRemaining -= 1;
        this._daysRemainingLabel.text = this.daysRemaining.ToString();

        if (this.daysRemaining <= 0 && this.currentStatus == BillStatus.Active)
        {
            this.currentStatus = BillStatus.Failed;
        }
    }

    public void TriggerFailedBill()
    {
        StartCoroutine(this.FailedBillSequence());
    }

    private IEnumerator FailedBillSequence()
    {
        this.EmphasizeBucket();

        yield return new WaitForSeconds(0.2f);

        this._failedStamp.gameObject.SetActive(true);
        this._failedStamp.gameObject.transform.DOScale(Vector3.one * 0.35f, 0.2f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.5f);

        HappinessManager.instance.RemoveHealth(this.billAttributes.punishmentAmount);

        yield return new WaitForSeconds(0.5f);

        this.RevertBucket();
    }
}
