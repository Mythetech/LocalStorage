using System;
using System.Text.Json;
using System.Threading.Tasks;
using Mythetech.LocalStorage.JsonConverters;
using Mythetech.LocalStorage.Serialization;
using Mythetech.LocalStorage.StorageOptions;
using Mythetech.LocalStorage.TestExtensions;
using Mythetech.LocalStorage.Tests.TestAssets;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Mythetech.LocalStorage.Tests.LocalStorageServiceTests
{
    public class RemoveItemAsync
    {
        private readonly LocalStorageService _sut;
        private readonly IStorageProvider _storageProvider;

        private const string Key = "testKey";

        public RemoveItemAsync()
        {
            var mockOptions = new Mock<IOptions<LocalStorageOptions>>();
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new TimespanJsonConverter());
            mockOptions.Setup(u => u.Value).Returns(new LocalStorageOptions());
            IJsonSerializer serializer = new SystemTextJsonSerializer(mockOptions.Object);
            _storageProvider = new InMemoryStorageProvider();
            _sut = new LocalStorageService(_storageProvider, serializer);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task ThrowsArgumentNullException_When_KeyIsInvalid(string key)
        {
            // assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RemoveItemAsync(key).AsTask());
        }

        [Fact]
        public async Task RemovesItemFromStoreIfExists()
        {
            // Arrange
            var data = new TestObject(2, "Jane Smith");
            await _sut.SetItemAsync(Key, data);

            // Act
            await _sut.RemoveItemAsync(Key);

            // Assert
            Assert.Equal(0, await _storageProvider.LengthAsync());
        }
        
        [Fact]
        public async Task DoesNothingWhenItemDoesNotExistInStore()
        {
            // Act
            await _sut.RemoveItemAsync(Key);

            // Assert
            Assert.Equal(0, await _storageProvider.LengthAsync());
        }
    }
}
