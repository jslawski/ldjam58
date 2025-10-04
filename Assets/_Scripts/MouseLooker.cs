using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLooker : MonoBehaviour
{    
    private Transform _rootTransform;

    [SerializeField]
    private Transform _modelTransform;

    private Collider _collider;

    [SerializeField]
    private LayerMask _colliderLayerMask;

    private Ray _mouseRay;
    private RaycastHit _hitInfo;

    private Tween _resetTween;

    private Vector2 _maxDistanceThresholds = new Vector2(0.1f, 0.3f);
    private Vector2 _maxRotationThresholds = new Vector3(15.0f, 7.0f);

    private float lookSpeed = 10.0f;

    public bool _lookActive = false;

    private void Awake()
    {
        this._collider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EnableMouseLook()
    {
        this._lookActive = true;
    }

    public void DisableMouseLook()
    {
        this._lookActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        this._mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(this._mouseRay, out this._hitInfo, 100.0f, this._colliderLayerMask) == true)
        {
            this.LookAtMouse();
        }
        else if (this._modelTransform.localRotation != Quaternion.identity && this._resetTween == null)
        {
            this.ResetRotation();
        }

        if (this._resetTween != null && this._resetTween.IsActive() == false)
        {
            this._resetTween = null;
        }
    }

    private void LookAtMouse()
    {
        this._modelTransform.DOKill();

        Vector3 viewportCardPosition = Camera.main.WorldToViewportPoint(this._modelTransform.position);
        Vector3 viewportPointPosition = Camera.main.WorldToViewportPoint(this._hitInfo.point);

        float xDistance = Mathf.Clamp(viewportPointPosition.x - viewportCardPosition.x, -this._maxDistanceThresholds.x, this._maxDistanceThresholds.x);
        float yDistance = Mathf.Clamp(viewportPointPosition.y - viewportCardPosition.y, -this._maxDistanceThresholds.y, this._maxDistanceThresholds.y);

        Vector2 rotationValues = this.GetRotationValues(xDistance, yDistance);

        this._modelTransform.localRotation = Quaternion.Lerp(this._modelTransform.localRotation, Quaternion.Euler(rotationValues.x, rotationValues.y, 0.0f), this.lookSpeed * Time.deltaTime);
    }

    private Vector2 GetRotationValues(float xDistance, float yDistance)
    {
        Vector2 rotationValues = Vector2.zero;

        //Yes this is correct.  The rotation axis and the distance coordinate are two different things
        rotationValues.x = (yDistance / this._maxDistanceThresholds.y) * this._maxRotationThresholds.y;
        rotationValues.y = -(xDistance / this._maxDistanceThresholds.x) * this._maxRotationThresholds.x;

        return rotationValues;
    }

    public void ResetRotation()
    {
        this._resetTween = this._modelTransform.DORotate(Vector3.zero, 0.2f);
    }
}
