using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mirza.Common;

namespace Mirza.Cli
{
    internal static class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false })
        {
#if DEBUG
            BaseAddress = new Uri(@"https://localhost:5001")
#endif
#if RELEASE
            BaseAddress = new Uri(@"http://app.mirzza.ir")
#endif
        };

        private static readonly string MirzaConfigDirectory = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Mirza");

        private static readonly string[] ValidAcceptResponses = { string.Empty, "Y", "YES" };

        private static string ConfigFile => Path.Join(MirzaConfigDirectory, ".mirza");

        private static async Task Main(string[] args)
        {
            EnsureMirzaDirectory();

            EnsureMirzaConfigFile();

            var mirzaCommand = new RootCommand("Mirza - Your friendly work log assistant");

            AddDoroodCommand(mirzaCommand);
            AddBedroodCommand(mirzaCommand);
            AddBenevisCommand(mirzaCommand);
            AddCheKhabarCommand(mirzaCommand);
            AddKhatbezanCommand(mirzaCommand);

            await mirzaCommand.InvokeAsync(args);
        }

        private static void AddCheKhabarCommand(Command rootCmd)
        {
            var logCommand = new Command("chekhabar", "get work log report");

            var dateArg = new Argument<DateTime>("date", () => DateTime.Today.Date)
            {
                Description = "Date of the report",
                Arity = ArgumentArity.ZeroOrOne
            };
            logCommand.AddArgument(dateArg);

            logCommand.Handler = CommandHandler.Create<DateTime>(HandleWorkLogReportCommand);

            rootCmd.AddCommand(logCommand);
        }

        private static void AddKhatbezanCommand(Command rootCmd)
        {
            var logCommand = new Command("khatbezan", "Delete a worklog");

            var workLogArg = new Argument<int>("WorkLogId")
            {
                Description = "Work log id to delete",
                Arity = ArgumentArity.ExactlyOne
            };
            logCommand.AddArgument(workLogArg);

            logCommand.Handler = CommandHandler.Create<int>(HandleWorkLogDeleteCommand);

            rootCmd.AddCommand(logCommand);
        }

        private static async Task HandleWorkLogDeleteCommand(int workLogId)
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

            SetHttpClientAccessKeyHeader();

            var httpResponse = await HttpClient.DeleteAsync($"api/users/worklog/{workLogId}");
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<DeleteWorkLogServiceSuccessResponse>(responseContent, new JsonSerializerOptions
                {
                    IgnoreNullValues = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                Console.WriteLine($"Done, work log {response.Id} deleted sucessfully");
            }
            else
            {
                var errorResponse =
                    JsonSerializer.Deserialize<DeleteWorkLogServiceErrorResponse>(responseContent,
                        new JsonSerializerOptions
                        {
                            IgnoreNullValues = false,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = false
                        });
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("oops!");
                Console.WriteLine(errorResponse.ErrorMessage);
                if (errorResponse.ErrorDetails != null)
                {
                    Console.WriteLine(errorResponse.ErrorDetails);
                }

                Console.ResetColor();
            }
        }

        private static async Task HandleWorkLogReportCommand(DateTime date)
        {
            var dateStr = date.ToString("yyyy-MM-dd");
            if (!IsAuthenticated())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("You first have to say hello to Mirza in order for him to know who you are...");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Try running 'mirza dorood <access-key>'");
                Console.ResetColor();
                return;
            }

            SetHttpClientAccessKeyHeader();

            var httpResponse = await HttpClient.GetAsync($"api/users/worklog?date={dateStr}");
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                var report = JsonSerializer.Deserialize<WorkLogReport>(responseContent, new JsonSerializerOptions
                {
                    IgnoreNullValues = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                var entries = report.WorkLogItems.OrderBy(o => o.StartTime);
                var border = new string('=', 10);
                Console.WriteLine($"============== {report.ReportDatePersian} =============");
                foreach (var item in entries)
                {
                    Console.WriteLine(
                        $"{item.Id})   {item.StartTime}\t{item.EndTime}\t{item.Description} (details: {item.Details})");
                }

                Console.WriteLine($"{border} Total => {report.TotalDuration} {border}");
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.Found)
            {
                if (httpResponse.Headers.Location.AbsolutePath.Equals("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("oops!");
                    Console.WriteLine("it looks like your accesskey is not valid.");

                    Console.ResetColor();
                }
            }
            else
            {
                var errorResponse =
                    JsonSerializer.Deserialize<AddWorkLogServiceErrorResponse>(responseContent,
                        new JsonSerializerOptions
                        {
                            IgnoreNullValues = false,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = false
                        });
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("oops!");
                Console.WriteLine(errorResponse.ErrorMessage);
                if (errorResponse.ErrorDetails != null)
                {
                    Console.WriteLine(errorResponse.ErrorDetails);
                }

                Console.ResetColor();
            }
        }

        private static void SetHttpClientAccessKeyHeader()
        {
            var accessKey = File.ReadAllLines(ConfigFile).First();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AccessKey", accessKey);
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
                Directory.CreateDirectory(MirzaConfigDirectory);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/detail/{accessKey}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("AccessKey", accessKey) }
            };

            var result = await HttpClient.SendAsync(request);
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

            Console.WriteLine($"access key set to {accessKey}");
        }

        private static async Task HandleLogWorkCommand(TimeSpan from, TimeSpan to, string desc, string details)
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

            var accessKey = File.ReadAllLines(ConfigFile).First();

            var user = GetUserFromConfigFile();

            var consented = GetConsentForSend(from, to, desc, details, user);

            if (consented)
            {
                await SendWorkLogToServerAndHandleResponse(from, to, desc, details, accessKey);
            }
            else
            {
                Console.WriteLine("discarding...");
            }
        }

        private static async Task SendWorkLogToServerAndHandleResponse(TimeSpan from, TimeSpan to, string desc,
            string details,
            string accessKey)
        {
            var requestObject = new AddWorkLogServiceInput(from, to, desc, details);
            var serialized = JsonSerializer.Serialize(requestObject);
            var content = new StringContent(serialized, Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/users/worklog")
            {
                Content = content,
                Headers = { Authorization = new AuthenticationHeaderValue("AccessKey", accessKey) }
            };
            var httpResponse = await HttpClient.SendAsync(requestMessage);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                var successResponse = JsonSerializer.Deserialize<AddWorkLogServiceSuccessResponse>(responseContent,
                    new JsonSerializerOptions
                    {
                        IgnoreNullValues = false,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = false
                    });
                Console.Write($"Done! Work log id: {successResponse.Id}");
            }
            else
            {
                var errorResponse =
                    JsonSerializer.Deserialize<AddWorkLogServiceErrorResponse>(responseContent,
                        new JsonSerializerOptions
                        {
                            IgnoreNullValues = false,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = false
                        });
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("oops!");
                Console.WriteLine(errorResponse.ErrorMessage);
                if (errorResponse.ErrorDetails != null)
                {
                    Console.WriteLine(errorResponse.ErrorDetails);
                }

                Console.ResetColor();
            }
        }

        private static bool GetConsentForSend(TimeSpan from, TimeSpan to, string desc, string details, UserModel user)
        {
            Console.WriteLine($"Dear {user.FirstName}, I'm adding a work log for you with the following details:");
            Console.WriteLine();
            Console.WriteLine($"\t{Utils.GetCurrentDateInPersian()}:\t{from} - {to}\t(duration: {to - from})");
            Console.WriteLine($"\tDescription:\t{desc ?? "-"}");
            Console.WriteLine($"\tDetails:\t{details ?? "-"}");
            Console.WriteLine();
            Console.WriteLine("if the information is correct, press 'y' otherwise any other key (default: y)");
            Console.Write(">");
            var response = Console.ReadLine();
            var sendToServer = response != null &&
                               ValidAcceptResponses.Contains(response.ToUpperInvariant());
            return sendToServer;
        }

        private static UserModel GetUserFromConfigFile()
        {
            var json = File.ReadAllLines(ConfigFile)[1];
            var model = JsonSerializer.Deserialize<UserModel>(json, new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });
            return model;
        }
    }
}