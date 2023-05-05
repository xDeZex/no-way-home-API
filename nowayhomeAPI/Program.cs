using System.Text.Json;
using System.Text.Json.Nodes;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                       policy =>
                       {
                           policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                       });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors(MyAllowSpecificOrigins);


app.MapGet("/stationreal", async (string siteid, int timewindow) =>
{
    Console.WriteLine("real");
    string realkey = "cea6074a2f0248a3b466aae7a88af063";
    string URLStationReal = "https://api.sl.se/api2/realtimedeparturesv4.json?key=" + realkey;
    var json = await GetJSONAsync($"{ URLStationReal}&siteid={siteid}&timewindow={timewindow}");
    return json;
}).RequireCors(MyAllowSpecificOrigins);

app.MapGet("/stationname", async (string station) =>
{
    Console.WriteLine("name");
    string namekey = "968f31ed688a44bcbb24bafd5fc5a40b";
    string URLStationName = "https://api.sl.se/api2/typeahead.json?key=" + namekey;
    var json = await GetJSONAsync($"{ URLStationName}&searchstring={station}&stationsonly=true");
    return json;

});

app.MapGet("", () =>
{
    return "test";
});



app.Run();


static async Task<JsonNode> GetJSONAsync(string path)
{

    HttpClient client = new HttpClient();
    JsonNode json;
    HttpResponseMessage response = await client.GetAsync(path);
    json = await response.Content.ReadFromJsonAsync<JsonNode>();
    return json;
}

