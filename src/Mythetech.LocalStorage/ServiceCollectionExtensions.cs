using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mythetech.LocalStorage.JsonConverters;
using Mythetech.LocalStorage.Serialization;
using Mythetech.LocalStorage.StorageOptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Mythetech.LocalStorage
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalStorage(this IServiceCollection services)
            => AddLocalStorage(services, null);

        public static IServiceCollection AddLocalStorage(this IServiceCollection services, Action<LocalStorageOptions>? configure)
        {
            services.TryAddScoped<IStorageProvider, BrowserStorageProvider>();
            AddServices(services, configure);
            return services;
        }

        public static IServiceCollection AddLocalStorageStreaming(this IServiceCollection services)
            => AddLocalStorageStreaming(services, null);

        public static IServiceCollection AddLocalStorageStreaming(this IServiceCollection services, Action<LocalStorageOptions>? configure)
        {
            services.TryAddScoped<IStorageProvider, BrowserStreamingStorageProvider>();
            AddServices(services, configure);
            return services;
        }

        private static void AddServices(IServiceCollection services, Action<LocalStorageOptions>? configure)
        {
            services.TryAddScoped<IJsonSerializer, SystemTextJsonSerializer>();
            services.TryAddScoped<ILocalStorageService, LocalStorageService>();
            services.TryAddScoped<ISyncLocalStorageService, LocalStorageService>();
            if (services.All(serviceDescriptor => serviceDescriptor.ServiceType != typeof(IConfigureOptions<LocalStorageOptions>)))
            {
                services.Configure<LocalStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
            }
        }

        /// <summary>
        /// Registers the LocalStorage services as singletons. This should only be used in Blazor WebAssembly applications.
        /// Using this in Blazor Server applications will cause unexpected and potentially dangerous behaviour. 
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddLocalStorageAsSingleton(this IServiceCollection services)
            => AddLocalStorageAsSingleton(services, null);

        /// <summary>
        /// Registers the LocalStorage services as singletons. This should only be used in Blazor WebAssembly applications.
        /// Using this in Blazor Server applications will cause unexpected and potentially dangerous behaviour. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalStorageAsSingleton(this IServiceCollection services, Action<LocalStorageOptions>? configure)
        {
            services.TryAddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
            services.TryAddSingleton<IStorageProvider, BrowserStorageProvider>();
            services.TryAddSingleton<ILocalStorageService, LocalStorageService>();
            services.TryAddSingleton<ISyncLocalStorageService, LocalStorageService>();
            if (services.All(serviceDescriptor => serviceDescriptor.ServiceType != typeof(IConfigureOptions<LocalStorageOptions>)))
            {
                services.Configure<LocalStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
            }
            return services;
        }
    }
}
