using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Onyx.Api.Extensions;
using Onyx.Api.Options;
using Onyx.Services.Products.Extensions;
using Onyx.Services.Products.Messaging.Commands;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddProductsService();

// Options
var authOptions = new AuthOptions();
builder.Configuration.GetSection(AuthOptions.SectionName).Bind(authOptions);
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));

// Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = authOptions.Authority;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authOptions.Authority,
        ValidAudience = authOptions.ClientId,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateAudience = true,
    };
}).AddApiKey(options =>
{
    var config = builder.Configuration.GetSection(AuthOptions.SectionName);
    config.Bind(options);
});

// Messaging
const string queueName = "onyx-queue";

builder.Services.AddRebus(config =>
    config
        .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), queueName))
        .Logging(l => l.ColoredConsole())
        .Timeouts(t => t.StoreInMemory())
        .Routing(cf =>
        {
            //TODO: Should clean this up for prod code.
            cf.TypeBased()
                .MapAssemblyOf<CreateProductCommand>(queueName);
        })
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("api/healthcheck")
    .AllowAnonymous();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();