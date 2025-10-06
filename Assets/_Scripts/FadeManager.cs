using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;    

    private Image _fadePanel;

    private float _timeToFade = 0.5f;

    private void Awake()
    {
        this._fadePanel = GetComponentInChildren<Image>();

        if (instance == null)
        {
            instance = this;
        }
    }

    public void FadeToBlack(UnityAction callback = null)
    {
        Color targetColor = Color.black;
        Color currentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        DOTween.To(() => currentColor, x => currentColor = x, targetColor, this._timeToFade).SetLink(this.gameObject).OnUpdate(() =>
        {
            this._fadePanel.color = currentColor;
        });

        StartCoroutine(this.FadeCallbackDelay(callback));
    }

    private IEnumerator FadeCallbackDelay(UnityAction callback)
    {
        yield return new WaitForSeconds(this._timeToFade);

        if (callback != null)
        {
            callback();
        }
    }

    public void FadeFromBlack(UnityAction callback = null)
    {
        Color targetColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        Color currentColor = Color.black;

        DOTween.To(() => currentColor, x => currentColor = x, targetColor, this._timeToFade).SetLink(this.gameObject).OnUpdate(() =>
        {
            this._fadePanel.color = currentColor;
        });

        StartCoroutine(this.FadeCallbackDelay(callback));
    }
}
