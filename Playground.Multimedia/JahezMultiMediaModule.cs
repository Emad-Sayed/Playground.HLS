using Microsoft.Extensions.DependencyInjection;
namespace Playground.Multimedia
{
    public static class JahezMultiMediaModule
    {
        public static void AddJahezMultiMedia(this IServiceCollection service,Action<MultiMediaOptions> options)
        {
            var configuration = new MultiMediaOptions();
            options(configuration);
            service.AddSingleton(configuration);
            service.AddSingleton<IInputOutputHelper, InputOutHelper>();
            service.AddSingleton<IMediaConverter, MediaConverter>();
        }
    }
}
