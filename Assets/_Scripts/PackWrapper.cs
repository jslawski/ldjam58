using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackWrapper : MonoBehaviour
{
    private Renderer testRenderer;

    private Collider _collider;

    [SerializeField]
    private LayerMask _packLayerMask;

    private void Awake()
    {
        this.testRenderer = GetComponent<Renderer>();
        this._collider = GetComponent<Collider>();
    }

    public bool IsClickingPack()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, 100, this._packLayerMask) == true)
        {
            return true;
        }

        return false;
    }

    public void OpenCardPack()
    {
        this.testRenderer.enabled = false;
        this._collider.enabled = false;
    }
}
