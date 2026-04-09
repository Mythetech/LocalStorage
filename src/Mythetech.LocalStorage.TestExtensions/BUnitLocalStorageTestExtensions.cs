using System;
using System.Diagnostics.CodeAnalysis;
using Mythetech.LocalStorage;
using Mythetech.LocalStorage.JsonConverters;
using Mythetech.LocalStorage.Serialization;
using Mythetech.LocalStorage.StorageOptions;
using Mythetech.LocalStorage.TestExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bunit
{
    [ExcludeFromCodeCoverage]
    public static class BUnitLocalStorageTestExtensions
    {
        public static ILocalStorageService AddLocalStorage(this TestContextBase context)
            => AddLocalStorage(context, null);

        public static ILocalStorageService AddLocalStorage(this TestContextBase context, Action<LocalStorageOptions>? configure)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var localStorageOptions = new LocalStorageOptions();
            configure?.Invoke(localStorageOptions);
            localStorageOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());

            var localStorageService = new LocalStorageService(new InMemoryStorageProvider(), new SystemTextJsonSerializer(localStorageOptions));
            context.Services.AddSingleton<ILocalStorageService>(localStorageService);

            return localStorageService;
        }
    }
}
