using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchServices;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDB", MongoClientSettings
    .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
    .Key(x => x.Make, KeyType.Text)
    .Key(x => x.Model, KeyType.Text)
    .Key(x => x.Color, KeyType.Text)
    .CreateAsync();

        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var Items = await httpClient.GetItemsForSearchDb();

        Console.WriteLine("<====================>" + Items.Count + "return from the auctions services<====================>");

        if (Items.Count > 0) await DB.SaveAsync(Items);

    }
}
