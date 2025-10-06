using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HappinessMeter : MonoBehaviour
{
    [SerializeField]
    private RectTransform _barTransform;

    [SerializeField]
    private Image _fillImage;
    [SerializeField]
    private Image _transitionImage;
    [SerializeField]
    private TextMeshProUGUI _currentHealthLabel;

    private float _timeToDecreaseHealth = 0.2f;

    private Vector3 _originalScale;
    private Vector3 _emphasizeScale;

    private Sequence addHealthSequence;
    private Sequence removeHealthSequence;

    private void Start()
    {
        this._originalScale = this._barTransform.localScale;
        this._emphasizeScale = this._originalScale * 1.1f;

        this._fillImage.fillAmount = HappinessManager.instance.GetCurrentFillAmount();
        this._transitionImage.fillAmount = HappinessManager.instance.GetCurrentFillAmount();
        this._currentHealthLabel.text = HappinessManager.instance._currentHealth.ToString() + " / " + HappinessManager.instance._maxHealth.ToString();

        this.transform.localScale = Vector3.zero;
    }

    public void DisplayMeter()
    {
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    public void HideMeter()
    {
        this.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
    }

    public void AddHealth(int healthToAdd)
    {
        HappinessManager.instance._currentHealth = HappinessManager.instance._currentHealth + healthToAdd;

        int catchUpValue = HappinessManager.instance._currentHealth - healthToAdd;

        if (HappinessManager.instance._currentHealth > HappinessManager.instance._maxHealth)
        {
            HappinessManager.instance._currentHealth = HappinessManager.instance._maxHealth;
        }

        float targetFillValue = HappinessManager.instance.GetCurrentFillAmount();
        float currentFillValue = this._fillImage.fillAmount;

        StartCoroutine(this.IncrementToCurrentValue(catchUpValue));

        this.addHealthSequence.Kill();
        this.addHealthSequence = DOTween.Sequence();

        Tweener growTween = this._barTransform.DOScale(this._emphasizeScale, 0.2f).SetEase(Ease.OutBack);
        Tweener returnTween = this._barTransform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);

        this.addHealthSequence.Append(growTween)
        .Insert(0.0f, DOTween.To(() => currentFillValue, x => currentFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._fillImage.fillAmount = currentFillValue; }))
        .Insert(0.0f, DOTween.To(() => currentFillValue, x => currentFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._transitionImage.fillAmount = currentFillValue; }))
        .Append(returnTween);

        this.addHealthSequence.Play();
    }

    public void RemoveHealth(int healthToRemove)
    {
        HappinessManager.instance._currentHealth = HappinessManager.instance._currentHealth - healthToRemove;

        int catchUpValue = HappinessManager.instance._currentHealth + healthToRemove;

        if (HappinessManager.instance._currentHealth < 0)
        {
            HappinessManager.instance._currentHealth = 0;
        }

        float targetFillValue = HappinessManager.instance.GetCurrentFillAmount();
        float currentFillValue = this._fillImage.fillAmount;
        float transitionFillValue = this._fillImage.fillAmount;

        StartCoroutine(this.DecrementToCurrentValue(catchUpValue));

        this.removeHealthSequence.Kill();
        this.removeHealthSequence = DOTween.Sequence();

        Tweener shakeTween = this._barTransform.DOShakePosition(this._timeToDecreaseHealth, 20f, 25, 90, false, false);
        Tweener growTween = this._barTransform.DOScale(this._emphasizeScale, 0.2f).SetEase(Ease.OutBack);
        Tweener returnTween = this._barTransform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);

        this.removeHealthSequence.Append(growTween)
        .Insert(0.0f, shakeTween)
        .Insert(0.0f, DOTween.To(() => currentFillValue, x => currentFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._fillImage.fillAmount = currentFillValue; }))
        .AppendInterval(0.2f)
        .Append(DOTween.To(() => transitionFillValue, x => transitionFillValue = x, targetFillValue, this._timeToDecreaseHealth).SetLink(this.gameObject).OnUpdate(() => { this._transitionImage.fillAmount = transitionFillValue; }))
        .Append(returnTween);

        this.removeHealthSequence.Play();
    }

    private IEnumerator IncrementToCurrentValue(int catchUpValue)
    {
        int amountToIncrement = 5;

        while (catchUpValue < HappinessManager.instance._currentHealth)
        {
            catchUpValue += amountToIncrement;
            this._currentHealthLabel.text = catchUpValue.ToString() + " / " + HappinessManager.instance._maxHealth.ToString();

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DecrementToCurrentValue(int catchUpValue)
    {
        int amountToDecrement = 5;

        while (catchUpValue > HappinessManager.instance._currentHealth)
        {
            catchUpValue -= amountToDecrement;
            this._currentHealthLabel.text = catchUpValue + " / " + HappinessManager.instance._maxHealth.ToString();
            yield return new WaitForFixedUpdate();
        }
    }
}
