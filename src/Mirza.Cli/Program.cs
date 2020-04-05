using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mirza.Cli
{
    internal static class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri(@"https://localhost:5001/")
        };

        private static readonly string MirzaConfigDirectory = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Mirza");

        private static string ConfigFile => Path.Join(MirzaConfigDirectory, ".mirza");

        private static async Task Main(string[] args)
        {
            EnsureMirzaDirectory();

            EnsureMirzaConfigFile();

            var mirzaCommand = new RootCommand("Mirza - Your friendly work log assistant");

            AddDoroodCommand(mirzaCommand);
            AddBedroodCommand(mirzaCommand);
            AddBenevisCommand(mirzaCommand);

            _ = await mirzaCommand.InvokeAsync(args);
        }

        private static void EnsureMirzaConfigFile()
        {
            if (!File.Exists(ConfigFile))
            {
                File.Create(ConfigFile).Close();
            }

            if ((File.GetAttributes(ConfigFile) & FileAttributes.Hidden) == 0)
            {
                File.SetAttributes(ConfigFile, File.GetAttributes(ConfigFile) | FileAttributes.Hidden);
            }
        }

        private static void EnsureMirzaDirectory()
        {
            if (!Directory.Exists(MirzaConfigDirectory))
            {
                _ = Directory.CreateDirectory(MirzaConfigDirectory);
            }
        }

        private static bool IsAuthenticated()
        {
            var fileContent = File.ReadAllLines(ConfigFile);
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
                    HttpClient.DefaultRequestHeaders.Authorization = null;

                    File.Delete(ConfigFile);
                    EnsureMirzaConfigFile();
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
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AccessKey", accessKey);
            var result = await HttpClient.GetAsync($"/api/users/detail/{accessKey}");
            if (!result.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error validating access-key");
                Console.ResetColor();
                return;
            }

            var content = await result.Content.ReadAsStringAsync();

            // TODO: implement a better approach!
            File.Delete(ConfigFile);
            EnsureMirzaConfigFile();
            await using (var s = new StreamWriter(ConfigFile, true, Encoding.UTF8))
            {
                s.WriteLine(accessKey);
                s.Write(content);
            }
            // File.WriteAllText(ConfigFile, accessKey,Encoding.UTF8);
            // File.AppendAllText(ConfigFile, Environment.NewLine);
            // File.AppendAllText(ConfigFile, content);

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

            var json = File.ReadAllLines(ConfigFile)[1];
            var model = JsonSerializer.Deserialize<UserModel>(json, new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            Console.WriteLine($"Dear {model.FirstName}, I'm adding a work log for you with the following details:");

            Console.WriteLine($"{GetCurrentDateInPersian()}:\t{from} - {to}\t(duration: {to - from})");
            Console.WriteLine($"Description:\t{desc ?? "-"}");
            Console.WriteLine($"Details:\t{details ?? "-"}");

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

        private static string GetPersianDate(DateTime dt)
        {
            var pc = new PersianCalendar();

            var year = pc.GetYear(dt);
            var month = pc.GetMonth(dt).ToString().PadLeft(2, '0');
            var day = pc.GetDayOfMonth(dt).ToString().PadLeft(2, '0');
            return $"{year}/{month}/{day}";
        }

        private static string GetCurrentDateInPersian() => GetPersianDate(DateTime.Today);
    }

    internal class UserModel
    {
        public string FirstName { get; set; }
    }
}