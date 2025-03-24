using DemoAIChatForBusiness.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.Context;
using System.Text;

public static class Program
{
    private static MemoryServerless s_memory = null!;
    private static AppSettings settings = null!;
    private static string TitleProgram = "Vui lòng chọn doanh nghiệp";
    private static int selectedIndex = 0;
    public static async Task Main()
    {

        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        settings = new AppSettings();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        configuration.Bind(settings);

        var ollamaConfig = new OllamaConfig
        {
            Endpoint = settings.OllamaConfig.Endpoint,
            TextModel = new OllamaModelConfig(settings.OllamaConfig.TextModel, settings.OllamaConfig.ContextLength),
            EmbeddingModel = new OllamaModelConfig(settings.OllamaConfig.EmbeddingModel, settings.OllamaConfig.EmbeddingLength)

        };

        var builder = new KernelMemoryBuilder()
            .WithOllamaTextGeneration(ollamaConfig, new CL100KTokenizer())
            .WithOllamaTextEmbeddingGeneration(ollamaConfig, new CL100KTokenizer());

        s_memory = builder.Build<MemoryServerless>();

        await BrandOptions();

    }
    public static async Task Converstation()
    {
        Console.WriteLine($"===== AI Chatbot - {TitleProgram} =====");
        Console.WriteLine("Nhập lệnh: \n- `[câu hỏi]` để hỏi \n- `exit` để chọn doanh nghiệp khác.");
        while (true)
        {

            Console.Write("\n> ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "exit")
            {
                Console.Clear();
                await BrandOptions();
            }
            await AskQuestion(input.Substring(0));
        }
    }

    private static async Task BrandOptions()
    {
        ConsoleKey key;

        if (settings.Documents.Count == 0)
        {
            Console.WriteLine("❌ Không có doanh nghiệp nào để chọn!");
            return;
        }

        do
        {
            Console.Clear();
            Console.WriteLine($"===== AI Chatbot - Chọn Doanh Nghiệp =====\n");

            for (int i = 0; i < settings.Documents.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {settings.Documents[i].Title}"); // Highlighted option
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {settings.Documents[i].Title}");
                }
            }

            Console.WriteLine("\n🔼 Sử dụng ▲ (Up) và ▼ (Down) để chọn, nhấn Enter để xác nhận.");
            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? settings.Documents.Count - 1 : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == settings.Documents.Count - 1) ? 0 : selectedIndex + 1;
            }

        } while (key != ConsoleKey.Enter);

        TitleProgram = settings.Documents[selectedIndex].Title;
        Console.Clear();
        await StoreFile();
        Console.Clear();
        await Converstation();
    }

    private static async Task AskQuestion(string question)
    {
        Console.Write("Đang suy nghĩ...");
        var thinkingCTS = StartThinkingAnimation(); // Bắt đầu hiệu ứng xoay

        var role = $"Vai trò của bạn là chủ sở hữu: {settings.Documents[selectedIndex].Title}. "
      + $"Nhiệm vụ: Trả lời khách hàng với giọng điệu chuyên nghiệp, thân thiện và tự hào.";

        var fullQuestion = $"{role}\n\nCâu hỏi: {question}";


        var context = new RequestContext();

        context.SetArg("custom_rag_max_tokens_int", 20); // Kernel-Memory token limit
        context.SetArg("custom_rag_temperature_float", 0.1); // More deterministic, concise response

        var answer = await s_memory.AskAsync(fullQuestion, minRelevance: 0.3, filter: MemoryFilters.ByTag("user", settings.Documents[selectedIndex].BrandId), context: context);


        StopThinkingAnimation(thinkingCTS);
        Console.WriteLine("\n🔹 Câu hỏi: " + question);
        Console.WriteLine("✅ Trả lời: " + answer.Result);
    }


    private static async Task StoreFile()
    {
        DocumentConfiguration brand = settings.Documents[selectedIndex];
        string replacedPath = brand.Path.Replace("/", "\\");
        string baseDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../..")) + replacedPath;

        if (Directory.Exists(baseDirectory))
        {
            string[] files = Directory.GetFiles(baseDirectory);
            if (files.Length >= 1)
            {
                Console.Write($"[1] Kiểm tra dữ liệu của {brand.Title}...");


                var existingDoc = await s_memory.GetDocumentStatusAsync(brand.BrandId);
                if (existingDoc != null)
                {
                    Console.WriteLine($"[1] Dữ liệu của {brand.Title} đã có sẵn, bỏ qua lưu trữ.");
                    return;
                }

                // Import only if not already stored
                Console.Write($"\n[2] Hệ thống đang lưu dữ liệu của {brand.Title}...");
                var thinkingCTS = StartThinkingAnimation();
                var context = new RequestContext();
                await s_memory.ImportDocumentAsync(new Document(brand.BrandId).AddFiles(files).AddTag("user", brand.BrandId), context: context);
                StopThinkingAnimation(thinkingCTS);
            }
        }
    }

    private static CancellationTokenSource StartThinkingAnimation()
    {
        var cts = new CancellationTokenSource();
        string[] spinner = { "\\", "|", "/", "-" };
        int index = 0;

        Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                Console.Write($"\b{spinner[index]}");
                index = (index + 1) % spinner.Length;
                await Task.Delay(200);
            }
            Console.Write("\b ");
        });

        return cts;
    }

    private static void StopThinkingAnimation(CancellationTokenSource cts)
    {
        cts.Cancel();
    }



}