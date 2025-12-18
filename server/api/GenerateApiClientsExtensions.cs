using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;

namespace api;

public static class GenerateApiClientsExtensions
{
    public static async Task GenerateApiClientsFromOpenApi(this WebApplication app, string path)
    {
        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
            .GenerateAsync("v1");

        var openApiJson = document.ToJson();
        
        var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "openapi-with-docs.json");
        await File.WriteAllTextAsync(openApiPath, openApiJson);
        
        var documentFromJson = await OpenApiDocument.FromJsonAsync(openApiJson);
        
        var settings = new TypeScriptClientGeneratorSettings
        {
            Template = TypeScriptTemplate.Fetch,
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Interface,
                DateTimeType = TypeScriptDateTimeType.String,
                NullValue = TypeScriptNullValue.Undefined,
                TypeScriptVersion = 5.2m,
                GenerateCloneMethod = false,
                MarkOptionalProperties = true,
                GenerateConstructorInterface = true,
                ConvertConstructorInterfaceData = true
            }
        };
        
        var generator = new TypeScriptClientGenerator(documentFromJson, settings);
        var code = generator.GenerateFile();

        var outputPath = Path.Combine(Directory.GetCurrentDirectory() + path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await File.WriteAllTextAsync(outputPath, code);
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("OpenAPI JSON with documentation saved at: " + openApiPath);
        logger.LogInformation("TypeScript client generated at: " + outputPath);
    }
}