# RystemV2

**This library allows to speed up the basic operations on system libraries like:**
- added directly to string the methods to transform to json, csv, bson, byte array, stream and viceversa.
- added directly to Uri object the possibility to compose in fluid method the request.
- added a different approach to catch the exceptions.
- added a real random, and a guid with time-inverse approach.
- added a NoContext() method to Task class to perform a quick ConfigureAwait(false)
- added a ToResult() method to Task class to perform a quick ConfigureAwait(false).GetAwaiter().GetResult()
- added a fast crypting with Aes and fast hash with Sha256
- added a fast ToBase45() and ToBase64() method and their viceversa FromBase45() and FromBase64()

## Methods

### Json
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      var json = falseNueve.ToJson();
      var falseNueve2 = json.FromJson<FalseNueve>();
 
### Bson
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      var bson = falseNueve.ToBson();
      var falseNueve2 = bson.FromBson<FalseNueve>();
      
### Csv
      List<CsvModel> csvs = new List<CsvModel>();
      for (int i = 0; i < 100; i++)
          csvs.Add(new CsvModel()
          {
              Name = "Ale",
              Babel = "dsjakld,dsjakdljsa\",\"dsakdljsa\",dsadksa;dl,\",\",",
              Hotel = i,
              Value = 33D,
              Nothing = 34
          });
      string firstCsv = csvs.ToCsv();
      List<CsvModel> csvsComparer = firstCsv.FromCsv<CsvModel>().ToList();

### Aes
      //Before install the Aes Password, SaltKey, IVKey in a static constructor or during startup.
       static Crypting()
        {
            CryptingExtensions.Aes.Configure("4a4a4a4a", "4a4a4a4a", "4a4a4a4a4a4a4a4a");
        }
      //After that use it everywhere
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.Encrypt();
      var falseNueve2 = crypting1.Decrypt<FalseNueve>();
      string crypting2 = "Message to Encrypt".Encrypt();
      string decrypting2 = crypting2.Decrypt();
      
 ### Sha256
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.ToHash();
      string crypting2 = "Message to Hash".ToHash();
      
 ### Webrequest as fluid as possible
      //simple request
      string response = await new Uri("https://www.google.com")
                .CreateHttpRequest()
                .Build()
                .InvokeAsync();
      //response with a json
      var response = await new Uri("https://jsonplaceholder.typicode.com/todos/1")
                .CreateHttpRequest()
                .Build()
                .InvokeAsync<Rootobject>();
                
 ### Try/Catch
      await Try.Execute(async () =>
            {
                await Task.Delay(10);
                throw new ArgumentException("aaaaa");
            })
                .Catch<ArgumentException>(async x =>
                {
                    await Task.Delay(10);
                })
                .InvokeAsync();
                
### Base64
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.ToBase64();
      var falseNueve2 = crypting1.FromBase64<FalseNueve>();
      
### Base45
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.ToBase45();
      var falseNueve2 = crypting1.FromBase45<FalseNueve>();
      
### Task
      Task task = new Task();
      
      //Instead to use
      task.ConfigureAwait(false);
      //Use
      task.NoContext();
      
      //Instead to use
      task.NoContext().GetAwaiter().GetResult()
      //Use
      task.ToResult();
      
### Reflection/Properties
     //Simplify fetching properties
     var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      var properties1 = falseNueve.GetType().FetchProperties();
      //after the first, the other times you call FetchProperties from an object you get properties very fast.
      var properties2 = falseNueve.GetType().FetchProperties();
     
### Random
      //Get true random number
      int maxValue = 60;
      int value = Alea.GetNumber(maxValue);
      //value goes from 0 to 60
      
### TimedKey
      //Simplify string.Format("{0:d19}{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"))
      Alea.GetTimedKey();
      
      
