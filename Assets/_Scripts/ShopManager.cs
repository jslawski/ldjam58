using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager :MonoBehaviour
{
    public static ShopManager instance;    

    public Queue<Material> _packMaterials;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        this._packMaterials = new Queue<Material>();
    }

    public void PurchasePack(Material purchasedMaterial)
    { 
        this._packMaterials.Enqueue(purchasedMaterial);
    }

    public Material GetNextPackMaterial()
    { 
        return this._packMaterials.Dequeue();
    }
}
