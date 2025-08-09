using System.Reflection;

namespace SimpleRegistry;

public class Registry
{
    private readonly Dictionary<string, IRegistryItem> _registryItems = new();
    
    public void Register<TRegistryType>(string key, TRegistryType obj) where TRegistryType : IRegistryItem
    {
        _registryItems.Add(key, obj);
    }

    public string GetKey<TRegistryType>(TRegistryType obj) where TRegistryType : IRegistryItem
    {
        return _registryItems.FirstOrDefault(x => x.Value.Equals(obj)).Key;
    }

    [Obsolete("Removing registry items can cause many glitches and errors due to missing objects.")]
    public void Unregister<TRegistryType>(TRegistryType obj) where TRegistryType : IRegistryItem
    {
        _registryItems.Remove(GetKey(obj));
    }
    
    [Obsolete("Removing registry items can cause many glitches and errors due to missing objects.")]
    public void Unregister<TRegistryType>(string key) where TRegistryType : IRegistryItem
    {
        _registryItems.Remove(key);
    }
    
    public void OverrideAtKey<TRegistryType>(string key, TRegistryType obj) where TRegistryType : IRegistryItem
    {
        IRegistryItem? existing = _registryItems.GetValueOrDefault(key);
        if (existing == null) throw new RegistryException("Item to override not found.");

        _registryItems[key] = obj;
    }
    
    public void OverrideByOld<TRegistryType>(TRegistryType oldObj, TRegistryType newObj) where TRegistryType : IRegistryItem
    {
        string key = GetKey(oldObj);
        if (string.IsNullOrEmpty(key)) throw new RegistryException("Item to override not found.");

        _registryItems[key] = newObj;
    }
    
    public Dictionary<string, IRegistryItem> GetAllItems()
    {
        return _registryItems;
    }
    
    public Dictionary<string, TRegistryType> GetItemsOfType<TRegistryType>() where TRegistryType : IRegistryItem
    {
        return _registryItems
            .Where(x => x.Value is TRegistryType)
            .ToDictionary(x => x.Key, x => (TRegistryType)x.Value);
    }
    
    public TRegistryType FindItem<TRegistryType>(Func<TRegistryType, bool> predicate)
        where TRegistryType : IRegistryItem
    {
        Dictionary<string, TRegistryType> matchingItems = GetItemsOfType<TRegistryType>();
        return matchingItems.First(x => predicate(x.Value)).Value;
    }
    
    public IEnumerable<TRegistryType> FindItems<TRegistryType>(Func<TRegistryType, bool> predicate)
        where TRegistryType : IRegistryItem
    {
        Dictionary<string, TRegistryType> matchingItems = GetItemsOfType<TRegistryType>();
        return matchingItems
            .Where(x => predicate(x.Value))
            .Select(x => x.Value);
    }
    
    [Obsolete("Removing registry items can cause many glitches and errors due to missing objects.")]
    public void Clear()
    {
        _registryItems.Clear();
    }
    
    [Obsolete("Removing registry items can cause many glitches and errors due to missing objects.")]
    public void Clear<TRegistryType>() where TRegistryType : IRegistryItem 
    {
        var keysToRemove = _registryItems
            .Where(x => x.Value is TRegistryType)
            .Select(x => x.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _registryItems.Remove(key);
        }
    }
}

public interface IRegistryItem;

public class RegistryException : Exception
{
    public RegistryException(string message) : base(message)
    {
    }

    public RegistryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}