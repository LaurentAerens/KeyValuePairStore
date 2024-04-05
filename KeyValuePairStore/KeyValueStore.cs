using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class KeyValueStore
{
    private Dictionary<string, object> store;
    private string filePath;
    DeltaTime ContinuesStoreTime = new DeltaTime();
    int cleanUpTime;


    /// Represents a key-value store that persists data to a file.
    /// </summary>
    /// <param name="filePath">The path to the file where the data is stored. If not provided, a default file path will be used.</param>
    /// <param name="continuesStoreTime">The time interval for continuous data storage. If not provided, a default value will be used.</param>
    /// <param name="cleanUpTime">The time interval for cleaning up old data. If not provided, a default value will be used.</param>
    /// <param name="OverwriteSetting">A flag indicating whether to overwrite the existing settings in the store. If set to false, the settings in the store will be compared with the provided values.</param>
    public KeyValueStore(string filePath = null, DeltaTime continuesStoreTime = null, int cleanUpTime = 0, bool OverwriteSetting = false)
    {
        bool MissmatchSettings = false;
        if (continuesStoreTime == null)
        {
            continuesStoreTime = new DeltaTime();
        }
        ContinuesStoreTime = continuesStoreTime;
        if (!ContinuesStoreTime.IsOff && cleanUpTime == 0)
        {
            cleanUpTime = 3;
        }
        this.cleanUpTime = cleanUpTime;
        if (filePath == null)
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keyValueStore.json");
        }
        this.filePath = filePath;

        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var settingsjson = JsonConvert.SerializeObject(data["Settings"]);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsjson);
            var storejson = JsonConvert.SerializeObject(data["Store"]);
            store = JsonConvert.DeserializeObject<Dictionary<string, object>>(storejson);
            if (!OverwriteSetting)
            {
                if (settings.ContainsKey("ContinuesStoreTime") && settings.ContainsKey("cleanUpTime"))
                {
                    string JsonContinuesStoreTime = JsonConvert.SerializeObject(settings["ContinuesStoreTime"]);
                    DeltaTime ContinuesStoreTimeSettings = CreateDeltaTimeFromJson(JsonContinuesStoreTime);

                    if (ContinuesStoreTime.IsOff)
                    {
                        ContinuesStoreTime = ContinuesStoreTimeSettings;
                    }
                    else
                    {
                        if (ContinuesStoreTime != ContinuesStoreTimeSettings)
                        {
                            MissmatchSettings = true;
                        }
                    }
                    if (cleanUpTime == 0)
                    {
                        cleanUpTime = (int)(long)settings["cleanUpTime"];
                    }
                    else
                    {
                        if (cleanUpTime != (int)(long)settings["cleanUpTime"])
                        {
                            MissmatchSettings = true;
                        }
                    }
                }
                else
                {
                    MissmatchSettings = true;
                }
            }
        }
        else
        {
            store = new Dictionary<string, object>();
        }
        Save();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => RemoveOldKeys();
        if (MissmatchSettings)
        {
            throw new ArgumentException("The settings in the keyValueStore en the contructor do not match, Constructor onces are taken: This might brake stuff.");
        }
    }
    private DeltaTime CreateDeltaTimeFromJson(string json)
    {
        var jObject = JObject.Parse(json);

        int years = (int)jObject["Years"];
        int months = (int)jObject["Months"];
        int days = (int)jObject["Days"];
        int hours = (int)jObject["Hours"];

        return new DeltaTime(years, months, days, hours);
    }
    private void Save()
    {
        var settings = new Dictionary<string, object>
        {
            { "ContinuesStoreTime", ContinuesStoreTime },
            { "cleanUpTime", cleanUpTime }
        };

        var data = new Dictionary<string, object>
        {
            { "Settings", settings },
            { "Store", store }
        };

        var content = JsonConvert.SerializeObject(data);
        File.WriteAllText(filePath, content);
    }

    /// <summary>
    /// Retrieves the keys from the key-value store.
    /// </summary>
    /// <param name="WithDateTime">Indicates whether to include the date and time in the keys.</param>
    /// <returns>A list of keys.</returns>
    public List<string> GetKeys(bool WithDateTime = false)
    {
        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        if (WithDateTime || ContinuesStoreTime.IsOff)
        {
            return store.Keys.ToList();
        }
        else
        {
            List<string> keys = new List<string>();

            if (timeToKeep[0] != 0)
            {
                foreach (string key in store.Keys)
                {
                    if (int.TryParse(key.Substring(key.Length - 10), out _))
                    {
                        keys.Add(key.Substring(0, key.Length - 10));
                    }
                    else
                    {
                        keys.Add(key);
                    }
                }
            }
            else if (timeToKeep[1] != 0)
            {
                foreach (string key in store.Keys)
                {
                    if (int.TryParse(key.Substring(key.Length - 8), out _))
                    {
                        keys.Add(key.Substring(0, key.Length - 8));
                    }
                    else
                    {
                        keys.Add(key);
                    }
                }
            }
            else if (timeToKeep[2] != 0)
            {
                foreach (string key in store.Keys)
                {
                    if (int.TryParse(key.Substring(key.Length - 6), out _))
                    {
                        keys.Add(key.Substring(0, key.Length - 6));
                    }
                    else
                    {
                        keys.Add(key);
                    }
                }
            }
            else if (timeToKeep[3] != 0)
            {
                foreach (string key in store.Keys)
                {
                    if (int.TryParse(key.Substring(key.Length - 4), out _))
                    {
                        keys.Add(key.Substring(0, key.Length - 4));
                    }
                    else
                    {
                        keys.Add(key);
                    }
                }
            }
            else
            {
                keys = store.Keys.ToList();
            }

            return keys;
        }

    }

    /// <summary>
    /// Retrieves the entire key-value store.
    /// </summary>
    /// <returns>The key-value store as a dictionary.</returns>
    public Dictionary<string, object> GetStore()
    {
        return store;
    }
    
    /// <summary>
    /// Retrieves the settings of the key-value store.
    /// </summary>
    /// <returns>A dictionary containing the settings.</returns>
    public Dictionary<string, object> GetSettings()
    {
        if (ContinuesStoreTime.IsOff)
        {
            return null;
        }
        else
        {
            var settings = new Dictionary<string, object>
            {
                { "ContinuesStoreTime", ContinuesStoreTime },
                { "cleanUpTime", cleanUpTime }
            };
            return settings;
        }

    }

    /// <summary>
    /// Retrieves the versions of keys that match the specified key name.
    /// </summary>
    /// <param name="keyName">The name of the key.</param>
    /// <returns>A dictionary containing Iterations and Date for the key.</returns>
    public Dictionary<int, string> GetKeyVersions(string keyName)
    {
        if (ContinuesStoreTime.IsOff)
        {
            return null;
        }
        Dictionary<int, string> keyVersions = new Dictionary<int, string>();
        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        foreach (string key in store.Keys)
        {
            if (key.StartsWith(keyName) && !key.Equals(keyName))
            {
                if (timeToKeep[0] != 0)
                {
                    string suffix = key.Substring(key.Length - 10);
                    if (int.TryParse(suffix, out int iteration))
                    {
                        keyVersions.Add(GetIterationBack(suffix), suffix);
                    }
                    else
                    {
                        return keyVersions;
                    }
                }
                else if (timeToKeep[1] != 0)
                {
                    string suffix = key.Substring(key.Length - 8);
                    if (int.TryParse(suffix, out int iteration))
                    {
                        keyVersions.Add(GetIterationBack(suffix), suffix);
                    }
                    else
                    {
                        return keyVersions;
                    }
                }
                else if (timeToKeep[2] != 0)
                {
                    string suffix = key.Substring(key.Length - 6);
                    if (int.TryParse(suffix, out int iteration))
                    {
                        keyVersions.Add(GetIterationBack(suffix), suffix);
                    }
                    else
                    {
                        return keyVersions;
                    }
                }
                else if (timeToKeep[3] != 0)
                {
                    string suffix = key.Substring(key.Length - 4);
                    if (int.TryParse(suffix, out int iteration))
                    {
                        keyVersions.Add(GetIterationBack(suffix), suffix);
                    }
                    else
                    {
                        return keyVersions;
                    }
                }

            }
        }
        keyVersions = keyVersions.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        return keyVersions;
    }

    /// <summary>
    /// Retrieves the keys and their corresponding versions from the store.
    /// </summary>
    /// <returns>A dictionary containing the keys and their versions.</returns>
    public Dictionary<string, List<string>> GetKeysVersions()
    {
        if (ContinuesStoreTime.IsOff)
        {
            Dictionary<string, List<string>> keysNoVersions = new Dictionary<string, List<string>>();
            foreach (string key in store.Keys)
            {
                keysNoVersions[key] = null;
            }
            return keysNoVersions;
        }
        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        Dictionary<string, List<string>> keyVersions = new Dictionary<string, List<string>>();
        if (timeToKeep[0] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 10);
                string keySuffix = key.Substring(key.Length - 10);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyVersions.ContainsKey(key))
                    {
                        keyVersions[key] = new List<string>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyVersions.ContainsKey(keyName))
                {
                    keyVersions[keyName] = new List<string>();
                }
                keyVersions[keyName].Add(keySuffix);
            }
        }
        else if (timeToKeep[1] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 8);
                string keySuffix = key.Substring(key.Length - 8);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyVersions.ContainsKey(key))
                    {
                        keyVersions[key] = new List<string>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyVersions.ContainsKey(keyName))
                {
                    keyVersions[keyName] = new List<string>();
                }
                keyVersions[keyName].Add(keySuffix);
            }
        }
        else if (timeToKeep[2] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 6);
                string keySuffix = key.Substring(key.Length - 6);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyVersions.ContainsKey(key))
                    {
                        keyVersions[key] = new List<string>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyVersions.ContainsKey(keyName))
                {
                    keyVersions[keyName] = new List<string>();
                }
                keyVersions[keyName].Add(keySuffix);
            }           
        }
        else if (timeToKeep[3] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 4);
                string keySuffix = key.Substring(key.Length - 4);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyVersions.ContainsKey(key))
                    {
                        keyVersions[key] = new List<string>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyVersions.ContainsKey(keyName))
                {
                    keyVersions[keyName] = new List<string>();
                }
                keyVersions[keyName].Add(keySuffix);
            }
        }
        return keyVersions;
    }
    /// <summary>
    /// Retrieves the keys and their corresponding iterations based on the time values set for the store.
    /// </summary>
    /// <returns>A dictionary containing the keys and their iterations.</returns>
    public Dictionary<string, List<int>> GetKeysIterations()
    {
        if(ContinuesStoreTime.IsOff)
        {
            return null;
        }
        Dictionary<string, List<int>> keyIterations = new Dictionary<string, List<int>>();
        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        if (timeToKeep[0] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 10);
                string keySuffix = key.Substring(key.Length - 10);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyIterations.ContainsKey(key))
                    {
                        keyIterations[key] = new List<int>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyIterations.ContainsKey(keyName))
                {
                    keyIterations[keyName] = new List<int>();
                }
                int iteration = GetIterationBack(keySuffix);
                keyIterations[keyName].Add(iteration);
            }
        }
        else if (timeToKeep[1] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 8);
                string keySuffix = key.Substring(key.Length - 8);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyIterations.ContainsKey(key))
                    {
                        keyIterations[key] = new List<int>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyIterations.ContainsKey(keyName))
                {
                    keyIterations[keyName] = new List<int>();
                }
                int iteration = GetIterationBack(keySuffix);
                keyIterations[keyName].Add(iteration);
            }
        }
        else if (timeToKeep[2] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 6);
                string keySuffix = key.Substring(key.Length - 6);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyIterations.ContainsKey(key))
                    {
                        keyIterations[key] = new List<int>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyIterations.ContainsKey(keyName))
                {
                    keyIterations[keyName] = new List<int>();
                }
                int iteration = GetIterationBack(keySuffix);
                keyIterations[keyName].Add(iteration);
            }
        }
        else if (timeToKeep[3] != 0)
        {
            foreach (string key in store.Keys)
            {
                string keyName = key.Substring(0, key.Length - 4);
                string keySuffix = key.Substring(key.Length - 4);
                if (!int.TryParse(keySuffix, out _))
                {
                    if (!keyIterations.ContainsKey(key))
                    {
                        keyIterations[key] = new List<int>();
                    }
                    continue; // Skip the key if keySuffix is not an integer
                }
                if (!keyIterations.ContainsKey(keyName))
                {
                    keyIterations[keyName] = new List<int>();
                }
                int iteration = GetIterationBack(keySuffix);
                keyIterations[keyName].Add(iteration);
            }
        }
        return keyIterations;
    }

    /// <summary>
    /// Sets the value associated with the specified key in the key-value store.
    /// If ContinuesStoreTime is off, the value is directly stored in the store and saved.
    /// If ContinuesStoreTime is on, We update the key or make a new one if that is needed.
    /// </summary>
    /// <param name="key">The key to associate the value with.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void Set(string key, object value, bool straightKey = false)
    {
        if (ContinuesStoreTime.IsOff || straightKey)
        {
            store[key] = value;
            Save();
            return;
        }
        else
        {
            List<string> keyNames = Checkkeys(key);
            foreach (string keyName in keyNames)
            {
                if (store.ContainsKey(keyName))
                {
                    store[keyName] = value;
                }
            }
            store[GenereateKey(key)] = value;
            Save();
        }
    }

    /// <summary>
    /// Retrieves the value associated with the specified key from the key-value store.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">The number of iterations ago to search for the key.</param>
    /// <param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>
    /// The value associated with the specified key, or null if the key is not found.
    /// </returns>
    public object Get(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        try
        {
            object result = null;
            if (ContinuesStoreTime.IsOff || straightLookup)
            {
                return store[key];
            }
            else
            {
                List<string> keyNames = Checkkeys(key, iterationsAgo, lookupDate);
                foreach (string keyName in keyNames)
                {
                    if (store.ContainsKey(keyName))
                    {
                        result = store[keyName];
                    }
                }
            }
            return result;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Sets the value of a string key in the key-value store.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void SetString(string key, string value, bool straightKey = false)
    {
        Set(key, value, straightKey);
    }
    /// <summary>
    /// Retrieves the value associated with the specified key as a string.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">
    /// The number of iterations ago to get the value from. This is multiplied by DeltaTime to calculate the actual time ago.
    /// If the value from the specified number of iterations ago does not exist, an error is thrown instead of returning empty string.
    /// </param>
    ///<param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>The value associated with the specified key as a string, or empty string if the key does not exist or the value is not a string.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist in the store and iterationsAgo != 0.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not a string.</exception>
    public string GetString(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        var value = Get(key, iterationsAgo, lookupDate, straightLookup);
        if (value == null)
        {
            return string.Empty;
        }
        else if (value is string stringValue)
        {
            return stringValue;
        }
        else
        {
            throw new InvalidCastException($"The value for key '{key}' is not of type 'string'.");
        }
    }

    /// <summary>
    /// Sets the value of the specified key as an integer.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The integer value to set.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void SetInt(string key, int value, bool straightKey = false)
    {
        Set(key, value, straightKey);
    }

    /// <summary>
    /// Retrieves the value associated with the specified key as a integer.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">
    /// The number of iterations ago to get the value from. This is multiplied by DeltaTime to calculate the actual time ago.
    /// If the value from the specified number of iterations ago does not exist, an error is thrown instead of returning 0.
    /// </param>
    /// <param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>The value associated with the specified key as a integer, or 0 if the key does not exist or the value is not a integer.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist in the store and iterationsAgo != 0.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not a integer.</exception>
    public int GetInt(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        var value = Get(key, iterationsAgo, lookupDate, straightLookup);
        if (value == null)
        {
            return 0;
        }
        else if (value is int intValue)
        {
            return intValue;
        }
        else
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"The value for key '{key}' is not of type 'int'.");
            }
            catch (FormatException)
            {
                throw new InvalidCastException($"The value for key '{key}' is not of type 'int'.");
            }

        }
    }

    /// <summary>
    /// Sets the value of the specified key as a double.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The double value to set.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void SetDouble(string key, double value, bool straightKey = false)
    {
        Set(key, value, straightKey);
    }

    /// <summary>
    /// Retrieves the value associated with the specified key as a double.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">
    /// The number of iterations ago to get the value from. This is multiplied by DeltaTime to calculate the actual time ago.
    /// If the value from the specified number of iterations ago does not exist, an error is thrown instead of returning 0.
    /// </param>
    /// <param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>The value associated with the specified key as a double, or 0 if the key does not exist or the value is not a double.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist in the store and iterationsAgo != 0.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not a double.</exception>
    public double GetDouble(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        var value = Get(key, iterationsAgo, lookupDate, straightLookup);
        if (value == null)
        {
            return 0;
        }
        else if (value is double doubleValue)
        {
            return doubleValue;
        }
        else
        {
            throw new InvalidCastException($"The value for key '{key}' is not of type 'double'.");
        }
    }

    /// <summary>
    /// Sets the value of the specified key as a long.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The long value to set.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void SetLong(string key, long value, bool straightKey = false)
    {
        Set(key, value, straightKey);
    }

    /// <summary>
    /// Retrieves the value associated with the specified key as a long.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">
    /// The number of iterations ago to get the value from. This is multiplied by DeltaTime to calculate the actual time ago.
    /// If the value from the specified number of iterations ago does not exist, an error is thrown instead of returning 0.
    /// </param>
    /// <param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>The value associated with the specified key as a long, or 0 if the key does not exist or the value is not a long.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist in the store and iterationsAgo != 0.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not a long.</exception>
    public long GetLong(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        var value = Get(key, iterationsAgo, lookupDate, straightLookup);
        if (value == null)
        {
            return 0;
        }
        else if (value is long longValue)
        {
            return longValue;
        }
        else
        {
            throw new InvalidCastException($"The value for key '{key}' is not of type 'long'.");
        }
    }

    /// <summary>
    /// Sets the value of the specified key as a char.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The char value to set.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void SetChar(string key, char value, bool straightKey = false)
    {
        Set(key, value, straightKey);
    }

    /// <summary>
    /// Retrieves the value associated with the specified key as a char.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">
    /// The number of iterations ago to get the value from. This is multiplied by DeltaTime to calculate the actual time ago.
    /// If the value from the specified number of iterations ago does not exist, an error is thrown instead of returning '\0'.
    /// </param>
    /// <param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>The value associated with the specified key as a char, or '\0' if the key does not exist or the value is not a char.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist in the store and iterationsAgo != 0.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not a char.</exception>
    public char GetChar(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        var value = Get(key, iterationsAgo, lookupDate, straightLookup);
        if (value == null)
        {
            return '\0';
        }
        else if (value is char charValue)
        {
            return charValue;
        }
        else
        {
            try
            {
                char charvalue = Convert.ToChar(value);
                if (charvalue == '\0')
                {
                    throw new InvalidCastException($"The value for key '{key}' is not of type 'char'.");
                }
                else
                {
                    return charvalue;
                }
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"The value for key '{key}' is not of type 'char'.");
            }
        }
    }

    /// <summary>
    /// Sets the value of the specified key as a bool.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The bool value to set.</param>
    /// <param name="straightKey">A flag that allows you to turn of the keyDeltaTime Proceccing</param>
    public void SetBool(string key, bool value, bool straightKey = false)
    {
        Set(key, value, straightKey);
    }

    /// <summary>
    /// Retrieves the value associated with the specified key as a bool.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="iterationsAgo">
    /// The number of iterations ago to get the value from. This is multiplied by DeltaTime to calculate the actual time ago.
    /// If the value from the specified number of iterations ago does not exist, an error is thrown instead of returning false.
    /// </param>
    /// <param name="lookupDate">The lookup date get the keyValue on a certain date</param>
    /// <param name="straightLookup">Removes all DateTime processing and look the key as is</param>
    /// <returns>The value associated with the specified key as a bool, or false if the key does not exist or the value is not a bool.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist in the store and iterationsAgo != 0.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value associated with the key is not a bool.</exception>
    public bool GetBool(string key, int iterationsAgo = 0, DateTime lookupDate = default, bool straightLookup = false)
    {
        var value = Get(key, iterationsAgo, lookupDate, straightLookup);
        if (value == null)
        {
            return false;
        }
        else if (value is bool boolValue)
        {
            return boolValue;
        }
        else
        {
            throw new InvalidCastException($"The value for key '{key}' is not of type 'bool'.");
        }
    }


    private List<string> Checkkeys(string key, int iterationsAgo = 0, DateTime lookupDate = default)
    {
        if (lookupDate == default)
        {
            lookupDate = DateTime.Now;
        }
        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        List<string> keyNames = new List<string>();


        if (timeToKeep[0] != 0)
        {
            for (int i = 0; i < timeToKeep[0]; i++)
            {
                keyNames.Add(key + lookupDate.AddHours(-i - iterationsAgo * timeToKeep[0]).ToString("yyyyMMDDHH"));
            }
        }
        else if (timeToKeep[1] != 0)
        {
            for (int i = 0; i < timeToKeep[1]; i++)
            {
                keyNames.Add(key + lookupDate.AddDays(-i - iterationsAgo * timeToKeep[1]).ToString("yyyyMMDD"));
            }
        }
        else if (timeToKeep[2] != 0)
        {
            for (int i = 0; i < timeToKeep[2]; i++)
            {
                keyNames.Add(key + lookupDate.AddMonths(-i - iterationsAgo * timeToKeep[2]).ToString("yyyyMM"));
            }
        }
        else if (timeToKeep[3] != 0)
        {
            for (int i = 0; i < timeToKeep[3]; i++)
            {
                keyNames.Add(key + lookupDate.AddYears(-i - iterationsAgo * timeToKeep[3]).ToString("yyyy"));
            }
        }
        else
        {
            keyNames.Add(key);
        }

        return keyNames;
    }
    private string GenereateKey(string key)
    {

        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        string keyName;

        if (timeToKeep[0] != 0)
        {
            keyName = key + DateTime.Now.ToString("yyyyMMDDHH");
        }
        else if (timeToKeep[1] != 0)
        {
            keyName = key + DateTime.Now.ToString("yyyyMMDD");
        }
        else if (timeToKeep[2] != 0)
        {
            keyName = key + DateTime.Now.ToString("yyyyMM");
        }
        else if (timeToKeep[3] != 0)
        {
            keyName = key + DateTime.Now.ToString("yyyy");
        }
        else
        {
            keyName = key;
        }

        return keyName;
    }
    private int GetIterationBack(string suffix)
    {
        int keyDate = int.Parse(suffix);
        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        if (timeToKeep[0] != 0)
        {
            int currentDate = int.Parse(DateTime.Now.ToString("yyyyMMDDHH"));
            return (currentDate - keyDate) / timeToKeep[0];
        }
        else if (timeToKeep[1] != 0)
        {
            int currentDate = int.Parse(DateTime.Now.ToString("yyyyMMDD"));
            return (currentDate - keyDate) / timeToKeep[1];
        }
        else if (timeToKeep[2] != 0)
        {
            int currentDate = int.Parse(DateTime.Now.ToString("yyyyMM"));
            return (currentDate - keyDate) / timeToKeep[2];
        }
        else if (timeToKeep[3] != 0)
        {
            int currentDate = int.Parse(DateTime.Now.ToString("yyyy"));
            return (currentDate - keyDate) / timeToKeep[3];
        }
        else
        {
            return 0;
        }
    }
    // cleanup stuff
    private void RemoveOldKeys()
    {
        var (minLenght, minDate, TimeMode) = CalculateCleanupData();
        if (TimeMode == 4)
        {
            return;
        }
        var currentDate = DateTime.Now;
        var keysToRemove = new List<string>();
        foreach (var key in store.Keys)
        {
            if (IsOldKey(key, currentDate, minLenght, minDate, TimeMode))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            store.Remove(key);
        }

        Save();
    }

    private bool IsOldKey(string key, DateTime currentDate, int minLenght, DateTime MinDate, int TimeMode)
    {

        if (key.Length < minLenght)
        {
            return false;
        }

        string Datestring = key.Substring(key.Length - minLenght);
        int year, month, day, hour;
        DateTime keyDate = new DateTime();
        switch (TimeMode)
        {
            case 0:
                if (!int.TryParse(Datestring.Substring(0, 4), out year) || !int.TryParse(Datestring.Substring(4, 2), out month) || !int.TryParse(key.Substring(6, 2), out day) || !int.TryParse(key.Substring(8, 2), out hour))
                {
                    return false;
                }
                keyDate = new DateTime(year, month, day, hour, 0, 0);
                return keyDate < MinDate;
            case 1:
                if (!int.TryParse(Datestring.Substring(0, 4), out year) || !int.TryParse(Datestring.Substring(4, 2), out month) || !int.TryParse(key.Substring(6, 2), out day))
                {
                    return false;
                }
                keyDate = new DateTime(year, month, day);
                return keyDate < MinDate;
            case 2:
                if (!int.TryParse(Datestring.Substring(0, 4), out year) || !int.TryParse(Datestring.Substring(4, 2), out month))
                {
                    return false;
                }
                keyDate = new DateTime(year, month, 1);
                return keyDate < MinDate;
            case 3:
                if (!int.TryParse(Datestring.Substring(0, 4), out year))
                {
                    return false;
                }
                keyDate = new DateTime(year, 1, 1);
                return keyDate < MinDate;
            default:
                return false;
        }
    }
    private (int, DateTime, int) CalculateCleanupData()
    {
        if (ContinuesStoreTime.IsOff)
        {
            return (0, DateTime.Now, 4);
        }
        int mode = 0;
        DateTime minDate = DateTime.Now;
        int minLenght = 0;

        int[] timeToKeep = ContinuesStoreTime.GetTimeValues();
        if (timeToKeep[0] != 0)
        {
            mode = 0;
            minLenght = 10;
            minDate = DateTime.Now.AddHours(-timeToKeep[0] - cleanUpTime);
        }
        else if (timeToKeep[1] != 0)
        {
            mode = 1;
            minLenght = 8;
            minDate = DateTime.Now.AddDays(-timeToKeep[1] - cleanUpTime);
        }
        else if (timeToKeep[2] != 0)
        {
            mode = 2;
            minLenght = 6;
            minDate = DateTime.Now.AddMonths(-timeToKeep[2] - cleanUpTime);
        }
        else if (timeToKeep[3] != 0)
        {
            mode = 3;
            minLenght = 4;
            minDate = DateTime.Now.AddYears(-timeToKeep[3] - cleanUpTime);
        }
        else
        {
            mode = 4;
            minLenght = 0;
            minDate = DateTime.Now;
        }
        return (minLenght, minDate, mode);

    }


}
