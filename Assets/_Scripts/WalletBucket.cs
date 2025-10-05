using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WalletBucket : RedeemBucket
{
    [SerializeField]
    private TextMeshProUGUI _currentValueLabel;

    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        this.currentValue += moneyValue;

        StopAllCoroutines();
        StartCoroutine(this.IncrementToCurrentValue());
    }

    public void RemoveMoney(int moneySpent)
    {
        this.currentValue -= moneySpent;

        StopAllCoroutines();
        StartCoroutine(this.DecrementToCurrentValue());
    }

    private IEnumerator IncrementToCurrentValue()
    {
        int labelValue = Int32.Parse(this._currentValueLabel.text);
        int amountToIncrement = 5;

        Vector3 originalScale = this._currentValueLabel.rectTransform.localScale;

        this._currentValueLabel.rectTransform.DOScale(this._currentValueLabel.rectTransform.localScale * 1.5f, 0.2f).SetEase(Ease.OutBack);

        while (labelValue < this.currentValue)
        {
            labelValue += amountToIncrement;
            this._currentValueLabel.text = labelValue.ToString();
            yield return new WaitForFixedUpdate();
        }

        this._currentValueLabel.rectTransform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    private IEnumerator DecrementToCurrentValue()
    {
        int labelValue = Int32.Parse(this._currentValueLabel.text);
        int amountToDecrement = 5;

        Vector3 originalScale = this._currentValueLabel.rectTransform.localScale;

        this._currentValueLabel.rectTransform.DOScale(this._currentValueLabel.rectTransform.localScale * 1.5f, 0.2f).SetEase(Ease.OutBack);

        while (labelValue > this.currentValue)
        {
            labelValue -= amountToDecrement;
            this._currentValueLabel.text = labelValue.ToString();
            yield return new WaitForFixedUpdate();
        }

        this._currentValueLabel.rectTransform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
}
