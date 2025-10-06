using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public void FinishShopping()
    {
        FadeManager.instance.FadeToBlack(this.SetupCardPacksManager);

        MusicManager.instance.FadeToPackMusic();
    }

    private void SetupCardPacksManager()
    {
        CardPacksManager.instance.ChangeBackgroundToOpen();
        CardPacksManager.instance.SetupPacks(this._packMaterials);
        CardPacksManager.instance.StartPackOpeningPhase();

        FadeManager.instance.FadeFromBlack();
        Destroy(this.gameObject);

    }

}
