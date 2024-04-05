using Newtonsoft.Json;
using System.IO;
using System.Linq;
namespace AerensStoreTest
{

    [TestFixture]
    public class KeyValueStoreTestsWithoutTime
    {
        private KeyValueStore _store;
        private string _filePath;
        private KeyValueStore _noParameterStore;
        private string _baseFilePath;

        [SetUp]
        public void SetUp()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testKeyValueStore.json");
            _baseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keyValueStore.json");
            _store = new KeyValueStore(_filePath);
            _noParameterStore = new KeyValueStore();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
            if (File.Exists(_baseFilePath))
            {
                File.Delete(_baseFilePath);
            }
        }

        [Test]
        public void KeyValueStore_WhenCreated_CreatesFile()
        {
            _store.SetInt("testKey", 123);
            Assert.That(File.Exists(_filePath), Is.True);
        }
        [Test]
        public void KeyValueStore_WhenCreated_CreatesFileNoParameters()
        {
            _noParameterStore.SetInt("testKey", 123);
            Assert.That(File.Exists(_baseFilePath), Is.True);
        }

        [Test]
        public void Set_WhenCalled_SetsValueForKey()
        {
            _store.Set("testKey", "testValue");

            var result = _store.Get("testKey");

            Assert.That(result, Is.EqualTo("testValue"));
        }

        [Test]
        public void SetString_WhenCalled_SetsStringValueForKey()
        {
            _store.SetString("testKey", "testValue");

            var result = _store.GetString("testKey");

            Assert.That(result, Is.EqualTo("testValue"));
        }

        [Test]
        public void SetBool_WhenCalled_SetsBoolValueForKey()
        {
            _store.SetBool("testKey", true);

            Assert.That(_store.GetBool("testKey"), Is.True);
        }

        [Test]
        public void SetChar_WhenCalled_SetsCharValueForKey()
        {
            _store.SetChar("testKey", 'a');

            Assert.That(_store.GetChar("testKey"), Is.EqualTo('a'));
        }

        [Test]
        public void SetLong_WhenCalled_SetsLongValueForKey()
        {
            _store.SetLong("testKey", 1234567890123456789L);

            Assert.That(_store.GetLong("testKey"), Is.EqualTo(1234567890123456789L));
        }

        [Test]
        public void SetDouble_WhenCalled_SetsDoubleValueForKey()
        {
            _store.SetDouble("testKey", 123.456);

            Assert.That(_store.GetDouble("testKey"), Is.EqualTo(123.456));
        }

        [Test]
        public void SetInt_WhenCalled_SetsIntValueForKey()
        {
            _store.SetInt("testKey", 123);

            Assert.That(_store.GetInt("testKey"), Is.EqualTo(123));
        }

        [Test]
        public void GetString_WhenValueCannotBeConvertedToString_ThrowsInvalidCastException()
        {
            _store.Set("testKey", 123);

            Assert.Throws<InvalidCastException>(() => _store.GetString("testKey"));
        }

        [Test]
        public void GetBool_WhenValueCannotBeConvertedToBool_ThrowsInvalidCastException()
        {
            _store.Set("testKey", "not a bool");

            Assert.Throws<InvalidCastException>(() => _store.GetBool("testKey"));
        }

        [Test]
        public void GetLong_WhenValueCannotBeConvertedToLong_ThrowsInvalidCastException()
        {
            _store.Set("testKey", "not a long");

            Assert.Throws<InvalidCastException>(() => _store.GetLong("testKey"));
        }

        [Test]
        public void GetDouble_WhenValueCannotBeConvertedToDouble_ThrowsInvalidCastException()
        {
            _store.Set("testKey", "not a double");

            Assert.Throws<InvalidCastException>(() => _store.GetDouble("testKey"));
        }

        [Test]
        public void GetInt_WhenValueCannotBeConvertedToInt_ThrowsInvalidCastException()
        {
            _store.Set("testKey", "not an int");

            Assert.Throws<InvalidCastException>(() => _store.GetInt("testKey"));
        }

