using Microsoft.Extensions.DependencyInjection;
using Opine.Dispatching;

namespace Opine.Plugins
{
    public interface IPlugin
    {
        void ParseArguments(string[] args);
        void RegisterServices(IServiceCollection collection);
        void RegisterHandlers(IHandlerRegistry handlerRegistry);
    }
}