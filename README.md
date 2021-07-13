# RystemV2

Nuget version: https://www.nuget.org/packages/Rystem/

### Contribute: https://gitcoin.co/grants/3127/rystem
### Contribute: https://www.buymeacoffee.com/keyserdsoze

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
To understand better how Rystem works please see Rystem.UnitTest project

### Json (Rystem.Text)
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      var json = falseNueve.ToJson();
      var falseNueve2 = json.FromJson<FalseNueve>();
 
### Bson (Rystem.Text)
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      var bson = falseNueve.ToBson();
      var falseNueve2 = bson.FromBson<FalseNueve>();
      
### Csv (Rystem.Text)
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

### Aes (Rystem.Security.Cryptography)
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
      
 ### Sha256 (Rystem.Security.Cryptography)
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.ToHash();
      string crypting2 = "Message to Hash".ToHash();
      
 ### Webrequest as fluid as possible (Rystem.Net)
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
                
 ### Try/Catch (Rystem)
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
                
### Base64 (Rystem.Text)
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.ToBase64();
      var falseNueve2 = crypting1.FromBase64<FalseNueve>();
      
### Base45 (Rystem.Text)
      var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      string crypting1 = falseNueve.ToBase45();
      var falseNueve2 = crypting1.FromBase45<FalseNueve>();
      
### Task (System.Threading.Tasks)
      Task task = new Task();
      
      //Instead to use
      task.ConfigureAwait(false);
      //Use
      task.NoContext();
      
      //Instead to use
      task.NoContext().GetAwaiter().GetResult()
      //Use
      task.ToResult();
      
### Reflection/Properties (Rystem.Reflection)
     //Simplify fetching properties
     var falseNueve = new FalseNueve()
      {
          Al = "a",
          Ol = "b"
      };
      var properties1 = falseNueve.GetType().FetchProperties();
      //after the first, the other times you call FetchProperties from an object you get properties very fast.
      var properties2 = falseNueve.GetType().FetchProperties();
     
### Random (Rystem)
      //Get true random number
      int maxValue = 60;
      int value = Alea.GetNumber(maxValue);
      //value goes from 0 to 60
      
### TimedKey (Rystem)
      //Simplify string.Format("{0:d19}{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"))
      Alea.GetTimedKey();
      
### Concurrency -> Race Condition (Rystem.Concurrency)
      //Use on a task
      Func<Task> action = async () => await CountAsync(v);
      //and Run under Race Condition
      List<Task> tasks = new List<Task>();
      for (int i = 0; i < 20; i++)
      {
          tasks.Add(action.RunUnderRaceConditionAsync(key));
      }
      await Task.WhenAll(tasks);
      //only the first method really runs.
      
      //you may use also the RaceCondition static class
      await RaceCondition.RunAsync(action, key);
      
### Concurrency -> Lock (Rystem.Concurrency)
      //Use on a task
      Func<Task> action = async () => await CountAsync(v);
      //and Run under Lock
      List<Task> tasks = new List<Task>();
      for (int i = 0; i < 20; i++)
      {
          tasks.Add(Execute());
      }
      await Task.WhenAll(tasks);
      async Task Execute()
      {
          var x = await action.LockAsync(key);
          if (x.InException)
              Error++;
      };
      //all method runs in a queue based on the key.
      
      //you may also use
      await Lock.RunAsync(action, key);
      
### Thread -> BackgroundWork (Rystem.Background)
      //Run in background a task continuously
      //In this example we are running as Task id 3 a method CountAsync(2) continuously every 300 milliseconds.
      Action action = async () => await CountAsync(2);
      action.RunInBackground("3", 300);
      await Task.Delay(1200);
      action.StopRunningInBackground("3");
      
      //you may also use
      BackgroundWork.Run(async () => await CountAsync(2), "3", 300);
      await Task.Delay(1200);
      Ghost.Stop("3");
      
      //you may also use IBackgroundWork
      private class MyFirstBackgroundWork : IBackgroundWork
      {
            public async Task ActionToDo()
            {
                //something to do
            }
            public bool RunImmediately => true;
            public string Cron => "* * * * * *";
      }
      
      //with IServiceCollection
      (Instance of IServiceCollection).AddBackgroundWork<MyFirstBackgroundWork>();
      
      //or with the specific method Run()
      (Instance of MyFirstBackgroundWork).Run();
      
### Thread -> Sequence (Rystem.Background)
      //Manage a queue with a T object to allow a batch operation after a maximum buffer elements or a maximum retention of time. 
      //Before install the Aes Password, SaltKey, IVKey in a static constructor or during startup.
       static EnqueueSample()
       {
            Sequence.Create<IFalseNueve>(500, TimeSpan.FromSeconds(2), Evaluate, QueueName, QueueType.FirstInFirstOut);
       }
       
       //Use with
       var falseNueve = new FalseNueve()
       {
            Al = "a",
            Ol = "b"
       };
       falseNueve.Enqueue(QueueName);
       //Force the batch (if you need to force)
       Sequence.Flush(QueueName, true);
       //Destroy the queue
       Sequence.Destroy(QueueName);
