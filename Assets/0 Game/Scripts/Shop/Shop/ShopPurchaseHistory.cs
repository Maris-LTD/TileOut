using System;
using System.Collections.Generic;

namespace IAPModule.Adapters.Shop
{
    [Serializable]
    public class PurchaseRecord
    {
        public string productId;
        public DateTime purchaseTime;
        public int purchaseCount;

        public PurchaseRecord(string productId, DateTime purchaseTime)
        {
            this.productId = productId;
            this.purchaseTime = purchaseTime;
            this.purchaseCount = 1;
        }
    }

    [Serializable]
    public class ShopPurchaseHistory
    {
        public List<PurchaseRecord> purchaseRecords = new List<PurchaseRecord>();

        public void AddPurchase(string productId)
        {
            var record = purchaseRecords.Find(r => r.productId == productId);
            if (record != null)
            {
                record.purchaseCount++;
                record.purchaseTime = DateTime.Now;
            }
            else
            {
                purchaseRecords.Add(new PurchaseRecord(productId, DateTime.Now));
            }
        }

        public PurchaseRecord GetRecord(string productId)
        {
            return purchaseRecords.Find(r => r.productId == productId);
        }

        public int GetPurchaseCount(string productId)
        {
            var record = GetRecord(productId);
            return record?.purchaseCount ?? 0;
        }

        public DateTime GetLastPurchaseTime(string productId)
        {
            var record = GetRecord(productId);
            return record?.purchaseTime ?? DateTime.MinValue;
        }

        public bool ShouldReset(string productId, int resetHours)
        {
            var record = GetRecord(productId);
            if (record == null) return false;

            var timeSincePurchase = DateTime.Now - record.purchaseTime;
            return timeSincePurchase.TotalHours >= resetHours;
        }

        public void ResetPurchase(string productId)
        {
            var record = GetRecord(productId);
            if (record != null)
            {
                record.purchaseCount = 0;
                record.purchaseTime = DateTime.MinValue;
            }
        }
    }
}



