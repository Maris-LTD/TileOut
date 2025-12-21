namespace GameModules.DataManager
{
    /// <summary>
    /// Interface defines standard methods for DataProvider
    /// Note: Classes implementing this interface will be automatically recognized by DataManager
    /// However, you can also create classes that do not implement this interface,
    /// as long as they have enough methods with standard names
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Load data into provider
        /// </summary>
        /// <param name="data">Data to load (can be JSON string, byte array, or object)</param>
        /// <returns>True if loading succeeds, false if fails</returns>
        bool LoadData(object data);
        
        /// <summary>
        /// Get data from provider
        /// </summary>
        /// <returns>Current data from provider</returns>
        object GetData();
        
        /// <summary>
        /// Save data from provider
        /// </summary>
        /// <returns>Data has been serialized for storage</returns>
        object SaveData();
        
        /// <summary>
        /// Check if the provider has data
        /// </summary>
        /// <returns>True if data exists, false if not</returns>
        bool HasData();
        
        /// <summary>
        /// Clear all data in provider
        /// </summary>
        void ClearData();
    }
    
    /// <summary>
    /// Generic interface for DataProvider with specific type
    /// </summary>
    /// <typeparam name="T">Data type of provider</typeparam>
    public interface IDataProvider<T> : IDataProvider
    {
        /// <summary>
        /// Load data with specific type
        /// </summary>
        /// <param name="data">Data to load</param>
        /// <returns>True if load successful</returns>
        bool LoadData(T data);
        
        /// <summary>
        /// Get data with specific type
        /// </summary>
        /// <returns>Current data</returns>
        new T GetData();
        
        /// <summary>
        /// Save data with specific type
        /// </summary>
        /// <returns>Serialized data</returns>
        new T SaveData();
    }
}
