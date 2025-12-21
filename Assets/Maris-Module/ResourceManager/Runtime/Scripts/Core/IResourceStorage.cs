using System.Collections.Generic;

namespace GameModules.ResourceManager
{
    public interface IResourceStorage
    {
        void Save(Dictionary<string, ResourceData> resources);
        Dictionary<string, ResourceData> Load();
        void Clear();
        bool HasData();
    }
}

