// Type: Akka.Actor.ITellScheduler
// Assembly: Akka, Version=1.0.1.0, Culture=neutral
// MVID: BF67E82C-C98D-4628-9184-D24944E11D78
// Assembly location: C:\Users\rbobi\Documents\Visual Studio 2013\Projects\Greentube.Test.Console\packages\Akka.1.0.1\lib\net45\Akka.dll

using System;

namespace Akka.Actor
{
  /// <summary>
  /// A scheduler that's able to schedule sending messages.
  /// 
  /// </summary>
  public interface ITellScheduler
  {
    /// <summary>
    /// Schedules to send a message once after a specified period of time.
    /// </summary>
    /// <param name="delay">The time period that has to pass before the message is sent.</param><param name="receiver">The receiver.</param><param name="message">The message.</param><param name="sender">The sender.</param>
    void ScheduleTellOnce(TimeSpan delay, ICanTell receiver, object message, IActorRef sender);
    /// <summary>
    /// Schedules to send a message once after a specified period of time.
    /// </summary>
    /// <param name="delay">The time period that has to pass before the message is sent.</param><param name="receiver">The receiver.</param><param name="message">The message.</param><param name="sender">The sender.</param><param name="cancelable">An <see cref="T:Akka.Actor.ICancelable"/> that can be used to cancel sending of the message. Once the message has been sent, it cannot be canceled.</param>
    void ScheduleTellOnce(TimeSpan delay, ICanTell receiver, object message, IActorRef sender, ICancelable cancelable);
    /// <summary>
    /// Schedules to send a message repeatedly. The first message will be sent after the specified initial delay and there after at the rate specified.
    /// </summary>
    /// <param name="initialDelay">The time period that has to pass before the first message is sent.</param><param name="interval">The interval, i.e. the time period that has to pass between messages are being sent.</param><param name="receiver">The receiver.</param><param name="message">The message.</param><param name="sender">The sender.</param>
    void ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, ICanTell receiver, object message, IActorRef sender);
    /// <summary>
    /// Schedules to send a message repeatedly. The first message will be sent after the specified initial delay and there after at the rate specified.
    /// </summary>
    /// <param name="initialDelay">The time period that has to pass before the first message is sent.</param><param name="interval">The interval, i.e. the time period that has to pass between messages are being sent.</param><param name="receiver">The receiver.</param><param name="message">The message.</param><param name="sender">The sender.</param><param name="cancelable">An <see cref="T:Akka.Actor.ICancelable"/> that can be used to cancel sending of the message. Once the message has been sent, it cannot be canceled.</param>
    void ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, ICanTell receiver, object message, IActorRef sender, ICancelable cancelable);
  }
}
