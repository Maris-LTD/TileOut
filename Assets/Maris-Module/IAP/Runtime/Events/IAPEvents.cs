namespace IAPModule.Events
{
    public struct IAPInitializedEvent
    {
        public bool Success;
        
        public IAPInitializedEvent(bool success)
        {
            Success = success;
        }
    }

    public struct IAPPurchaseCompleteEvent
    {
        public string ProductId;
        public bool Success;
        
        public IAPPurchaseCompleteEvent(string productId, bool success)
        {
            ProductId = productId;
            Success = success;
        }
    }

    public struct IAPPurchaseFailedEvent
    {
        public string ProductId;
        public string Reason;
        
        public IAPPurchaseFailedEvent(string productId, string reason)
        {
            ProductId = productId;
            Reason = reason;
        }
    }

    public struct IAPRestoreCompleteEvent
    {
        public bool Success;
        public string Error;
        
        public IAPRestoreCompleteEvent(bool success, string error = null)
        {
            Success = success;
            Error = error;
        }
    }
}
