using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HappinessManager : MonoBehaviour
{
    public static HappinessManager instance;

    [SerializeField]
    private RectTransform _barTransform;

    [SerializeField]
    private Image _fillImage;
    [SerializeField]
    private Image _transitionImage;
    [SerializeField]
    private TextMeshProUGUI _currentHealthLabel;

    private int _maxHealth = 1000;
    private int _currentHealth = 250;

    private float _timeToDecreaseHealth = 0.2f;

    private Vector3 _originalScale;
    private Vector3 _emphasizeScale;

    private Sequence addHealthSequence;
    private Sequence removeHealthSequence;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
        }

        CollectionManager.Setup();
    }

    // Start is called before the first frame update
    void Start()
    {
        this._originalScale = this._barTransform.localScale;
        this._emphasizeScale = this._originalScale * 1.1f;

        this._fillImage.fillAmount = this.GetCurrentFillAmount();
        this._transitionImage.fillAmount = this.GetCurrentFillAmount();
        this._currentHealthLabel.text = this._currentHealth.ToString() + " / " + this._maxHealth.ToString();
    }

    public void AddHealth(int healthToAdd)
    {
        this._currentHealth = this._currentHealth + healthToAdd;

        int catchUpValue = this._currentHealth - healthToAdd;

        if (this._currentHealth > this._maxHealth)
        {
            this._currentHealth = this._maxHealth;
        }

        float targetFillValue = this.GetCurrentFillAmount();
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
        this._currentHealth = this._currentHealth - healthToRemove;

        int catchUpValue = this._currentHealth + healthToRemove;

        if (this._currentHealth < 0)
        {
            this._currentHealth = 0;
        }

        float targetFillValue = this.GetCurrentFillAmount();
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

    private float GetCurrentFillAmount()
    {
        return (float)this._currentHealth / (float)this._maxHealth;
    }

    private IEnumerator IncrementToCurrentValue(int catchUpValue)
    {
        int amountToIncrement = 5;

        yield return new WaitForSeconds(0.2f);

        while (catchUpValue < this._currentHealth)
        {
            catchUpValue += amountToIncrement;
            this._currentHealthLabel.text = this._currentHealth.ToString() + " / " + this._maxHealth.ToString();
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DecrementToCurrentValue(int catchUpValue)
    {
        int amountToDecrement = 5;

        yield return new WaitForSeconds(0.2f);

        while (catchUpValue > this._currentHealth)
        {
            catchUpValue -= amountToDecrement;
            this._currentHealthLabel.text = this._currentHealth.ToString() + " / " + this._maxHealth.ToString();
            yield return new WaitForFixedUpdate();
        }
    }
}
