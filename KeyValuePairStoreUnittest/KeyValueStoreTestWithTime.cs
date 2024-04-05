using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AerensStoreTest
{
    [TestFixture]
    internal class KeyValueStoreTestWithTime
    {
        private KeyValueStore _store;
        private string _filePath;
        private DateTime _now = DateTime.Now;
        string testKey = "testKey";
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testKeyValueStore.json");
        int defaultCleanUpTime = 3;
        DeltaTime _deltaTime1year = new DeltaTime(years: 1);

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _store = new KeyValueStore(path, _deltaTime1year, OverwriteSetting: true);

        }
        [TearDown]
        public void TearDown()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        [Test]
        public void KeyValueStore_WhenCreated_CreatesFile()
        {
            _store.SetInt(testKey, 123);
            Assert.That(File.Exists(path), Is.True);
        }
        [Test]
        public void Set_WhenCalled_SetsValueForKey()
        {
            _store.Set(testKey, "testValue");

            var result = _store.Get(testKey);

            Assert.That(result, Is.EqualTo("testValue"));
        }
        [Test]
        public void Get_WhenNoDataIsSet_ReturnsNull()
        {
            var result = _store.Get("nonExistingTestKey");

            Assert.That(result, Is.Null);
        }
        [Test]
        public void Get_WhenOlderMonthIsWithinDeltaTime_ReturnsOldValue()
        {
            string date = _now.AddMonths(-1).ToString("yyyyMM");
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(months: 3));
            var result = _store.Get(testKey);
            Assert.That(result, Is.EqualTo(value));
        }
        [Test]
        public void Get_WhenOlderMonthIsntWithinDeltaTimeMonth_ReturnsNull()
        {
            string date = _now.AddMonths(-5).ToString("yyyyMM");
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(months: 3));
            var result = _store.Get(testKey);
            Assert.That(result, Is.Null);
        }
        [Test]
        public void Get_KeyFromNonExistingDateWithLookupdate_ReturnsNull()
        {
            string date = _now.AddYears(-5).ToString("yyyy");
            DateTime getDate = _now;
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(years: 3));
            var result = _store.Get(testKey, lookupDate: getDate);
            Assert.That(result, Is.Null);
        }
        [Test]
        public void Get_OldKeyWithLookupdate_ReturnsValue()
        {
            string date = _now.AddYears(-5).ToString("yyyy");
            DateTime getDate = _now.AddYears(-5).AddMonths(2);
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(years: 1));
            var result = _store.Get(testKey, lookupDate: getDate);
            Assert.That(result, Is.EqualTo(value));
        }
        [Test]
        public void Get_KeyFromNonExistingDateWithStraightlookup_ReturnsNull()
        {
            string date = _now.AddYears(-5).ToString("yyyy");
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(years: 3));
            string straightKey = testKey + _now.ToString("yyyy");
            var result = _store.Get(straightKey, straightLookup: true);
            Assert.That(result, Is.Null);
        }
        [Test]
        public void Get_OldKeyStraightlookup_ReturnsValue()
        {
            string date = _now.AddYears(-5).ToString("yyyy");
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(years: 1));
            string straightKey = testKey + date;
            var result = _store.Get(straightKey, straightLookup: true);
            Assert.That(result, Is.EqualTo(value));
        }
        [Test]
        public void Set_OverwritesKeyWithinDeltaTimeMonth_SetsValueForKey()
        {
            string date = _now.AddMonths(-1).ToString("yyyyMM");
            string wrongValue = "OldValue";
            CreateNewStore(date, wrongValue, path, new DeltaTime(months: 3));
            string correctValue = "CorrectValue";
            _store.Set(testKey, correctValue);

            var result = _store.Get(testKey);
            Assert.That(result, Is.EqualTo(correctValue));
        }
        [Test]
        public void Get_WhenOlderMonthIsntWithinDeltaTimeHour_ReturnsNull()
        {
            string date = _now.AddHours(-5).ToString("yyyyMMDDHH");
            string value = "testValue";
            CreateNewStore(date, value, path, new DeltaTime(hours: 3));
            var result = _store.Get(testKey);
            Assert.That(result, Is.Null);
        }
        [Test]
        public void Set_OverwritesKeyWithinDeltaTimeHour_SetsValueForKey()
        {
            string date = _now.AddHours(-1).ToString("yyyyMMDDHH");
            string wrongValue = "OldValue";
            CreateNewStore(date, wrongValue, path, new DeltaTime(hours: 3));
            string correctValue = "CorrectValue";
            _store.Set(testKey, correctValue);

            var result = _store.Get(testKey);
            Assert.That(result, Is.EqualTo(correctValue));
        }
        [Test]
        public void Get_OldKeyWithIntervalHours_ReturnsOldValue()
        {
            string date = _now.AddMonths(-5).ToString("yyyyMM");
            string oldValue = "oldTestValue";
            string newValue = "newTestValue";
            CreateNewStore(date, oldValue, path, new DeltaTime(months: 3));
            _store.Set(testKey, newValue);
            var newResult = _store.Get(testKey);
            var oldResult = _store.Get(testKey, 1);
            Assert.That(newResult, Is.EqualTo(newValue));
            Assert.That(oldResult, Is.EqualTo(oldValue));
        }
        [Test]
        public void RemoveOldKeys_WhenCalled_RemovesOldKeys()
        {
            _store = new KeyValueStore(path, new DeltaTime(months: 1), OverwriteSetting: true);
            string date = _now.AddMonths(-5).ToString("yyyyMM");
            string oldValue = "testValue";
            CreateNewStore(date, oldValue, path, new DeltaTime(months: 1));
            string newValue = "newTestValue";
            string newKeyName = "newTestKey";
            _store.Set(newKeyName, newValue);
            _store = null; // reset store
            _store = new KeyValueStore(path);
            var Oldresult = _store.Get(testKey);
            var Newresult = _store.Get(newKeyName);
            Assert.That(Oldresult, Is.Null);
            Assert.That(Newresult, Is.EqualTo(newValue));
        }
        [Test]
        public void RemoveOldKeys_WhenCalledWitCustomTime_RemovesNoKeys()
        {
            string date = _now.AddMonths(-5).ToString("yyyyMM");
            string oldValue = "oldTestValue";
            string newValue = "newTestValue";
            string newKeyName = "newTestKey";
            KeyValueStore storeDatetime = new KeyValueStore(path, new DeltaTime(months: 1), 6, OverwriteSetting: true);
            storeDatetime.Set(newKeyName, newValue);
            AddValueToJson(path, testKey + date, oldValue);
            storeDatetime = null; // reset store
            storeDatetime = new KeyValueStore(path);
            var Oldresult = storeDatetime.Get(testKey, 5);
            var Newresult = storeDatetime.Get(newKeyName);
            Assert.That(Oldresult, Is.EqualTo(oldValue));
            Assert.That(Newresult, Is.EqualTo(newValue));
        }
        [Test]
        public void SetKey_WithstraightKey_JsonContains_Key()
        {
            string straightKey = "testKey2";
            string value = "testValue";
            _store.Set(straightKey, value, straightKey: true);
            var json = File.ReadAllText(path);
            Assert.That(json.Contains(straightKey), Is.True);
        }
        [Test]
        public void GetKeys_WithDateTimeFalse_ReturnsAllKeys()
        {
            string oldkeyName = "oldTestKey2024";
            string oldkeyNameResult = "oldTestKey";
            string oldValue = "oldTestValue";
            string newValue = "newTestValue";
            string newKeyName = "newTestKey";
            KeyValueStore storeDatetime = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            storeDatetime.Set(newKeyName, newValue);
            storeDatetime.Set(oldkeyName, oldValue, true); 
            var result =storeDatetime.GetKeys();
            Assert.That(result.Contains(oldkeyNameResult), Is.True);
            Assert.That(result.Contains(newKeyName), Is.True);
        }
        [Test]
        public void GetKeys_WithDateTimeTrue_ReturnsAllKeysAndVersions()
        {
            string oldkeyName = "oldTestKey2024";
            string oldValue = "oldTestValue";
            string newValue = "newTestValue";
            string newKeyName = "newTestKey";
            string newKeyNameResult = "newTestKey" + DateTime.Now.ToString("yyyy");
            KeyValueStore storeDatetime = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true); //the setting don't really matter for this test, there just need to be settings
            storeDatetime.Set(newKeyName, newValue);
            storeDatetime.Set(oldkeyName, oldValue, true);
            var result = storeDatetime.GetKeys(true);
            Assert.That(result.Contains(newKeyNameResult), Is.True);
            Assert.That(result.Contains(oldkeyName), Is.True);
        }
        [Test]
        public void GetKeys_WithDateTimeFalseAndMixedKeys_ReturnsAllKeys()
        {
            string keyName1 = "testKey12024";
            string keyName2 = "testKey2";
            string keyName3 = "Test4";
            string keyName4 = "Testing";
            string keyName5 = "testKey2024";
            string keyName1Result = "testKey1";
            string value = "testValue";
            KeyValueStore storeDatetime = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            storeDatetime.Set(keyName1, value, true);
            storeDatetime.Set(keyName2, value);
            storeDatetime.Set(keyName3, value, true);
            storeDatetime.Set(keyName4, value, true);
            storeDatetime.Set(keyName5, value);
            var result = storeDatetime.GetKeys();
            Assert.That(result.Contains(keyName1Result), Is.True);
            Assert.That(result.Contains(keyName2), Is.True);
            Assert.That(result.Contains(keyName3), Is.True);
            Assert.That(result.Contains(keyName4), Is.True);
            Assert.That(result.Contains(keyName5), Is.True);
        }
        [Test]
        public void GetSettings_WhenCalled_ReturnsNotNull()
        {
            var result = _store.GetSettings();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public void GetSettings_WhenCalled_ReturnsSettings()
        {
            Dictionary<string, object> result = _store.GetSettings();
            string ContinuesStoreTimejson = JsonConvert.SerializeObject(result["ContinuesStoreTime"]);
            string defaultContinuesStoreTimejson = JsonConvert.SerializeObject(_deltaTime1year);
            Assert.That(result.ContainsKey("ContinuesStoreTime"), Is.True);
            Assert.That(result.ContainsKey("cleanUpTime"), Is.True);
            Assert.That(result["cleanUpTime"], Is.EqualTo(defaultCleanUpTime));
            Assert.That(ContinuesStoreTimejson, Is.EqualTo(defaultContinuesStoreTimejson));
        }
        [Test]
        public void GetKeyVersions_WhenCalled_ReturnsKeyVersions()
        {
            string Value1 = "TestValue";
            string KeyName = "TestKey";
            string KeyNameDate1 = KeyName + DateTime.Now.ToString("yyyy");
            string KeyNameDate2 = KeyName + DateTime.Now.AddYears(-1).ToString("yyyy");
            string KeyNameDate3 = KeyName + DateTime.Now.AddYears(-5).ToString("yyyy");
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            keyValueStore.Set(KeyNameDate1, Value1, true);
            keyValueStore.Set(KeyNameDate2, Value1, true);
            keyValueStore.Set(KeyNameDate3, Value1, true);
            Dictionary<int, string> result = keyValueStore.GetKeyVersions(KeyName);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.ContainsKey(0), Is.True);
            Assert.That(result.ContainsKey(1), Is.True);
            Assert.That(result.ContainsKey(5), Is.True);
            Assert.That(result[0], Is.EqualTo(DateTime.Now.ToString("yyyy")));
            Assert.That(result[1], Is.EqualTo(DateTime.Now.AddYears(-1).ToString("yyyy")));
            Assert.That(result[5], Is.EqualTo(DateTime.Now.AddYears(-5).ToString("yyyy")));
        }
        [Test]
        public void GetKeyVersions_WhenCalledWithNoKey_ReturnsEmptyDictionary()
        {
            string KeyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            Dictionary<int, string> result = keyValueStore.GetKeyVersions(KeyName);
            Assert.That(result.Count, Is.EqualTo(0));
        }
        [Test]
        public void GetKeyVersions_WhenCalledWithNoVersions_ReturnsEmptyDictionary()
        {
            string Value1 = "TestValue";
            string KeyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            keyValueStore.Set(KeyName, Value1, true);
            Dictionary<int, string> result = keyValueStore.GetKeyVersions(KeyName);
            Assert.That(result.Count, Is.EqualTo(0));
        }
        [Test]
        public void GetKeysVersions_WhenCalled_ReturnsKeyAndVersions()
        {
            string Value = "TestValue";
            string KeyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            string KeyNameDate1 = KeyName + DateTime.Now.ToString("yyyy");
            string KeyNameDate2 = KeyName + DateTime.Now.AddYears(-1).ToString("yyyy");
            string KeyNameDate3 = KeyName + DateTime.Now.AddYears(-5).ToString("yyyy");
            keyValueStore.Set(KeyNameDate1, Value, true);
            keyValueStore.Set(KeyNameDate2, Value, true);
            keyValueStore.Set(KeyNameDate3, Value, true);
            Dictionary<string, List<string>> results = keyValueStore.GetKeysVersions();
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.ContainsKey(KeyName), Is.True);
            List<string> versions = results[KeyName];
            Assert.That(versions.Count, Is.EqualTo(3));
            Assert.That(versions.Contains(DateTime.Now.ToString("yyyy")), Is.True);
            Assert.That(versions.Contains(DateTime.Now.AddYears(-1).ToString("yyyy")), Is.True);
            Assert.That(versions.Contains(DateTime.Now.AddYears(-5).ToString("yyyy")), Is.True);
        }
        [Test]
        public void GetKeysVersions_WhenCalled_ReturnsKeysAndVersions()
        {
            string Value = "TestValue";
            string KeyName1 = "TestKey";
            string KeyName2 = "KeyTest";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            string KeyName1Date1 = KeyName1 + DateTime.Now.ToString("yyyy");
            string KeyName1Date2 = KeyName1 + DateTime.Now.AddYears(-3).ToString("yyyy");
            string KeyName2Date1 = KeyName2 + DateTime.Now.ToString("yyyy");
            string KeyName2Date2 = KeyName2 + DateTime.Now.AddYears(-5).ToString("yyyy");
            keyValueStore.Set(KeyName1Date1, Value, true);
            keyValueStore.Set(KeyName1Date2, Value, true);
            keyValueStore.Set(KeyName2Date1, Value, true);
            keyValueStore.Set(KeyName2Date2, Value, true);
            Dictionary<string, List<string>> results = keyValueStore.GetKeysVersions();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.ContainsKey(KeyName1), Is.True);
            Assert.That(results.ContainsKey(KeyName2), Is.True);
            List<string> versionsKey1 = results[KeyName1];
            List<string> versionsKey2 = results[KeyName2];
            Assert.That(versionsKey1.Count, Is.EqualTo(2));
            Assert.That(versionsKey2.Count, Is.EqualTo(2));
            Assert.That(versionsKey1.Contains(DateTime.Now.ToString("yyyy")), Is.True);
            Assert.That(versionsKey1.Contains(DateTime.Now.AddYears(-3).ToString("yyyy")), Is.True);
            Assert.That(versionsKey2.Contains(DateTime.Now.ToString("yyyy")), Is.True);
            Assert.That(versionsKey2.Contains(DateTime.Now.AddYears(-5).ToString("yyyy")), Is.True);
        }
        [Test]
        public void GetKeysVersions_WhenCalledWithIncorrectVersion_GetSkipped()
        {
            string value = "TestValue";
            string keyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            keyValueStore.Set(keyName, value, true);
            keyValueStore.Set(keyName, value);
            Dictionary<string, List<string>> results = keyValueStore.GetKeysVersions();
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.ContainsKey(keyName), Is.True);
            List<string> versions = results[keyName];
            Assert.That(versions.Count, Is.EqualTo(1));
            Assert.That(versions.Contains(DateTime.Now.ToString("yyyy")), Is.True);
        }
        [Test]
        public void GetKeysVersions_WhenCalledWithOnlyIncorrectVersion_ReturnsEmptyList()
        {
            string value = "TestValue";
            string keyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            keyValueStore.Set(keyName, value, true);
            Dictionary<string, List<string>> results = keyValueStore.GetKeysVersions();
            Assert.That(results.Count, Is.EqualTo(1));
            List<string> versions = results[keyName];
            Assert.That(versions.Count, Is.EqualTo(0));
        }
        [Test]
        public void GetKeysIterations_WhenCalled_ReturnsKeyAndItterations()
        {
            string Value = "TestValue";
            string KeyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(years: 1), 6, OverwriteSetting: true);
            string KeyNameDate1 = KeyName + DateTime.Now.ToString("yyyy");
            string KeyNameDate2 = KeyName + DateTime.Now.AddYears(-1).ToString("yyyy");
            string KeyNameDate3 = KeyName + DateTime.Now.AddYears(-5).ToString("yyyy");
            keyValueStore.Set(KeyName, Value);
            keyValueStore.Set(KeyNameDate1, Value, true);
            keyValueStore.Set(KeyNameDate2, Value, true);
            keyValueStore.Set(KeyNameDate3, Value, true);
            Dictionary<string, List<int>> results = keyValueStore.GetKeysIterations();
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.ContainsKey(KeyName), Is.True);
            List<int> keysIterations = results[KeyName];
            Assert.That(keysIterations.Count, Is.EqualTo(3));
            Assert.That(keysIterations.Contains(0), Is.True);
            Assert.That(keysIterations.Contains(1), Is.True);
            Assert.That(keysIterations.Contains(5), Is.True);
        }
        [Test]
        public void GetKeysIterations_WhenCalled_WithComplexTimeDelta_ReturnKeysAndCorrectVersions()
        {
            string value = "TestValue";
            string keyName = "TestKey";
            KeyValueStore keyValueStore = new KeyValueStore(path, new DeltaTime(months: 3), 80, OverwriteSetting: true);
            string keyNameDate2 = keyName + DateTime.Now.AddMonths(-6).ToString("yyyyMM");
            keyValueStore.Set(keyName, value);
            //keyValueStore.Set(keyNameDate2, value, true);
            Dictionary<string, List<int>> results = keyValueStore.GetKeysIterations();
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.ContainsKey(keyName), Is.True);
            List<int> keysIterations = results[keyName];
            Assert.That(keysIterations.Count, Is.EqualTo(1));
            Assert.That(keysIterations.Contains(0), Is.True);
            Assert.That(keysIterations.Contains(2), Is.False);
        }

        private void CreateNewStore(string date, object value, string path, DeltaTime deltaTime)
        {

            var data = new Dictionary<string, object>
            {
                { testKey + date, value }
            };
            var newStore = new
            {
                Settings = new
                {
                    ContinuesStoreTime = deltaTime,
                    cleanUpTime = defaultCleanUpTime
                },
                Store = data
            };

            string json = JsonConvert.SerializeObject(newStore);
            File.WriteAllText(path, json);

            _store = new KeyValueStore(path);
        }
        private void AddValueToJson(string path, string key, string value)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var store = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var jsonStore = JsonConvert.SerializeObject(store["Store"]);
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStore);
                data[key] = value;
                var jsonSettings = JsonConvert.SerializeObject(store["Settings"]);
                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonSettings);
                 var newStore = new
                {
                    Settings = settings,
                    Store = data
                };
                json = JsonConvert.SerializeObject(newStore);
                File.WriteAllText(path, json);
            }
            else
            {
                var data = new Dictionary<string, object>
                {
                    { key, value }
                };
                var newStore = new
                {
                    Settings = new
                    {
                        ContinuesStoreTime = new DeltaTime(),
                        cleanUpTime = defaultCleanUpTime
                    },
                    Store = data
                };
                string json = JsonConvert.SerializeObject(newStore);
                File.WriteAllText(path, json);
            }
        }

    }
}
