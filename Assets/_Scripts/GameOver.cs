using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;

    [SerializeField]
    private TextMeshProUGUI _titleLabel;
    [SerializeField]
    private TextMeshProUGUI _financialValueLabel;
    [SerializeField]
    private TextMeshProUGUI _daysSurvivedLabel;

    [SerializeField]
    private RectTransform _playButtonTransform;
    [SerializeField]
    private RectTransform _backButtonTransform;

    public GameObject gameOverAnimation;

    private AudioChannelSettings _channelSettings;

    public AudioClip thudSound;
    public AudioClip zoomSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this._titleLabel.rectTransform.localScale = Vector3.zero;
        this._financialValueLabel.rectTransform.localScale = Vector3.zero;
        this._daysSurvivedLabel.rectTransform.localScale = Vector3.zero;
        this._playButtonTransform.localScale = Vector3.zero;
        this._backButtonTransform.localScale = Vector3.zero;

        this._channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");

    }

    public void TriggerGameOver()
    {
        this._daysSurvivedLabel.text = "<color=red>" + DayManager.instance.currentDay.ToString() + "</color> Days";
        this._financialValueLabel.text = "Personal Collection Value:\n" + "<color=red>$" + CollectionManager.GetCollectionTotalMoneyValue().ToString() + "</color>";
        StartCoroutine(this.GameOverFlow());
    }

    private IEnumerator GameOverFlow()
    {        
        this.gameOverAnimation.SetActive(true);

        AudioManager.instance.Play(this.zoomSound, this._channelSettings);

        yield return new WaitForSeconds(1.5f);

        this._daysSurvivedLabel.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        AudioManager.instance.Play(this.thudSound, this._channelSettings);

        yield return new WaitForSeconds(0.1f);

        this._titleLabel.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        AudioManager.instance.Play(this.thudSound, this._channelSettings);

        yield return new WaitForSeconds(0.7f);

        this._financialValueLabel.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        AudioManager.instance.Play(this.thudSound, this._channelSettings);

        yield return new WaitForSeconds(0.5f);

        this._playButtonTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        AudioManager.instance.Play(this.zoomSound, this._channelSettings);

        yield return new WaitForSeconds(0.1f);

        this._backButtonTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        AudioManager.instance.Play(this.zoomSound, this._channelSettings);
    }

    public void HideElements()
    {
        this._daysSurvivedLabel.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        this._titleLabel.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        this._financialValueLabel.rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        this._playButtonTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        this._backButtonTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
    }

    public void PlayButtonPressed()
    {
        StartCoroutine(this.PlayButtonFlow());
    }

    public void QuitButtonPressed()
    {
        StartCoroutine(this.QuitButtonFlow());
    }

    private IEnumerator PlayButtonFlow()
    {
        this.HideElements();

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("JaredScene");
    }

    private IEnumerator QuitButtonFlow()
    {
        this.HideElements();

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("MainMenu");
    }
}
