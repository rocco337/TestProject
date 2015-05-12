// Type: Akka.Actor.Props
// Assembly: Akka, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: BF67E82C-C98D-4628-9184-D24944E11D78
// Assembly location: C:\Users\rbobi\Documents\Visual Studio 2013\Projects\Greentube.Test.Console\packages\Akka.1.0.1\lib\net45\Akka.dll

using Akka.Routing;
using Akka.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Akka.Actor
{
  /// <summary>
  /// Props is a configuration object used in creating an [[Actor]]; it is
  ///                 immutable, so it is thread-safe and fully shareable.
  ///                 Examples on C# API:
  /// 
  /// <code>
  /// private Props props = Props.Empty();
  ///               private Props props = Props.Create(() =&gt; new MyActor(arg1, arg2));
  /// 
  ///               private Props otherProps = props.WithDispatcher("dispatcher-id");
  ///               private Props otherProps = props.WithDeploy(deployment info);
  /// 
  /// </code>
  /// 
  /// </summary>
  public class Props : IEquatable<Props>, ISurrogated
  {
    /// <summary>
    /// The none
    /// 
    /// </summary>
    public static readonly Props None;
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    protected Props();
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class from a copy.
    /// 
    /// </summary>
    protected Props(Props copy);
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    /// <param name="type">The type.</param><param name="args">The arguments.</param>
    public Props(Type type, object[] args);
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    /// <param name="type">The type.</param>
    public Props(Type type);
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    /// <param name="type">The type.</param><param name="supervisorStrategy">The supervisor strategy.</param><param name="args">The arguments.</param>
    public Props(Type type, SupervisorStrategy supervisorStrategy, IEnumerable<object> args);
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    /// <param name="type">The type.</param><param name="supervisorStrategy">The supervisor strategy.</param><param name="args">The arguments.</param>
    public Props(Type type, SupervisorStrategy supervisorStrategy, params object[] args);
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    /// <param name="deploy">The deploy.</param><param name="type">The type.</param><param name="args">The arguments.</param>
    public Props(Deploy deploy, Type type, IEnumerable<object> args);
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Akka.Actor.Props"/> class.
    /// 
    /// </summary>
    /// <param name="deploy">The deploy.</param><param name="type">The type.</param><param name="args">The arguments.</param>
    public Props(Deploy deploy, Type type, params object[] args);
    public ISurrogate ToSurrogate(ActorSystem system);
    public bool Equals(Props other);
    public override bool Equals(object obj);
    public override int GetHashCode();
    /// <summary>
    /// Creates the specified factory.
    /// 
    /// </summary>
    /// <typeparam name="TActor">The type of the actor.</typeparam><param name="factory">The factory.</param><param name="supervisorStrategy">Optional: Supervisor strategy</param>
    /// <returns>
    /// Props.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">The create function must be a 'new T (args)' expression</exception>
    public static Props Create<TActor>(Expression<Func<TActor>> factory, SupervisorStrategy supervisorStrategy = null) where TActor : ActorBase;
    /// <summary>
    /// Creates this instance.
    /// 
    /// </summary>
    /// <typeparam name="TActor">The type of the actor.</typeparam>
    /// <returns>
    /// Props.
    /// </returns>
    public static Props Create<TActor>(params object[] args) where TActor : ActorBase;
    /// <summary>
    /// Creates an actor by an actor producer
    /// 
    /// </summary>
    /// <typeparam name="TProducer">The type of the actor producer</typeparam><param name="args">The arguments</param>
    /// <returns>
    /// Props
    /// </returns>
    public static Props CreateBy<TProducer>(params object[] args) where TProducer : class, IIndirectActorProducer;
    /// <summary>
    /// Creates this instance.
    /// 
    /// </summary>
    /// <typeparam name="TActor">The type of the actor.</typeparam>
    /// <returns>
    /// Props.
    /// </returns>
    public static Props Create<TActor>(SupervisorStrategy supervisorStrategy) where TActor : new(), ActorBase;
    /// <summary>
    /// Creates the specified type.
    /// 
    /// </summary>
    /// <param name="type">The type.</param><param name="args"/>
    /// <returns>
    /// Props.
    /// </returns>
    public static Props Create(Type type, params object[] args);
    /// <summary>
    /// Returns a new Props with the specified mailbox set.
    /// 
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>
    /// Props.
    /// </returns>
    public Props WithMailbox(string path);
    /// <summary>
    /// Returns a new Props with the specified dispatcher set.
    /// 
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>
    /// Props.
    /// </returns>
    public Props WithDispatcher(string path);
    /// <summary>
    /// Returns a new Props with the specified router config set.
    /// 
    /// </summary>
    /// <param name="routerConfig">The router configuration.</param>
    /// <returns>
    /// Props.
    /// </returns>
    public Props WithRouter(RouterConfig routerConfig);
    /// <summary>
    /// Returns a new Props with the specified deployment configuration.
    /// 
    /// </summary>
    /// <param name="deploy">The deploy.</param>
    /// <returns>
    /// Props.
    /// </returns>
    public Props WithDeploy(Deploy deploy);
    /// <summary>
    /// Returns a new Props with the specified supervisor strategy set.
    /// 
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    /// <returns>
    /// Props.
    /// </returns>
    public Props WithSupervisorStrategy(SupervisorStrategy strategy);
    /// <summary>
    /// Create a new actor instance. This method is only useful when called during
    ///                 actor creation by the ActorSystem.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// ActorBase.
    /// </returns>
    public virtual ActorBase NewActor();
    /// <summary>
    /// Copies this instance.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// Props.
    /// </returns>
    protected virtual Props Copy();
    /// <summary>
    /// Gets the type.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The type.
    /// </value>
    [JsonIgnore]
    public Type Type { get; }
    /// <summary>
    /// Gets or sets the dispatcher.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The dispatcher.
    /// </value>
    [JsonIgnore]
    public string Dispatcher { get; }
    /// <summary>
    /// Gets or sets the mailbox.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The mailbox.
    /// </value>
    [JsonIgnore]
    public string Mailbox { get; }
    public string TypeName { get; private set; }
    /// <summary>
    /// Gets or sets the router configuration.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The router configuration.
    /// </value>
    [JsonIgnore]
    public RouterConfig RouterConfig { get; }
    /// <summary>
    /// Gets or sets the deploy.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The deploy.
    /// </value>
    public Deploy Deploy { get; protected set; }
    /// <summary>
    /// Gets or sets the supervisor strategy.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The supervisor strategy.
    /// </value>
    public SupervisorStrategy SupervisorStrategy { get; protected set; }
    /// <summary>
    /// A Props instance whose creator will create an actor that doesn't respond to any message
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The empty.
    /// </value>
    public static Props Empty { get; }
    /// <summary>
    /// Gets the arguments.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The arguments.
    /// </value>
    public object[] Arguments { get; }
    public class PropsSurrogate : ISurrogate
    {
      public ISurrogated FromSurrogate(ActorSystem system);
      public Type Type { get; set; }
      public Deploy Deploy { get; set; }
      public object[] Arguments { get; set; }
    }
  }
}
