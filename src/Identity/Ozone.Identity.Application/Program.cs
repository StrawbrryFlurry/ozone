using Microsoft.EntityFrameworkCore;
using Ozone.Identity.Api;
using Ozone.Identity.Core;
using Ozone.Identity.Core.Authorization.Provider.Azure;
using Ozone.Identity.Infrastructure;
using Ozone.Identity.Infrastructure.Auth;
using Ozone.Identity.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi();
builder.Services.AddApplicationCore();

builder.Services
  .AddOzoneAuthentication()
  .AddProvider<MsalAuthenticationProvider, MsalAuthenticationProviderOptions>(
    MsalAuthenticationProvider.AuthenticationProviderScheme,
    builder.Configuration.GetSection("msal")
  );

builder.Services.AddPersistence((sp, optionsBuilder) => { optionsBuilder.UseSqlServer(); });
builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.MapControllers();

app.Run();