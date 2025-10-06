using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObject : MonoBehaviour
{
    public float maxValue = 275;
    public float minValue = 230;

    private RectTransform rectTransform;

    private void Awake()
    {
        this.rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.BounceCoroutine());        
    }

    private IEnumerator BounceCoroutine()
    {
        while (true)
        {
            this.rectTransform.DOLocalMoveY(this.maxValue, 1.0f).SetEase(Ease.InOutBack);
            yield return new WaitForSeconds(1.0f);
            this.rectTransform.DOLocalMoveY(this.minValue, 1.0f).SetEase(Ease.InOutBack);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
