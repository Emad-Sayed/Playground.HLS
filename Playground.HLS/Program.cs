using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Playground.Multimedia;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJahezMultiMedia(options =>
{
    options.ContentRootPath = Path.Combine(builder.Environment.ContentRootPath, "videos");
    options.FFmpegPath = builder.Configuration.GetSection("FFmpeg")["Path"];
    options.TempFilePath = "TempVideos";
    options.Outputpath = Path.Combine(builder.Environment.ContentRootPath, "videos", "HLS");
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors();

//app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseStaticFiles();
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".m3u8"] = "application/x-mpegURL";
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "videos")),
    ContentTypeProvider = provider,
    RequestPath = "/videos",
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.Name.EndsWith(".m3u8"))
        {
            ctx.Context.Response.ContentType = "application/vnd.apple.mpegurl";
        }
    }
});
app.MapControllers();

app.Run();
