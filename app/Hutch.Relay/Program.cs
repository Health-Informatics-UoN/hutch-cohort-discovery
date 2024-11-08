using Hutch.Relay.Data;
using Hutch.Relay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RelayDb");
builder.Services.AddDbContext<ApplicationDbContext>(o => { o.UseNpgsql(connectionString); });

builder.Services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App specific
builder.Services
  .AddTransient<RelayTaskService>()
  .AddTransient<RelaySubTaskService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.MapControllers();

app.Run();
