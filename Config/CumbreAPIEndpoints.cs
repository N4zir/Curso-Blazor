using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CumbreSalonesCongresos.Config
{
    public static class CumbreAPIEndpoints
    {
        public static WebApplication UseAPIEndpoints(this WebApplication app)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var endpointClasses = currentAssembly.GetTypes().Where(@class => @class is { IsClass: true, IsAbstract: false } && typeof(IEndpoints).IsAssignableFrom(@class));

            foreach (var endpointClass in endpointClasses)
            {
                if (Activator.CreateInstance(endpointClass) is IEndpoints endpoints)
                {
                    endpoints.MapEndpoints(app);
                }
            }
            return app;
        }
    }

    public interface IEndpoints
    {
        void MapEndpoints(IEndpointRouteBuilder app);
    }
}