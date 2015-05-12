// Type: Akka.Actor.ActorRefImplicitSenderExtensions
// Assembly: Akka, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 577EC7FF-B6D6-4D8D-BEEE-70047E5A1B97
// Assembly location: C:\Users\rbobi\Documents\Visual Studio 2013\Projects\Greentube.Test.Console\packages\Akka.1.0.0\lib\net45\Akka.dll

namespace Akka.Actor
{
  public static class ActorRefImplicitSenderExtensions
  {
    public static void Tell(this IActorRef receiver, object message);
    /// <summary>
    /// Forwards the message using the current Sender
    /// 
    /// </summary>
    /// <param name="message"/>
    public static void Forward(this IActorRef receiver, object message);
  }
}
