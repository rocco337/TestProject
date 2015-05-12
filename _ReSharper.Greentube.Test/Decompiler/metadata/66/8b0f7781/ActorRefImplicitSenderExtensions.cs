// Type: Akka.Actor.ActorRefImplicitSenderExtensions
// Assembly: Akka, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: BF67E82C-C98D-4628-9184-D24944E11D78
// Assembly location: C:\Users\rbobi\Documents\Visual Studio 2013\Projects\Greentube.Test.Console\packages\Akka.1.0.1\lib\net45\Akka.dll

namespace Akka.Actor
{
  public static class ActorRefImplicitSenderExtensions
  {
    public static void Tell(this IActorRef receiver, object message);
    /// <summary>
    /// Forwards the message using the current Sender
    /// 
    /// </summary>
    /// <param name="receiver">The actor that receives the forward</param><param name="message">The message to forward</param>
    public static void Forward(this IActorRef receiver, object message);
  }
}