        [Test]
        public void Get_WhenNoDataIsSet_ReturnsNull()
        {
            var result = _noParameterStore.Get("testKey");

            Assert.That(result, Is.Null);
        }
        [Test]
        public void GetInt_WhenNoDataIsSet_ReturnsZero()
        {
            var result = _noParameterStore.GetInt("testKey");

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetString_WhenNoDataIsSet_ReturnsNull()
        {
            var result = _noParameterStore.GetString("testKey");

            Assert.That(result, Is.EqualTo(string.Empty));
        }
        [Test]
        public void GetDouble_WhenNoDataIsSet_ReturnsZero()
        {
            var result = _noParameterStore.GetDouble("testKey");

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetLong_WhenNoDataIsSet_ReturnsZero()
        {
            var result = _noParameterStore.GetLong("testKey");

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetChar_WhenNoDataIsSet_ReturnsNullChar()
        {
            var result = _noParameterStore.GetChar("testKey");

            Assert.That(result, Is.EqualTo('\0'));
        }

        [Test]
        public void GetBool_WhenNoDataIsSet_ReturnsFalse()
        {
            var result = _noParameterStore.GetBool("testKey");

            Assert.That(result, Is.False);
        }
        [Test]
        public void Save_WhenCalled_WritesToFile()
        {
            // Arrange
            _store.Set("key", "value");

            // Assert
            var content = File.ReadAllText(_filePath);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            Assert.That(data.ContainsKey("Store"));
            var storeData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data["Store"].ToString());
            Assert.That(storeData.ContainsKey("key"));
            Assert.That(storeData["key"], Is.EqualTo("value"));
        }
        [Test]
        public void Save_WhenStoreIsDeleted_ReturnValue()
        {
            _store.Set("key", "value");
            _store = null;
            _store = new KeyValueStore(_filePath);
            var result = _store.Get("key");
            Assert.That(result, Is.EqualTo("value"));
        }

        [Test]
        public void Set_WhenCalled_SetsValueForKey_RecreatedStore()
        {
            _store.Set("testKey", "testValue");
            _store = null;
            _store = new KeyValueStore(_filePath);

            var result = _store.Get("testKey");

            Assert.That(result, Is.EqualTo("testValue"));
        }

        [Test]
        public void SetString_WhenCalled_SetsStringValueForKey_RecreatedStore()
        {
            _store.SetString("testKey", "testValue");
            _store = null;
            _store = new KeyValueStore(_filePath);

            var result = _store.GetString("testKey");

            Assert.That(result, Is.EqualTo("testValue"));
        }

        [Test]
        public void SetBool_WhenCalled_SetsBoolValueForKey_RecreatedStore()
        {
            _store.SetBool("testKey", true);
            _store = null;
            _store = new KeyValueStore(_filePath);

            Assert.That(_store.GetBool("testKey"), Is.True);
        }

        [Test]
        public void SetChar_WhenCalled_SetsCharValueForKey_RecreatedStore()
        {
            _store.SetChar("testKey", 'a');
            _store = null;
            _store = new KeyValueStore(_filePath);

            Assert.That(_store.GetChar("testKey"), Is.EqualTo('a'));
        }

        [Test]
        public void SetLong_WhenCalled_SetsLongValueForKey_RecreatedStore()
        {
            _store.SetLong("testKey", 1234567890123456789L);
            _store = null;
            _store = new KeyValueStore(_filePath);

            Assert.That(_store.GetLong("testKey"), Is.EqualTo(1234567890123456789L));
        }

        [Test]
        public void SetDouble_WhenCalled_SetsDoubleValueForKey_RecreatedStore()
        {
            _store.SetDouble("testKey", 123.456);
            _store = null;
            _store = new KeyValueStore(_filePath);

            Assert.That(_store.GetDouble("testKey"), Is.EqualTo(123.456));
        }

        [Test]
        public void SetInt_WhenCalled_SetsIntValueForKey_RecreatedStore()
        {
            _store.SetInt("testKey", 123);
            _store = null;
            _store = new KeyValueStore(_filePath);

            Assert.That(_store.GetInt("testKey"), Is.EqualTo(123));
        }

        [Test]
        public void GetKeys_WithDateTimeFalse_ReturnsAllKeys()
        {
            _store.Set("key1", "value1");
            _store.Set("key2", "value2");
            _store.Set("key3", "value3");

            List<string> keys = _store.GetKeys(false);

            Assert.That(3, Is.EqualTo(keys.Count));
            Assert.That(keys, Contains.Item("key1"));
            Assert.That(keys, Contains.Item("key2"));
            Assert.That(keys, Contains.Item("key3"));
        }

        [Test]
        public void GetKeys_WithDateTimeTrue_ReturnsAllKeys()
        {
            _store.Set("key1", "value1");
            _store.Set("key2", "value2");
            _store.Set("key3", "value3");

            List<string> keys = _store.GetKeys(true);

            Assert.That(3, Is.EqualTo(keys.Count));
            Assert.That(keys, Contains.Item("key1"));
            Assert.That(keys, Contains.Item("key2"));
            Assert.That(keys, Contains.Item("key3"));
        }

        [Test]
        public void GetKeys_WithDateTimeFalse_ReturnsAllKeys_RecreatedStore()
        {
            _store.Set("key1", "value1");
            _store.Set("key2", "value2");
            _store.Set("key3", "value3");

            _store = null;
            _store = new KeyValueStore(_filePath);
            List<string> keys = _store.GetKeys(false);

            Assert.That(3, Is.EqualTo(keys.Count));
            Assert.That(keys, Contains.Item("key1"));
            Assert.That(keys, Contains.Item("key2"));
            Assert.That(keys, Contains.Item("key3"));
        }

        [Test]
        public void GetStore_ReturnsNonNullDictionary()
        {
            Dictionary<string, object> result = _store.GetStore();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetStore_ReturnsDictionaryWithCorrectData()
        {
            _store.Set("key1", "value1");
            _store.Set("key2", "value2");
            _store.Set("key3", "value3");

            Dictionary<string, object> result = _store.GetStore();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result["key1"], Is.EqualTo("value1"));
            Assert.That(result["key2"], Is.EqualTo("value2"));
            Assert.That(result["key3"], Is.EqualTo("value3"));
        }

        [Test]
        public void GetKeyVersions_WhenCalled_WithoutDeltaTime_ReturnsNull()
        {
            _store.Set("key1", "value1");
            Dictionary<int, string> result = new Dictionary<int, string>();
            result = _store.GetKeyVersions("key1");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetKeysVersions_WhenCalled_WithoutDeltaTime_ReturnsDictionaryWithKeyAndNull()
        {
            _store.Set("key1", "value1");
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            result = _store.GetKeysVersions();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result["key1"], Is.Null);
        }
        [Test]
        public void GetSettings_WhenCalled_WithoutDeltaTime_ReturnsNull()
        {
            var result = _store.GetSettings();
            Assert.That(result, Is.Null);
        }
        [Test]
        public void GetKeysIterations_WhenCalled_WithoutDeltaTime_ReturnsNull()
        {
            _store = null;
            _store = new KeyValueStore();
            var result = _store.GetKeysIterations();
            Assert.That(result, Is.Null);
        }
    }
}
