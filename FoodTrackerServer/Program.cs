using FoodTrackerServer;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;
using Type = Google.GenAI.Types.Type;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Urls.Add("http://0.0.0.0:5246");

app.MapGet("/", () => "Hello World!");

app.MapPost("/generate-recipe", async (GenerateRecipeRequest request) =>
{
    var apiKey = builder.Configuration["GEMINI_API_KEY"];
    if (string.IsNullOrEmpty(apiKey))
        return Results.Problem("Gemini API key error");

    var client = new Client(apiKey: apiKey);

    Schema recipeSchema = new()
    {
        Type = Type.Object,
        Properties = new Dictionary<string, Schema>
        {
            ["name"] = new() { Type = Type.String },
            ["description"] = new() { Type = Type.String },
            ["type"] = new() { Type = Type.String },
            ["foods"] = new()
            {
                Type = Type.Array,
                Items = new Schema
                {
                    Type = Type.Object,
                    Properties = new Dictionary<string, Schema>
                    {
                        ["foodId"] = new() { Type = Type.Integer },
                        ["amount"] = new() { Type = Type.Number },
                    },
                    Required = [ "amount", "foodId"]
                }
            }
        },
        Required = [ "name", "description", "type", "foods"]
    };

    var config = new GenerateContentConfig
    {
        ResponseSchema = recipeSchema,
        ResponseMimeType = "application/json",
        Temperature = 0.5
    };

    var ingredients = string.Join("\n", request.Ingredients.Select(i => $"{i.Name} ({i.InventoryAmount} {i.Unit}, kcal: {i.Kcal}, protein: {i.Protein}, carbs: {i.Carbs}, fat: {i.Fat}), id: {i.Id}"));

    var macros = $"Target macros: {request.TargetMacros.Kcal} kcal, {request.TargetMacros.Protein}g protein, {request.TargetMacros.Carbs}g carbs, {request.TargetMacros.Fat}g fat";

    var prompt = $@"
Generate a {request.Type ?? "meal"} using provided ingredients: {ingredients}

{macros}

Rules:
Use only the ingredients from the list (4-6 items corresponding to the type of recipe)
Do not add any new ingredients
Each ingredient mentioned in the description must be listed in the ingredient list
The name and description must be in Czech
The description contains ONLY cooking steps
Every ingredient used MUST include its Id

";

    try
    {
        var response = await client.Models.GenerateContentAsync(
            model: "gemini-2.5-flash",
            contents: prompt,
            config: config
        );

        var json = response.Candidates[0].Content.Parts[0].Text;

        var recipe = JsonSerializer.Deserialize<RecipeModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });


        return Results.Json(recipe);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error calling API: {ex.Message}");
    }




});

app.Run();
