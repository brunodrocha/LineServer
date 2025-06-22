// See https://aka.ms/new-console-template for more information

using System.Net;

using NBomber.Contracts;
using NBomber.CSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        var apiUrl = GetArg(args, "--api-url");
        if (!int.TryParse(GetArg(args, "--lines-total"), out var totalLines))
        {
            throw new ArgumentException("--lines-total");
        }

        var client = new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        };

        NBomberRunner
            .RegisterScenarios(
                Scenario.Create("Scenario 2000 users constantly", async context =>
                {
                    var index = context.Random.Next(0, totalLines);
                    var result = await client.GetAsync($"/lines/{index}");

                    var body = await result.Content.ReadAsStringAsync();

                    if (result.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(body))
                    {
                        return Response.Fail();
                    }

                    return Response.Ok();
                })
                .WithLoadSimulations(
                    Simulation.KeepConstant(2000, TimeSpan.FromSeconds(30))))
            .Run();
    }

    private static string GetArg(string[] args, string argName)
    {
        var index = Array.IndexOf(args, argName);

        if (index < 0 || index + 1 >= args.Length)
        {
            throw new ArgumentNullException(argName);
        }

        return args[index + 1];
    }
}