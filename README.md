# AI Chatbot with Ollama & Kernel Memory

![Sematic Kernel](https://github.com/user-attachments/assets/a34ba5d4-89e0-4682-a7bd-1546a0fa76a0)

## Overview
This project is an AI-powered chatbot that leverages **Ollama** for text generation and **Microsoft Kernel Memory** for document storage and retrieval. The chatbot uses company-provided documents to answer user queries, allowing businesses to provide AI-driven support and insights. 

### Features
- **Ollama Model Integration**: Supports text generation and embedding.
- **Customizable AI Models**: Easily change models via `appsettings.json`.
- **Business-Specific AI**: Uses company documents to generate accurate responses.
- **Memory Storage**: Stores and retrieves documents efficiently.
- **Interactive CLI**: Users can select a company and start a conversation.

## Getting Started

### Prerequisites
Make sure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Ollama](https://ollama.ai) for AI model processing
- A `appsettings.json` file configured with model and document settings

### Installation
1. **Clone the Repository**
   ```sh
   git clone https://github.com/quocbahuynh/demo-ai-for-businesses
   cd DemoAIChatForBusiness
   ```

2. **Install Dependencies**
   ```sh
   dotnet restore
   ```

3. **Run the Application**
   ```sh
   dotnet run
   ```

## Configuration
### Updating AI Models
You can update the AI model used in `appsettings.json`:
```json
"OllamaConfig": {
  "Endpoint": "http://localhost:11434",
  "TextModel": "llama3.2:latest",
  "ContextLength": 131072,
  "EmbeddingModel": "llama3.2:latest",
  "EmbeddingLength": 3072
}
```
Change `TextModel` and `EmbeddingModel` to your preferred models.

### Adding Company Documents
Documents are stored in `settings.Documents`:
```json
"Documents": [
  {
    "Title": "Company A",
    "BrandId": "company_a",
    "Path": "Stored/company_a"
  },
  {
    "Title": "Company B",
    "BrandId": "company_b",
    "Path": "Stored/company_b"
  }
]
```
#### Setting Up the Data Folder
Users must create a `Stored` folder inside `project-root/main-project/Stored`. Inside this folder, create subdirectories matching the `Path` in `appsettings.json`, and store `.txt` files containing company-provided data.

For example:
```
/DemoAIChatForBusiness (root)/
 ├── DemoAIChatForBusiness (main project)/
 │   ├── Stored/
 │   │   ├── company_a/
 │   │   │   ├── data.txt
 │   │   ├── company_b/
 │   │   │   ├── data.txt
 │   │   ├── company_c/
 │   │   │   ├── data.txt
```

## Usage
1. **Select a Company**: When running the chatbot, users will choose a company.
2. **Ask Questions**: Users can type questions, and the AI will answer based on stored company documents.
3. **Switch Company**: Type `exit` to select a different company.

## Contributing
Feel free to fork this repository and submit pull requests for improvements.
