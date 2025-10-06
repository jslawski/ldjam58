using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    public int currentDay = 1;

    [SerializeField]
    private TextMeshProUGUI _dayTitleLabel;

    [SerializeField]
    private HappinessMeter _happinessMeter;

    [SerializeField]
    private GameObject _shopPrefab;

    [SerializeField]
    private TextMeshProUGUI _dayLabel;

    private int _minHealthTax = 50;
    private int _maxHealthTax = 150;

    private Vector3 _labelInitialScale;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }        

        this._labelInitialScale = this._dayTitleLabel.transform.localScale;
    }

    private int GetRandomHealthTax()
    {
        int randomPrice = Random.Range(this._minHealthTax, this._maxHealthTax + 1);

        int reduction = randomPrice % 10; //Ensures that all prices are multiples of 10

        return (randomPrice - reduction);
    }

    public void StartDayTransitionSequence()
    {
        this._dayTitleLabel.gameObject.SetActive(true);
        this._dayTitleLabel.text = "End of Day " + this.currentDay.ToString();
        this._happinessMeter.gameObject.SetActive(true);
        StartCoroutine(this.DayTransitionSequence());
    }

    private IEnumerator DayTransitionSequence()
    {
        Vector3 targetScale = this._labelInitialScale;
        Vector3 startingScale = this._labelInitialScale * 3.0f;

        this._dayTitleLabel.transform.localScale = startingScale;

        this._dayTitleLabel.transform.DOScale(targetScale, 0.2f);

        yield return new WaitForSeconds(1.0f);

        this._happinessMeter.DisplayMeter();

        yield return new WaitForSeconds(0.5f);

        int healthTax = this.GetRandomHealthTax();

        this._happinessMeter.RemoveHealth(healthTax);

        yield return new WaitForSeconds(2.0f);

        this._dayTitleLabel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        this._happinessMeter.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);

        yield return new WaitForSeconds(0.5f);

        Instantiate(this._shopPrefab);

        this._dayTitleLabel.gameObject.SetActive(false);
        this._happinessMeter.gameObject.SetActive(false);

        FadeManager.instance.FadeFromBlack();

        this.currentDay++;

        this._dayLabel.text = "Day " + this.currentDay.ToString();
    }
}
