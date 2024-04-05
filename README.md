KeyValuePairStore

KeyValuePairStore is a project that provides a key-value store for storing data. The data is stored in a file and can be of any type that can be serialized to JSON.

## Classes

### `DeltaTime`

The `DeltaTime` class represents a time span with properties for years, months, days, and hours. This class is a support class for `KeyValueStore` where it determines the timedelta used for versioning of the keypairs. The `DeltaTime` class has the following properties:

- `Years`: Represents the number of years in the time span.
- `Months`: Represents the number of months in the time span.
- `Days`: Represents the number of days in the time span.
- `Hours`: Represents the number of hours in the time span.
- `IsOff`: A boolean value that indicates whether the time span is turned off. This property is set automatically and does not require user input.

### `KeyValueStore`

The `KeyValueStore` class provides a key-value store for storing data. It allows you to set and get values by key. The values are stored in a file and can be of any type that can be serialized to JSON. The `KeyValueStore` class has the following key methods:

- `Set`: This method allows you to set a value for a given key. The value is serialized to JSON and stored in a file.
- `Get`: This method allows you to get the value for a given key. The value is deserialized from JSON.

The `KeyValueStore` constructor does not require any parameters. However, it does have optional parameters:

- `filePath`: If provided, this is where the key-value pairs and settings will be/are stored.
- `continuesStoreTime`: A `DeltaTime` object, this is the time span used for versioning of the keyvaluepairs.
- `cleanUpTime`: An integer that specifies how many iterations to keep a version of a key before it gets removed at clean up.
- `OverwriteSetting`: A boolean that determines whether existing settings in the json should be overwritten.

If these parameters are not provided, the `KeyValueStore` will use default values.

Next to `Get` and `Set` there are also the following methods:
| Method            | Parameters                                                                 | Output                           | Description                                                                                                                                                                                                                                           |
| ----------------- | -------------------------------------------------------------------------- | -------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| SetString         | string key, string value, bool straightKey*                                | none                             | This set a string in the KeyValueStore, straightKey put the plain keyName in the store without any time processing                                                                                                                                    |
| SetInt            | string key, int value, bool straightKey*                                   | none                             | This set a int in the KeyValueStore, straightKey put the plain keyName in the store without any time processing                                                                                                                                       |
| SetDouble         | string key, double value, bool straightKey*                                | none                             | This set a double in the KeyValueStore, straightKey put the plain keyName in the store without any time processing                                                                                                                                    |
| SetLong           | string key, long value, bool straightKey*                                  | none                             | This set a long in the KeyValueStore, straightKey put the plain keyName in the store without any time processing                                                                                                                                      |
| SetChar           | string key, char value, bool straightKey*                                  | none                             | This set a char in the KeyValueStore, straightKey put the plain keyName in the store without any time processing                                                                                                                                      |
| SetBool           | string key, bool value, bool straightKey*                                  | none                             | This set a bool in the KeyValueStore, straightKey put the plain keyName in the store without any time processing                                                                                                                                      |
| GetString         | string key, int iterationsAgo*, DateTime lookupDate*, bool straightLookup* | string                           | This get a string from the KeyValueStore, IterationsAgo gives you how many DeltaTimes old you want this Value to be, lookupDate is the startdate for this IterationsAgo and straightLookup ignores all setting and just get it based on the plain key |
| GetInt            | string key, int iterationsAgo*, DateTime lookupDate*, bool straightLookup* | int                              | This get a int from the KeyValueStore, IterationsAgo gives you how many DeltaTimes old you want this Value to be, lookupDate is the startdate for this IterationsAgo and straightLookup ignores all setting and just get it based on the plain key    |
| GetDouble         | string key, int iterationsAgo*, DateTime lookupDate*, bool straightLookup* | double                           | This get a double from the KeyValueStore, IterationsAgo gives you how many DeltaTimes old you want this Value to be, lookupDate is the startdate for this IterationsAgo and straightLookup ignores all setting and just get it based on the plain key |
| GetLong           | string key, int iterationsAgo*, DateTime lookupDate*, bool straightLookup* | long                             | This get a long from the KeyValueStore, IterationsAgo gives you how many DeltaTimes old you want this Value to be, lookupDate is the startdate for this IterationsAgo and straightLookup ignores all setting and just get it based on the plain key   |
| GetChar           | string key, int iterationsAgo*, DateTime lookupDate*, bool straightLookup* | char                             | This get a char from the KeyValueStore, IterationsAgo gives you how many DeltaTimes old you want this Value to be, lookupDate is the startdate for this IterationsAgo and straightLookup ignores all setting and just get it based on the plain key   |
| GetBool           | string key, int iterationsAgo*, DateTime lookupDate*, bool straightLookup* | bool                             | This get a bool from the KeyValueStore, IterationsAgo gives you how many DeltaTimes old you want this Value to be, lookupDate is the startdate for this IterationsAgo and straightLookup ignores all setting and just get it based on the plain key   |
| GetKeys           | bool WithDateTime*                                                         | List<string>                     | This get all the keys in the KeyValueStore, If WithdateTime is true you also get all the date version like Key2024 & Key2023 instead of only key                                                                                                      |
| GetStore          | none                                                                       | Dictionary<string, object>       | This just returns the store itself                                                                                                                                                                                                                    |
| GetSettings       | none                                                                       | Dictionary<string, object>       | This just returns the settings themselves                                                                                                                                                                                                             |
| GetKeyVersion     | string key                                                                 | Dictionary<int, string>          | This returns a dictionary with the iteration back and the corresponding date (in string) for the given key                                                                                                                                            |
| GetKeysVersions   | none                                                                       | Dictionary<string, List<string>> | This returns a dictionary with all the keys and their corresponding dates (in string)                                                                                                                                                                 |
| GetKeysIterations | none                                                                       | Dictionary<string, List<int>>    | This returns a dictionary with all the keys and their corresponding iterations back                                                                                                                                                                   |

