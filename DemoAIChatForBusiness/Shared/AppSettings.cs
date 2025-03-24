namespace DemoAIChatForBusiness.Shared
{
    public class AppSettings
    {
        public OllamaConfiguration OllamaConfig { get; set; }
        public List<DocumentConfiguration> Documents { get; set; }
    }

    public class OllamaConfiguration
    {
        public string Endpoint { get; set; }
        public string TextModel { get; set; }
        public int ContextLength { get; set; }
        public string EmbeddingModel { get; set; }
        public int EmbeddingLength { get; set; }
    }

    public class DocumentConfiguration
    {
        public string BrandId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
    }
}
