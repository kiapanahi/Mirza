using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mirza.Cli
{
    internal static class Program
    {
        private const string ConfigFileName = ".mirza";

        private static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri(@"https://localhost:5001/")
        };

        private static async Task Main(string[] args)
        {
            if (!File.Exists(ConfigFileName))
            {
                using var stream = File.Create(ConfigFileName);
            }

            var mirzaCommand = new RootCommand("Mirza - Your friendly work log assistant");

            AddDoroodCommand(mirzaCommand);
            AddBedroodCommand(mirzaCommand);
            AddBenevisCommand(mirzaCommand);

            _ = await mirzaCommand.InvokeAsync(args);
        }

        private static bool IsAuthenticated()
        {
            var fileContent = File.ReadAllLines(ConfigFileName);
            if (fileContent.Length == 0)
            {
                return false;
            }

            return fileContent[0].Length == 32;
        }

        private static void AddDoroodCommand(Command rootCmd)
        {
            var cmd = new Command("dorood", "set AccessKey for future use");

            var accessKeyArg = new Argument<string>("access-key")
            {
                Description = "Set the access-key",
                Arity = ArgumentArity.ExactlyOne
            };

            cmd.AddArgument(accessKeyArg);
            cmd.Handler = CommandHandler.Create<string>(HandleAddAccessKeyCommand);

            rootCmd.AddCommand(cmd);
        }

        private static void AddBedroodCommand(Command rootCmd)
        {
            var cmd = new Command("bedrood", "UNSET AccessKey")
            {
                Handler = CommandHandler.Create(() =>
                {
                    Console.WriteLine("Mirza will miss you.");
                    File.WriteAllText(ConfigFileName, string.Empty);
                })
            };

            rootCmd.AddCommand(cmd);
        }

        private static void AddBenevisCommand(Command rootCmd)
        {
            var logCommand = new Command("benevis", "record a work log");

            var fromArg = new Argument<TimeSpan>("from", () => DateTime.Now.TimeOfDay)
            {
                Description = "The starting time of the current work log",
                Arity = ArgumentArity.ExactlyOne
            };
            logCommand.AddArgument(fromArg);


            var toArg = new Argument<TimeSpan>("to")
            {
                Description = "The end time of the current work log",
                Arity = ArgumentArity.ExactlyOne
            };
            logCommand.AddArgument(toArg);

            var descriptionArg = new Argument<string>("desc")
            {
                Description = "The work log description - Optional",
                Arity = ArgumentArity.ZeroOrOne
            };
            logCommand.AddArgument(descriptionArg);

            var detailsArg = new Argument<string>("details")
            {
                Description = "The work log details - Optional",
                Arity = ArgumentArity.ZeroOrOne
            };
            logCommand.AddArgument(detailsArg);

            logCommand.Handler = CommandHandler.Create<TimeSpan, TimeSpan, string, string>(HandleLogWorkCommand);

            rootCmd.AddCommand(logCommand);
        }

        private static async Task HandleAddAccessKeyCommand(string accessKey)
        {
            var result = await HttpClient.GetAsync($"/api/users/detail/{accessKey}");
            if (!result.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error validating access-key");
                Console.ResetColor();
                return;
            }

            var content = await result.Content.ReadAsStringAsync();

            File.WriteAllText(ConfigFileName, accessKey);
            File.AppendAllText(ConfigFileName, Environment.NewLine);
            File.AppendAllText(ConfigFileName, content);

            Console.WriteLine($"access key set to {accessKey}");
        }

        private static void HandleLogWorkCommand(TimeSpan from, TimeSpan to, string desc, string details)
        {
            if (!IsAuthenticated())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("You first have to say hello to Mirza in order for him to know who you are...");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Try running 'mirza dorood <access-key>'");
                Console.ResetColor();
                return;
            }

            var json = File.ReadAllLines(ConfigFileName)[1];
            var model = JsonSerializer.Deserialize<UserModel>(json, new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            Console.WriteLine($"Dear {model.FirstName}, I'm adding a work log for you with the following details:");

            Console.WriteLine($"Date: {DateTime.Today.Date: yyyy-MM-dd} from {from} to {to}\tduration: {to - from}");
            Console.WriteLine($"Description: {desc}");
            Console.WriteLine($"Details: {details}");

            Console.WriteLine("if the information is correct, press 'y' otherwise any other key");
            Console.Write(">");
            var response = Console.Read();
            var sendToServer = response switch
            {
                'y' => true,
                'Y' => true,
                _ => false
            };

            if (sendToServer)
            {
                Console.WriteLine("sending to server");
            }
            else
            {
                Console.WriteLine("discarding...");
            }
        }
    }

    internal class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Name => $"{FirstName} {LastName}";
    }
}