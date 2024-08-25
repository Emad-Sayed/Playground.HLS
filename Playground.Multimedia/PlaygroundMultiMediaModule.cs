using Microsoft.Extensions.DependencyInjection;
namespace Playground.Multimedia
{
    public static class PlaygroundMultiMediaModule
    {
        public static void AddMultiMedia(this IServiceCollection service,Action<MultiMediaOptions> options)
        {
            var configuration = new MultiMediaOptions();
            options(configuration);
            service.AddSingleton(configuration);
            service.AddSingleton<IInputOutputHelper, InputOutHelper>();
            service.AddSingleton<IMediaConverter, MediaConverter>();
        }
    }
}
