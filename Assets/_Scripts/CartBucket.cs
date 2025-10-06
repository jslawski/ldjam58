using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartBucket : RedeemBucket
{
    public override  void DisplayBucket()    
    {
        this.bucketTransform.localScale = Vector3.zero;
        this.bucketTransform.DOScale(Vector3.one * 4.0f, 0.2f).SetEase(Ease.OutBack);
    }
}
