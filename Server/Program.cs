using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Server.DataContext;
using Server.Domain.Identities;
using Server.Repositories;
using Server.Services;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServerServices();
var app = builder.Build();

app.UseApp();


app.Run();
