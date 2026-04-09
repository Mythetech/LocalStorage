# Mythetech LocalStorage

A library that provides access to the browser's local storage APIs for Blazor applications. An additional benefit of using this library is that it will handle serializing and deserializing values when saving or retrieving them.

> **Attribution:** This project is a fork of [Blazored.LocalStorage](https://github.com/Blazored/LocalStorage) originally created by Chris Sainty. We thank Chris for his work on the original library.

## Installing

To install the package add the following line to your csproj file replacing x.x.x with the latest version number:

```xml
<PackageReference Include="Mythetech.LocalStorage" Version="x.x.x" />
```

You can also install via the .NET CLI with the following command:

```
dotnet add package Mythetech.LocalStorage
```

## Setup

Register the local storage services with the service collection in your _Program.cs_ file.

```c#
builder.Services.AddLocalStorage();
```

### Registering services as Singleton - Blazor WebAssembly **ONLY**
99% of developers will want to register Mythetech LocalStorage using the method described above. However, in some very specific scenarios
developers may have a need to register services as Singleton as opposed to Scoped. This is possible by using the following method:

```csharp
builder.Services.AddLocalStorageAsSingleton();
```

This method will not work with Blazor Server applications as Blazor's JS interop services are registered as Scoped and cannot be injected into Singletons.

### Using JS Interop Streaming
When using interactive components in server-side apps JS Interop calls are limited to the configured SignalR message size (default: 32KB).
Therefore when attempting to store or retrieve an object larger than this in LocalStorage the call will fail with a SignalR exception.

The following streaming implementation can be used to remove this limit (you will still be limited by the browser).

Register the streaming local storage service:

```c#
builder.Services.AddLocalStorageStreaming();
```

Add the JavaScript file to your _App.razor_:

```html
<script src="_content/Mythetech.LocalStorage/Mythetech.LocalStorage.js"></script>
```

## Usage (Blazor WebAssembly)
To use Mythetech.LocalStorage in Blazor WebAssembly, inject the `ILocalStorageService` per the example below.

```c#
@inject Mythetech.LocalStorage.ILocalStorageService localStorage

@code {

    protected override async Task OnInitializedAsync()
    {
        await localStorage.SetItemAsync("name", "John Smith");
        var name = await localStorage.GetItemAsync<string>("name");
    }

}
```

With Blazor WebAssembly you also have the option of a synchronous API, if your use case requires it. You can swap the `ILocalStorageService` for `ISyncLocalStorageService` which allows you to avoid use of `async`/`await`. For either interface, the method names are the same.

```c#
@inject Mythetech.LocalStorage.ISyncLocalStorageService localStorage

@code {

    protected override void OnInitialized()
    {
        localStorage.SetItem("name", "John Smith");
        var name = localStorage.GetItem<string>("name");
    }

}
```

## Usage (Blazor Server)

**NOTE:** Due to pre-rendering in Blazor Server you can't perform any JS interop until the `OnAfterRender` lifecycle method.

```c#
@inject Mythetech.LocalStorage.ILocalStorageService localStorage

@code {

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await localStorage.SetItemAsync("name", "John Smith");
        var name = await localStorage.GetItemAsync<string>("name");
    }

}
```

The APIs available are:

- asynchronous via `ILocalStorageService`:
  - SetItemAsync()
  - SetItemAsStringAsync()
  - GetItemAsync()
  - GetItemAsStringAsync()
  - RemoveItemAsync()
  - RemoveItemsAsync()
  - ClearAsync()
  - LengthAsync()
  - KeyAsync()
  - KeysAsync()
  - ContainKeyAsync()

- synchronous via `ISyncLocalStorageService` (Synchronous methods are **only** available in Blazor WebAssembly):
  - SetItem()
  - SetItemAsString()
  - GetItem()
  - GetItemAsString()
  - RemoveItem()
  - RemoveItems()
  - Clear()
  - Length()
  - Key()
  - Keys()
  - ContainKey()

**Note:** Mythetech.LocalStorage methods will handle the serialisation and de-serialisation of the data for you, the exceptions are the `SetItemAsString[Async]` and `GetItemAsString[Async]` methods which will save and return raw string values from local storage.

## Configuring JSON Serializer Options
You can configure the options for the default serializer (System.Text.Json) when calling the `AddLocalStorage` method to register services.

```c#
builder.Services.AddLocalStorage(config =>
    config.JsonSerializerOptions.WriteIndented = true
);
```

## Using a custom JSON serializer
By default, the library uses `System.Text.Json`. If you prefer to use a different JSON library for serialization--or if you want to add some custom logic when serializing or deserializing--you can provide your own serializer which implements the `Mythetech.LocalStorage.Serialization.IJsonSerializer` interface.

To register your own serializer in place of the default one, you can do the following:

```csharp
builder.Services.AddLocalStorage();
builder.Services.Replace(ServiceDescriptor.Scoped<IJsonSerializer, MySerializer>());
```

## Testing with bUnit
The `Mythetech.LocalStorage.TestExtensions` package provides test extensions for use with the [bUnit testing library](https://bunit.dev/). Using these test extensions will provide an in memory implementation which mimics local storage allowing more realistic testing of your components.

### Installing

To install the package add the following line to your csproj file replacing x.x.x with the latest version number:

```xml
<PackageReference Include="Mythetech.LocalStorage.TestExtensions" Version="x.x.x" />
```

You can also install via the .NET CLI with the following command:

```
dotnet add package Mythetech.LocalStorage.TestExtensions
```

### Usage example

Below is an example test which uses these extensions.

```c#
public class IndexPageTests : TestContext
{
    [Fact]
    public async Task SavesNameToLocalStorage()
    {
        // Arrange
        const string inputName = "John Smith";
        var localStorage = this.AddLocalStorage();
        var cut = RenderComponent<BlazorWebAssembly.Pages.Index>();

        // Act
        cut.Find("#Name").Change(inputName);
        cut.Find("#NameButton").Click();

        // Assert
        var name = await localStorage.GetItemAsync<string>("name");

        Assert.Equal(inputName, name);
    }
}
```

## Migration from Blazored.LocalStorage

If you're migrating from Blazored.LocalStorage, update your package references and namespaces:

1. Replace `Blazored.LocalStorage` with `Mythetech.LocalStorage` in your csproj files
2. Replace `using Blazored.LocalStorage` with `using Mythetech.LocalStorage` in your code
3. Update any script references from `Blazored.LocalStorage.js` to `Mythetech.LocalStorage.js`
4. Rename `AddBlazoredLocalStorage()` to `AddLocalStorage()`, `AddBlazoredLocalStorageAsSingleton()` to `AddLocalStorageAsSingleton()`, and `AddBlazoredLocalStorageStreaming()` to `AddLocalStorageStreaming()`
