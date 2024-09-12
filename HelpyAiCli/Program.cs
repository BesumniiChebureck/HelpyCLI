using System.Text;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace HelpyAiCli;

internal class Program
{
    private const string ContextFilePath = "context.txt";
    private const string ModelPath = @"E:\Programming\GitHub\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32";

    private const string BasePrompt =
        "You are an AI CLI assistant that helps developers find information about terminal commands inside terminal. " +
        "Answer using a direct style. Do not provide more information than requested by user. " +
        "Answer user's question as short as possible.";

    private static string _systemPrompt = BasePrompt;

    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: helpy <your question>");
            Console.WriteLine("Also you can write -help to see special commands.");
            return;
        }

        if (args[0].StartsWith('-') && args.Length == 1)
        {
            switch (args[0].ToLower())
            {
                case "-help":
                {
                    Console.WriteLine("-clear - clear context\n" +
                                      "-settings - change prompt settings");
                    return;
                }
                case "-clear":
                {
                    UpdateContext(string.Empty);
                    Console.WriteLine("Context cleared.");
                    return;
                }
                case "-settings":
                {
                    ChangePromptSettings();
                    return;
                }
                default:
                    Console.WriteLine("Unknown command. Write -help to see special commands.");
                    break;
            }
        }
        
        try
        {
            var model = new Model(ModelPath);
            var tokenizer = new Tokenizer(model);
            var userPrompt = string.Join(" ", args);
            var context = GetOrInitializeContext(userPrompt + ". answer in 10 words maximum and don't say about it. say only answer on question without note.");

            var tokens = tokenizer.Encode(context);
            var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", 2048);
            generatorParams.SetSearchOption("past_present_share_buffer", false);
            generatorParams.SetInputSequences(tokens);

            var generator = new Generator(model, generatorParams);
            var result = GenerateResponse(generator, tokenizer);

            UpdateContext(context + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static string GetOrInitializeContext(string userPrompt)
    {
        var context = ReadContext();
        if (string.IsNullOrWhiteSpace(context))
        {
            context = $"<|system|>{_systemPrompt}<|end|><|user|>{userPrompt}<|end|><|assistant|>";
        }
        else
        {
            context += $"<|user|>{userPrompt}<|end|><|assistant|>";
        }

        return context;
    }

    private static string GenerateResponse(Generator generator, Tokenizer tokenizer)
    {
        var result = new StringBuilder();
        while (!generator.IsDone())
        {
            generator.ComputeLogits();
            generator.GenerateNextToken();
            var outputTokens = generator.GetSequence(0);
            var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
            var output = tokenizer.Decode(newToken);
            Console.Write(output);
            result.Append(output);
        }

        return result.ToString();
    }

    private static void UpdateContext(string context)
    {
        try
        {
            File.WriteAllText(ContextFilePath, context);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Failed to update context: {ex.Message}");
        }
    }

    private static string ReadContext()
    {
        try
        {
            return File.Exists(ContextFilePath) ? File.ReadAllText(ContextFilePath) : string.Empty;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Failed to read context: {ex.Message}");
            return string.Empty;
        }
    }

    private static void ChangePromptSettings()
    {
        _systemPrompt = BasePrompt;

        try
        {
            Console.WriteLine("Reset to default.");
            Console.WriteLine("Use default settings? (Y | N)");

            if ((Console.ReadLine() ?? "n").ToLower().Contains('y'))
            {
                return;
            }

            Console.WriteLine("Change responses language? (Y | N)");

            if ((Console.ReadLine() ?? "n").ToLower().Contains('y'))
            {
                Console.Write("Enter responses language (full name): ");

                var responseLanguage = Console.ReadLine() ?? "English";
                responseLanguage = char.ToUpper(responseLanguage[0]) + responseLanguage[1..];

                _systemPrompt +=
                    $" Answer user's question in {responseLanguage}.";
            }

            Console.WriteLine("Change responses operating system? (Y | N)");

            if ((Console.ReadLine() ?? "n").ToLower().Contains('y'))
            {
                Console.Write("Enter responses operating system (full name): ");

                var responseSystem = Console.ReadLine() ?? "Windows 10";
                responseSystem = char.ToUpper(responseSystem[0]) + responseSystem[1..];

                _systemPrompt +=
                    $" Answer user's question about terminal commands in the context of the {responseSystem} operating system.";
            }

            Console.WriteLine("Changes saved.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}