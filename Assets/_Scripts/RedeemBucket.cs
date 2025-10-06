using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeemBucket : MonoBehaviour
{
    public Transform bucketTransform;
    private Vector3 _originalScale;
    private Vector3 _targetScale;

    public int currentValue = 0;

    private void Awake()
    {
        this.bucketTransform = GetComponent<Transform>();
        this._originalScale = transform.localScale;
        this._targetScale = this.bucketTransform.localScale * 1.5f;
    }

    private void Start()
    {
        this.DisplayBucket();
    }

    public virtual void DisplayBucket()
    {
        this.bucketTransform.localScale = Vector3.zero;        
        this.bucketTransform.DOScale(Vector3.one * 3.0f, 0.2f).SetEase(Ease.OutBack);
    }

    public void EmphasizeBucket()
    {
        this.bucketTransform.DOKill();
        this.bucketTransform.DOScale(this._targetScale, 0.2f).SetEase(Ease.OutBack);
    }

    public void RevertBucket()
    {
        this.bucketTransform.DOKill();
        this.bucketTransform.DOScale(this._originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    public virtual void RedeemCardValue(int moneyValue, int happyValue) { }    
}
