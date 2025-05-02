using MMLib.SwaggerForOcelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("Routes/ocelot.products.json", optional: false, reloadOnChange: true)
    .AddJsonFile("Routes/ocelot.paiement.json", optional: false, reloadOnChange: true)
    .AddJsonFile("Routes/ocelot.users.json", optional: false, reloadOnChange: true)
    .AddJsonFile("Routes/ocelot.global.json", optional: false, reloadOnChange: true)
    .AddJsonFile("Routes/ocelot.orders.json", optional: false, reloadOnChange: true)
    .AddJsonFile("Routes/ocelot.SwaggerEndPoints.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();

app.Run();
