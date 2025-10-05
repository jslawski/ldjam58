using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillManager : MonoBehaviour
{
    public static BillManager instance;

    private BillAttributes[] _allBills;
    
    private BillBucket[] _allActiveBills;

    public Queue<BillAttributes> completedBillsToPrune;
    public Queue<BillAttributes> failedBillsToPrune;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this._allBills = Resources.LoadAll<BillAttributes>("BillAttributes");
        this._allActiveBills = GetComponentsInChildren<BillBucket>();
        this.completedBillsToPrune = new Queue<BillAttributes>();
        this.failedBillsToPrune = new Queue<BillAttributes>();
    }

    private void Start()
    {
        this.SetupBills();
    }

    private void SetupBills()
    {
        for (int i = 0; i < this._allActiveBills.Length; i++)
        {
            BillAttributes newBill = this.GetNewBill();

            while (this.IsBillDuplicate(newBill, i) == true)
            {
                newBill = this.GetNewBill();
            }

            this._allActiveBills[i].Setup(newBill);
        }
    }

    private bool IsBillDuplicate(BillAttributes bill, int billIndex)
    {
        for (int i = 0; i < billIndex; i++)
        {
            if (this._allActiveBills[i].billAttributes == bill)
            {
                return true;
            }
        }

        return false;
    }

    private BillAttributes GetNewBill(BillAttributes billToReplace = null)
    {
        int randomIndex = Random.Range(0, this._allBills.Length);

        while (this._allBills[randomIndex] == billToReplace) 
        {
            randomIndex = Random.Range(0, this._allBills.Length);
        }

        return this._allBills[randomIndex];
    }

    private void DecrementBillDays()
    {
        for (int i = 0; i < this._allActiveBills.Length; i++)
        { 
            this._allActiveBills[i].DecrementBillDays();
        }
    }

    public void UpdateAndReplaceOldBills()
    {
        this.DecrementBillDays();

        for (int i = 0; i < this._allActiveBills.Length; i++)
        {
            if (this._allActiveBills[i].currentStatus == BillStatus.Complete)
            {
                this.completedBillsToPrune.Enqueue(this._allActiveBills[i].billAttributes);
                this._allActiveBills[i].Setup(this.GetNewBill(this._allActiveBills[i].billAttributes));
            }
            else if (this._allActiveBills[i].currentStatus == BillStatus.Failed)
            {
                this.failedBillsToPrune.Enqueue(this._allActiveBills[i].billAttributes);
                this._allActiveBills[i].Setup(this.GetNewBill(this._allActiveBills[i].billAttributes));
            }
        }
    }
}
