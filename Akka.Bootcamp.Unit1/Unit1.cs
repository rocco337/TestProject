using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Bootcamp.Unit1
{
    public class Unit1
    {
        public Unit1()
        {
            ActorSystem MyActorSystem = ActorSystem.Create("MyActorSystem");

            Props consoleWriterProps = Props.Create(() => new ConsoleWriterActor());
            var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

            Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor));
            var consoleValidationActor = MyActorSystem.ActorOf(validationActorProps, "consoleValidationActor");

            Props consoleReaderProps = Props.Create(() => new ConsoleReaderActor(consoleValidationActor));
            var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            PrintInstructions();

            MyActorSystem.AwaitTermination();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }
    }

    internal class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is InputError)
            {
                var msg = message as InputError;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg.Reason);
            }
            else if (message is InputSuccess)
            {
                var msg = message as InputSuccess;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg.Reason);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }
    }

    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        public const string StartCommand = "start";
        private readonly IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }

            GetAndValidateInput();
        }

        // in ConsoleReaderActor, after OnReceive()

        #region Internal methods

        private void DoPrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // if user typed ExitCommand, shut down the entire actor system (allows the process to exit)
                Context.System.Shutdown();
                return;
            }

            // otherwise, just hand message off to validation actor (by telling its actor ref)
            _validationActor.Tell(message);
        }

        #endregion Internal methods
    }

    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input
                _consoleWriterActor.Tell(new NullInputError("No input received."));
            }
            else
            {
                var valid = IsValid(msg);
                if (valid)
                {
                    // send success to console writer
                    _consoleWriterActor.Tell(new InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    // signal that input was bad
                    _consoleWriterActor.Tell(new ValidationError("Invalid: input had odd number of characters."));
                }
            }

            // tell sender to continue doing its thing (whatever that may be, this actor doesn't care)
            Sender.Tell(new ContinueProcessing());
        }

        /// <summary>
        /// Determines if the message received is valid.
        /// Currently, arbitrarily checks if number of chars in message received is even.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static bool IsValid(string msg)
        {
            var valid = msg.Length % 2 == 0;
            return valid;
        }
    }

    #region Neutral/system messages

    /// <summary>
    /// Marker class to continue processing.
    /// </summary>
    public class ContinueProcessing { }

    #endregion Neutral/system messages

    #region Success messages

    // in Messages.cs
    /// <summary>
    /// Base class for signalling that user input was valid.
    /// </summary>
    public class InputSuccess
    {
        public InputSuccess(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; private set; }
    }

    #endregion Success messages

    // in Messages.cs

    #region Error messages

    /// <summary>
    /// Base class for signalling that user input was invalid.
    /// </summary>
    public class InputError
    {
        public InputError(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; private set; }
    }

    /// <summary>
    /// User provided blank input.
    /// </summary>
    public class NullInputError : InputError
    {
        public NullInputError(string reason)
            : base(reason)
        {
        }
    }

    /// <summary>
    /// User provided invalid input (currently, input w/ odd # chars)
    /// </summary>
    public class ValidationError : InputError
    {
        public ValidationError(string reason)
            : base(reason)
        {
        }
    }

    #endregion Error messages
}