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

    private AudioChannelSettings _channelSettings;

    private AudioChannelSettings _nonLoopSettings;
    private AudioChannelSettings _loopSettings;

    public AudioClip redeemSound;
    public AudioClip moneyLoop;

    public int loopId = 0;

    private void Start()
    {
        this._initialScale = this._currentValueLabel.rectTransform.localScale;
        this._emphasizedScale = this._initialScale * 1.3f;

        this._nonLoopSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");
        this._loopSettings = new AudioChannelSettings(true, 1.0f, 1.0f, 1.0f, "SFX");

        this.RedeemCardValue(500, 0);
    }

    public override void RedeemCardValue(int moneyValue, int happyValue)
    {
        this.currentValue += moneyValue;

        StopAllCoroutines();
        StartCoroutine(this.IncrementToCurrentValue(this.currentValue - moneyValue));

        AudioManager.instance.Stop(this.loopId);

        AudioManager.instance.Play(this.redeemSound, this._nonLoopSettings);
        this.loopId = AudioManager.instance.Play(this.moneyLoop, this._loopSettings);
    }

    public void RemoveMoney(int moneySpent)
    {
        this.currentValue -= moneySpent;

        StopAllCoroutines();
        StartCoroutine(this.DecrementToCurrentValue(this.currentValue + moneySpent));

        AudioManager.instance.Stop(this.loopId);

        AudioManager.instance.Play(this.redeemSound, this._nonLoopSettings);
        this.loopId = AudioManager.instance.Play(this.moneyLoop, this._loopSettings);
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

        AudioManager.instance.Stop(this.loopId);
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

        AudioManager.instance.Stop(this.loopId);
    }
}
