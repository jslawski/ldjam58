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

    private int _easyMinHealthTax = 180;
    private int _easyMaxHealthTax = 280;

    private int _medMinHealthTax = 250;
    private int _medMaxHealthTax = 350;

    private int _hardMinHealthTax = 300;
    private int _hardMaxHealthTax = 350;

    private Vector3 _labelInitialScale;

    private AudioChannelSettings _channelSettings;

    public AudioClip thudSound;
    public AudioClip zoomSound;
    public AudioClip damageSound;
    public AudioClip welcome;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }        

        this._labelInitialScale = this._dayTitleLabel.transform.localScale;

        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");

    }

    private int GetRandomHealthTax()
    {
        int randomPrice = 0;

        if (this.currentDay < 5)
        {
            randomPrice = Random.Range(this._easyMinHealthTax, this._easyMaxHealthTax + 1);
        }
        else if (this.currentDay < 10)
        {
            randomPrice = Random.Range(this._medMinHealthTax, this._medMaxHealthTax + 1);
        }
        else
        {
            randomPrice = Random.Range(this._hardMinHealthTax, this._hardMaxHealthTax + 1);
        }

        int reduction = randomPrice % 10; //Ensures that all prices are multiples of 10

        return (randomPrice - reduction);
    }

    public void StartDayTransitionSequence()
    {
        this._dayTitleLabel.gameObject.SetActive(true);
        this._dayTitleLabel.text = "Start of Day " + (this.currentDay + 1).ToString();
        this._happinessMeter.gameObject.SetActive(true);
        StartCoroutine(this.DayTransitionSequence());
    }

    private IEnumerator DayTransitionSequence()
    {
        Vector3 targetScale = this._labelInitialScale;
        Vector3 startingScale = this._labelInitialScale * 3.0f;

        this._dayTitleLabel.transform.localScale = startingScale;

        this._dayTitleLabel.transform.DOScale(targetScale, 0.2f);

        AudioManager.instance.Play(this.thudSound, this._channelSettings);

        yield return new WaitForSeconds(1.0f);

        AudioManager.instance.Play(this.thudSound, this._channelSettings);

        this._happinessMeter.DisplayMeter();

        yield return new WaitForSeconds(0.5f);

        int healthTax = this.GetRandomHealthTax();

        this._happinessMeter.RemoveHealth(healthTax);

        AudioManager.instance.Play(this.damageSound, this._channelSettings);

        HappinessManager.instance.UpdateMeter();

        yield return new WaitForSeconds(2.0f);

        this._dayTitleLabel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        this._happinessMeter.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);

        AudioManager.instance.Play(this.zoomSound, this._channelSettings);

        yield return new WaitForSeconds(0.5f);

        if (HappinessManager.instance._currentHealth <= 0)
        {
            GameOver.instance.TriggerGameOver();            
        }
        else 
        {
            Instantiate(this._shopPrefab);

            CardPacksManager.instance.ChangeBackgroundToStore();

            this._dayTitleLabel.gameObject.SetActive(false);
            this._happinessMeter.gameObject.SetActive(false);

            FadeManager.instance.FadeFromBlack();

            this.currentDay++;

            this._dayLabel.text = "Day " + this.currentDay.ToString();

            MusicManager.instance.FadeToShopMusic();

            AudioManager.instance.Play(this.welcome, this._channelSettings);
        }
    }
}
