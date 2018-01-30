# Opine
Opine CQRS and Event Sourcing framework for C# and .NET Core

## Getting started

### Cloning and building

Clone the repository. Use the `dotnet build` command to build the projects.

```
git clone https://github.com/cpersona/Opine
cd Opine
dotnet build
```

## Motivation

### If you are a beginner...

If you are new to Domain Driven Design (DDD), Command Query Responsibility Segregation (CQRS), or Event Sourcing (ES), I would urge you to view one of the many videos available on YouTube and other services on the topic. One quick and simple resource is the [CQRS FAQ](http://cqrs.nu/Faq). See the full resources list in the **Resources** section.

Beginners in any coding discipline often start by looking for a reference implementation. In the world of CQRS, beginners are often met with, "Build your own". This is good advice. You can create a "Hello World" example quite easily without the help of any framework or library. For this reason, the concepts within CQRS can seem deceptively simple at first so building a CQRS-based system can help to highlight the sublties and common pitfalls of the architecture. 

Opine is a reference implementation for a CQRS based system. The code is relatively simple and straightforward. A beginner to CQRS can use Opine as a stepping stone for building their own system. Additionally, it can be helpful to see a system running end to end with relatively little complexity. 

### If you've done this before...

Building a robust CQRS system can be a considerable time investment and building one for the second or third time can be detrimental once the concepts are understood. 

Opine was created to provide a slightly opinionated, but flexible system that allows you to leverage your knowledge of CQRS, and Event Sourcing while giving you the flexibility to take one without the other. 

## Core Concepts
The following sections describe how CQRS/ES concepts are mapped in Opine.

### Domain Driven Design

#### Aggregates
The core of Opine is the `IAggregate` interface. This interface, and the generic version, `IAggregate<T>` give the framework access to the Aggregate's version, root object, and new events. The default implementations, `Aggregate` and `EventSourcedAggregate` provide standard functionality for state sourced and event sourced Aggregates, respectively. More detail on this can be found in the **State** section.

#### Events and Commands
Events in Opine are required to implement the `IEvent` marker interface. Commands in Opine are required to implement the `ICommand` marker interface. Opine uses the marker interface to ensure that a Command has only one handler while Events can have many handlers. 

Messages are stored in an `IMessageStore` instance.

```C#
public class NameChanged : IEvent 
{
    // Required for serialization
    protected NameChanged() { } 
    
    public NameChanged(string name)
    {
        this.Name = name;
    }
    
    public string Name { get; set; }
}

public class ChangeName : ICommand 
{
    // Required for serialization
    protected ChangeName() { } 
    
    public ChangeName(string name)
    {
        this.Name = name;
    }
    
    public string Name { get; set; }
}
```

#### Repositories
Repositories in Opine implement the `IRepository` interface which provides methods for loading and saving an Aggregate. Opine provides default implementations for `StateSourcedRepository` and `EventSourcedRepository` for state sourced and event sourced Aggregates, respectively. More detail on this can be found in the **State** section.

#### Handlers
By default, Handlers are classes marked with the `HandlerClassAttribute`. Handler methods must be marked with the `HandlerMethodAttribute`. Handlers are registered with a global handler map (Message.Type -> Handler). The framework uses the default `HandlerFinder` instance to identify handler methods by using the two attributes, but users can provide a different `IHandlerFinder` implementation if needed. 

The signature of a handler depends on the type of dispatching that is used, whether _Static_ or _Dynamic_. More detail on this can be found in the **Dispatching** section.  

#### Processes (or Sagas)
Processes in Opine are implemented by marking classes with the `HandlerClassAttribute` and providing a ProcessCode value. Each Process type has a unique ProcessCode. As with other event handlers, process handler methods must be marked with `HandlerMethodAttribute`. Certain events with either start or resume a process. Those events will typically not have the correct ProcessCode. For these event handlers, `HandlerMethodAttribute.IsProcessStart` should be set to true. 

### State
When loading or rehydrating Aggregates, there are two options: Event Sourcing and State Sourcing.

#### State Sourcing
State sourcing in Opine is enabled by using the `StateSourcedRepository` in conjunction with `IAggregateLoader<T>`, which serves as a way of restoring the state of an Aggregate from arbitrary storage such as relational or non-relational databases. State sourced Aggregates inherit from `Aggregate<T>` and provide a constructor that a root instance (state) and a version number.

```C#
// Root entity type
public class User 
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

// Loader to load root from EF context
public class UserAggregateLoader : IAggregateLoader<User>
{
    private MyContext db;
    
    public UserAggregateLoader(MyContext db) 
    {
        this.db = db;
    }
    
    public async Task<IAggregate> Load(Type type, object id) 
    {
        Check.IsOfType<User>(type);
        return await Load(id);
    }
    
    public async Task<User> Load(object id) 
    {
        // Return the user by id or a new instance with the given id. 
        // NOTE: We always return an instance
        return (await db.Users.Where(x => x.Id == id).FirstOrDefaultAsync()) ?? new User { Id = id };
    }
}

// State sourced aggregate
public class UserAggregate : Aggregate<User> 
{
    public UserAggregate(User root, int version) 
        : base(root, version)
    {
        
    }
    
    public void ChangeName(string name)
    {
        if (Root.Name != name)
        {
            // Act directly on the root and emit the event as well
            Root.Name = name;
            Emit(new NameChanged(name));
        }
    }
}
```

#### Event Sourcing
Event sourcing in Opine is enabled by using the `EventSourcedRepository` in conjunction with aggregates that inherit `EventSourcedAggregateBase<T>` and `EventSourcedAggregate<T>`. Event sourced Aggregates must provide a constructor that accepts a root instance (snapshot), a version number, and a sequence of events to source from. 

Classes that inherit from `EventSourcedAggregateBase<T>` can define a simple case statement to handle event sourcing.

```C#
// Root entity type (can be a non-EF POCO)
public class User 
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

// Event sourced aggregate
public class UserAggregate : EventSourcedAggregateBase<User> 
{
    // Constructor has required format
    public UserAggregate(User root, int version, IEnumerable<IEvent> events) 
        : base(root, version, events)
    {

    }
    
    public override void Apply(IEvent e)
    {
        switch (e) 
        {
            case NameChanged evt:
                OnNameChanged(evt);
                break;
            ...
        }
    }
    
    private void OnNameChanged(NameChanged e)
    {
        // Update name on root
        Root.Name = name;
    }
    
    public void ChangeName(string name)
    {
        if (Root.Name != name)
        {
            // Emit the event (OnNameChanged will be called by base class)
            Emit(new NameChanged(name));
        }
    }
}
```

If using a case statement is not preferred, then the `EventSourcedAggregate<T>` can be used. Event handlers are registered in the constructor of the class utilizing helper methods declare in the base class. 

```C#
// Root entity type (can be a non-EF POCO)
public class User 
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

// Event sourced aggregate
public class UserAggregate : EventSourcedAggregate<User> 
{
    // Constructor has required format
    public UserAggregate(User root, int version, IEnumerable<IEvent> events) 
        : base(root, version)
    {
        // Register event sourcing transform methods (When NameChanged then call OnNameChanged)
        When<NameChanged>(OnNameChanged);
        // Apply all of the given events
        ApplyAll(events);
    }
    
    private void OnNameChanged(NameChanged e)
    {
        // Update name on root
        Root.Name = name;
    }
    
    public void ChangeName(string name)
    {
        if (Root.Name != name)
        {
            // Emit the event (OnNameChanged will be called by base class)
            Emit(new NameChanged(name));
        }
    }
}
```


##### Snapshots
Snapshots are stored in an `ISnapshotStore`. A snapshot is created after an Aggregate's events are saved to the event store. When the Aggregate is loaded, the Snapshot for the Aggregate is loaded and used as the root instance if it is available. 

### Dispatching
By default, Opine provides two strategies for dispatching messages (Events and Commands) to Handlers. New implementations of `IDispatcher` can be created if needed. 

#### Static Dispatching
Static dispatching is enabled by using the `StaticDispatcher` class. Handler classes must implement `IStaticHandler` which provides a single Handler method for the framework to call. This form of dispatching will have better performance than dynamic dispatching, but requires more work in the handler. For example, the handler class must load the Aggregate before handling a command and then save the Aggregate after handling the command. 

One can create a switch statement in the single Handle method in order to handle the various events or commands. 

```C#
[HandlerClass(HandlerType.Command)]
public class MyHandler : IStaticHandler 
{
    private IRepository repository;
    public MyHandler(IRepository repository) 
    {
        this.repository = repository;
    }

    public async Task Handle(MessageContext ctx, object message) 
    {
        var user = repository.Load<UserAggregate>(ctx.AggregateId);
        switch (message) 
        {
            case ChangeName c: 
                Handle(user, c);
                break;
            ...
        }
        await repository.Save(ctx, user);
    }

    [HandlerMethod]
    public async Task Handle(UserAggregate user, ChangeName c) 
    {
        user.ChangeName(c.Name);
    }
}
```

If a single switch statement is not preferred then one can inherit from the `StaticHandlerBase` base class. Handlers inheriting from that class can then register individual handler methods and `StaticHandlerBase` will handle the routing internally. 

```C#
[HandlerClass(HandlerType.Command)]
public class MyHandler : StaticHandlerBase 
{
    private IRepository repository;
    public MyHandler(IRepository repository) 
    {
        // Register handlers with the base class
        Register<ChangeName>(Handle);
        this.repository = repository;
    }

    // This method will be called by StaticHandlerBase.Handle
    // at run-time
    [HandlerMethod]
    public async Task Handle(MessageContext ctx, ChangeName c) 
    {
        var user = repository.Load<UserAggregate>(ctx.AggregateId);
        user.ChangeName(c.Name);
        await repository.Save(ctx, user);
    }
}
```


#### Dynamic Dispatching
Dynamic dispatching is enabled by using the `DynamicDispatcher` class. This form of dispatching uses reflection to inject parameters into a handler method. Compare the code below to the static dispatch forms above to see how this type of dispatching can cut down on the amount of boilerplate code required. 

```C#
[HandlerClass(HandlerType.Command)]
public class MyHandler
{
    [HandlerMethod]
    public void Handle(UserAggregate user, ChangeName c)
    {
        user.ChangeName(c.Name);
    }
}
```

### Running Opine
Opine comes with a message processing console application. This application, **Opine.Job** provides a quick way to process work asynchronously. The job can process Events or Commands.

#### Opine.Job
The application reads new messages from the registered `IMessageStore` instance. You specify what Stream to read (Event or Command). You can provide override options such as the offset into the message store to start reading from and the number of messages to read into memory. You can also provide a specific type (Aggregate type) to process. 

| Option | Name | Default Value | Description |
|--------|------|:-------------:|-------------|
| -s | Stream | **Required** | Possible values: *events*, *commands*. |
| -t | Type   | **Required** | Possible values: *User*, *Invoice* |
| -o | Offset | 0 | Offset in message stream |
| -b | Buffer | 100 | Number of messages to read per fetch |
| -a | Plugin Assemblies | **Required** | Possible values: *Path/To/Your/PluginAssembly.dll* |


#### Plugins
Plugins are created by implementing the `IPlugin` interface. Your implementation of `IPlugin` will give you the chance to make decisions such as what type of repository and dispatching strategy you would like to use. The interface provides methods that allow the plugin to participate in the lifecycle of the Job application:
* Parse command line arguments
* Register services for dependency injection
* Register handlers for commands and events

```C#
public class TestPlugin : IPlugin 
{
    public void ParseArguments(string[] args)
    {
        // Process arguments
        // Maybe store them for use in the following methods
    }
    
    public void RegisterServices(IServiceCollection services)
    {
        services
            // Dispatching
            .AddScoped<IDispatcher, DynamicDispatcher>()
            // Repository
            .AddScoped<IRepository, EventSourcedRepository>()
            // Messaging
            .AddScoped<IEventStoreConnection>(s => {
                    var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
                    connection.ConnectAsync().Wait();
                    return connection;
                })
            .AddScoped<IMessageStore, ESMessageStore>()
            // NOTE: We are using ESMessageStore to read messages and write messages
            //       Serve the same instance in both cases
            .AddScoped<IMessageReader>(s => s.GetService<IMessageStore>())
            .AddScoped<ISnapshotStore, ESSnapshotStore>()
                
            // Unit of work
            .AddScoped<IUnitOfWork, ESUnitOfWork>()
            
            // Handlers
            .AddScoped<MyHandler>();
    }
    
    public void RegisterHandlers(IHandlerRegistry handlerRegistry)
    {
        var finder = new HandlerFinder();
        var handlers = finder.FindHandlers(typeof(MyHandler));
        foreach (var h in handlers)
        {
            handlerRegistry.Register(h.MessageType, h);
        }
    }
}
```

# Technologies
* .NET Core 2.0
* C# 7

# Optional Technologies
* Event Store (https://geteventstore.com)

# Third-party Libraries
* Newtonsoft.Json (https://github.com/JamesNK/Newtonsoft.Json)
* CommandLineParser (https://github.com/gsscoder/commandline)
* EventStore.ClientAPI.NetCore (https://github.com/EventStore/ClientAPI.NetCore)

# References
* Martin Fowler on CQRS (https://martinfowler.com/bliki/CQRS.html)
* CQRS FAQ (http://cqrs.nu/Faq)
