// <copyright file="ImageQualityReducerComposerTests.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Media;

namespace Umbraco.Community.ImageQualityReducer.Tests
{
    [TestFixture]
    internal sealed partial class ImageQualityReducerComposerTests
    {
        private ServiceCollection serviceCollection = null!;
        private IUmbracoBuilder builder = null!;
        private ServiceProvider serviceProvider = null!;

        [Test]
        public void When_No_Config_Present_Default_Config_Should_Be_Returned()
        {
            this.Compose("appSettings.noConfig.json");

            var options = this.serviceProvider.GetRequiredService<IOptions<Configuration>>();

            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Not.Null);
                Assert.That(options.Value.Enabled, Is.False);
                Assert.That(options.Value.UseQueryString, Is.False);
                Assert.That(options.Value.DefaultImageQuality, Is.EqualTo(85));
                Assert.That(options.Value.ExtensionSpecificImageQuality.Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void When_No_Config_Is_Present_It_Should_Be_Returned()
        {
            this.Compose("appSettings.json");

            var options = this.serviceProvider.GetRequiredService<IOptions<Configuration>>();

            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Not.Null);
                Assert.That(options.Value.Enabled, Is.True);
                Assert.That(options.Value.UseQueryString, Is.True);
                Assert.That(options.Value.DefaultImageQuality, Is.EqualTo(80));
                Assert.That(options.Value.ExtensionSpecificImageQuality.Count, Is.EqualTo(2));
                Assert.That(options.Value.ExtensionSpecificImageQuality["jpg"], Is.EqualTo(75));
                Assert.That(options.Value.ExtensionSpecificImageQuality["webp"], Is.EqualTo(60));
            });
        }

        [Test]
        public void When_Enabled_And_UseQueryString_ImageUrlGenerator_Should_Be_Decorated()
        {
            this.Compose("appSettings.json");

            var generator = this.serviceProvider.GetRequiredService<IImageUrlGenerator>();

            Assert.That(generator, Is.InstanceOf<DecoratedImageUrlGenerator>());
        }

        [Test]
        public void When_Enabled_And_UseQueryString_Is_False_ImageUrlGenerator_Should_Not_Be_Decorated()
        {
            this.Compose("appSettings.noQueryString.json");

            var generator = this.serviceProvider.GetRequiredService<IImageUrlGenerator>();

            Assert.That(generator, Is.InstanceOf<NoopImageUrlGenerator>());
        }

        [Test]
        public void When_Disabled_ImageUrlGenerator_Should_Not_Be_Decorated()
        {
            this.Compose("appSettings.noConfig.json");

            var generator = this.serviceProvider.GetRequiredService<IImageUrlGenerator>();

            Assert.That(generator, Is.InstanceOf<NoopImageUrlGenerator>());
        }

        private void Compose(string appSettingsFile)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true);

            var config = configBuilder.Build();
            this.serviceCollection = new ServiceCollection();

            this.serviceCollection.AddSingleton<IImageUrlGenerator, NoopImageUrlGenerator>();

            this.builder = new UmbracoBuilder(this.serviceCollection, config, new TypeLoader(Mock.Of<ITypeFinder>(), Mock.Of<ILogger<TypeLoader>>()));

            var composer = new ImageQualityReducerComposer();

            composer.Compose(this.builder);

            this.serviceProvider = this.serviceCollection.BuildServiceProvider();
        }
    }
}
