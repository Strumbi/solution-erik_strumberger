

using BackendAkademija.api;
using BackendAkademija.api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppDi(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();