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

    private Vector3 _initialScale;
    private Vector3 _emphasizedScale;

    private AudioSource[] _soundEffects;

    private void Awake()
    {
        this._soundEffects = GetComponents<AudioSource>();
    }

    private void Start()
    {
        this._initialScale = this._currentValueLabel.rectTransform.localScale;
        this._emphasizedScale = this._initialScale * 1.3f;

        this.RedeemCardValue(500, 0);
    }

    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        this.currentValue += moneyValue;

        StopAllCoroutines();
        StartCoroutine(this.IncrementToCurrentValue(this.currentValue - moneyValue));

        this._soundEffects[0].Play();
        this._soundEffects[1].Play();
    }

    public void RemoveMoney(int moneySpent)
    {
        this.currentValue -= moneySpent;

        StopAllCoroutines();
        StartCoroutine(this.DecrementToCurrentValue(this.currentValue + moneySpent));

        this._soundEffects[0].Play();
        this._soundEffects[1].Play();
    }

    private IEnumerator IncrementToCurrentValue(int catchUpValue)
    {
        int amountToIncrement = 5;

        Vector3 originalScale = this._currentValueLabel.rectTransform.localScale;

        this._currentValueLabel.rectTransform.DOScale(this._emphasizedScale, 0.2f).SetEase(Ease.OutBack);

        while (catchUpValue < this.currentValue)
        {
            catchUpValue += amountToIncrement;
            this._currentValueLabel.text = "$" + catchUpValue.ToString();
            yield return new WaitForFixedUpdate();
        }

        this._currentValueLabel.rectTransform.DOScale(this._initialScale, 0.2f).SetEase(Ease.OutBack);

        this._soundEffects[1].Stop();
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

        this._soundEffects[1].Stop();
    }
}