:bulb: **note:** * Means that the parameter is optional.
## Getting Started

You can get started with the KeyValuePairStore project by installing the NuGet package. You can do this by running the following command in the Package Manager Console:

```powershell
paket add KeyValuePairStore --version 1.2.0
```

After this basic usage is very simple, here a simple demo:

```csharp
KeyValueStore keyValueStore = new KeyValueStore();
int index = keyValueStore.GetInt("demokey");
index++;
keyValueStore.SetInt("demokey", index);
Console.WriteLine("Value of demokey: " + index);
```

the first output will be 1, the second time you run the code it will be 2, and so on. The value is stored in a file and will be kept even if you close the application.

> :bulb:  **note:** if you get a key that does not exist it wil return 0 (or the equivalent for the type requested).

#### more advanced demo with Deltatime

this is a more advanced demo where we use the `DeltaTime` class to create a yearly report. For this report we have a few event that increment a counter, and we want to keep track of the yearly count. Or store some data.

```csharp
DeltaTime deltaTime = new DeltaTime(1, 0, 0, 0);
KeyValueStore keyValueStore = new KeyValueStore(continuesStoreTime: deltaTime, cleanUpTime: 5); // clean up data older then 5 years (aka 5 * DeltaTime)
int FileProcessCount = keyValueStore.GetInt("FileProcessCount");
FileProcessCount++;
keyValueStore.SetInt("FileProcessCount", FileProcessCount);
int SomeRandomData = Random.Next(0, 100);
keyValueStore.SetInt("SomeRandomData", SomeRandomData);
// now for the data of this year so far
Console.WriteLine("FileProcessCount: " + keyValueStore.GetInt("FileProcessCount"));
Console.WriteLine("SomeRandomData: " + keyValueStore.GetInt("SomeRandomData"));
// now for the data of last year
Console.WriteLine("FileProcessCount last year: " + keyValueStore.GetInt("FileProcessCount", 1));
Console.WriteLine("SomeRandomData last year: " + keyValueStore.GetInt("SomeRandomData", 1));
```

:bulb: **note:** All get functions will return a **InvalidCastException** if the data is not of the type requested, exept the **char** this will convert ints to char folling the Convert.ToChar() documentation. 

## Contributing

Contributions to the KeyValuePairStore project are welcome. If you have a feature request, bug report, or want to contribute code, please open an issue or pull request on the GitHub repository.

## License

The KeyValuePairStore project is licensed under the terms of the license included in the `LICENSE.txt` file.