// <copyright file="ImageSharpMiddlewareOptionsConfigurationTests.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Commands.Converters;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;

namespace Umbraco.Community.ImageQualityReducer.Tests
{
    [TestFixture]
    internal sealed class ImageSharpMiddlewareOptionsConfigurationTests
    {
        private Mock<IOptions<Configuration>> configurationMock = null!;
        private Configuration config = null!;
        private ImageSharpMiddlewareOptionsConfiguration middlewareOptionsConfiguration = null!;

        [SetUp]
        public void SetUp()
        {
            this.config = new Configuration();

            this.configurationMock = new Mock<IOptions<Configuration>>();
            this.configurationMock.SetupGet(x => x.Value).Returns(this.config);

            this.middlewareOptionsConfiguration =
                new ImageSharpMiddlewareOptionsConfiguration(this.configurationMock.Object);
        }

        [Test]
        public void Configure_Should_Not_Set_OnParseCommandAsync_When_Disabled_In_Config()
        {
            var options = new ImageSharpMiddlewareOptions();

            this.middlewareOptionsConfiguration.Configure(options);

            Assert.That(options.OnParseCommandsAsync.Method.Name, Is.Not.EqualTo(nameof(ImageSharpMiddlewareOptionsConfiguration
                .ParseCommandAsyncHandler)));
        }

        [Test]
        public void Configure_Should_Not_Set_OnParseCommandAsync_When_Enabled_UseQueryString_Is_True_In_Config()
        {
            this.config.Enabled = true;
            this.config.UseQueryString = true;

            var options = new ImageSharpMiddlewareOptions();

            this.middlewareOptionsConfiguration.Configure(options);

            Assert.That(options.OnParseCommandsAsync.Method.Name, Is.Not.EqualTo(nameof(ImageSharpMiddlewareOptionsConfiguration
                .ParseCommandAsyncHandler)));
        }

        [Test]
        public void Configure_Should_Set_OnParseCommandAsync_When_Enabled_UseQueryString_Is_False_In_Config()
        {
            this.config.Enabled = true;

            var options = new ImageSharpMiddlewareOptions();

            this.middlewareOptionsConfiguration.Configure(options);

            Assert.That(options.OnParseCommandsAsync.Method.Name, Is.EqualTo(nameof(ImageSharpMiddlewareOptionsConfiguration
                .ParseCommandAsyncHandler)));
        }

        [Test]
        public void ParseCommandAsyncHandler_Should_Not_Set_Quality_When_Already_Set()
        {
            var defaultQuality = "50";

            var httpContext = new DefaultHttpContext();

            var commandCollection = new CommandCollection
            {
                { QualityWebProcessor.Quality, defaultQuality },
            };

            var context = new ImageCommandContext(httpContext, commandCollection, new CommandParser(new List<ICommandConverter>()), CultureInfo.InvariantCulture);

            this.middlewareOptionsConfiguration.ParseCommandAsyncHandler(context);

            var quality = context.Commands[QualityWebProcessor.Quality];

            Assert.That(quality, Is.EqualTo(defaultQuality));
        }

        [Test]
        public void ParseCommandAsyncHandler_Should_Set_Default_Quality_When_Not_Already_Set()
        {
            var httpContext = new DefaultHttpContext();

            var commandCollection = new CommandCollection
            {
                { "test", "test" },
            };

            var context = new ImageCommandContext(httpContext, commandCollection, new CommandParser(new List<ICommandConverter>()), CultureInfo.InvariantCulture);

            this.middlewareOptionsConfiguration.ParseCommandAsyncHandler(context);

            var quality = context.Commands[QualityWebProcessor.Quality];

            Assert.That(quality, Is.EqualTo(this.config.DefaultImageQuality.ToString()));
        }

        [Test]
        public void ParseCommandAsyncHandler_Should_Set_Quality_From_Specific_Extension()
        {
            var expectedQuality = 50;

            this.config.ExtensionSpecificImageQuality.Add("jpg", expectedQuality);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/image.jpg";

            var commandCollection = new CommandCollection
            {
                { "test", "test" },
            };

            var context = new ImageCommandContext(httpContext, commandCollection, new CommandParser(new List<ICommandConverter>()), CultureInfo.InvariantCulture);

            this.middlewareOptionsConfiguration.ParseCommandAsyncHandler(context);

            var quality = context.Commands[QualityWebProcessor.Quality];

            Assert.That(quality, Is.EqualTo(expectedQuality.ToString()));
        }

        [Test]
        public void ParseCommandAsync_Should_Set_Default_Quality_When_No_Specific_Quality_Is_Found()
        {
            this.config.ExtensionSpecificImageQuality.Add("png", 80);
            this.config.ExtensionSpecificImageQuality.Add("webp", 80);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/image.jpg";

            var commandCollection = new CommandCollection
            {
                { "test", "test" },
            };

            var context = new ImageCommandContext(httpContext, commandCollection, new CommandParser(new List<ICommandConverter>()), CultureInfo.InvariantCulture);

            this.middlewareOptionsConfiguration.ParseCommandAsyncHandler(context);

            var quality = context.Commands[QualityWebProcessor.Quality];

            Assert.That(quality, Is.EqualTo(this.config.DefaultImageQuality.ToString()));
        }

        [Test]
        public void ParseCommandAsyncHandler_Should_Set_Quality_For_Format_Param()
        {
            var expectedQuality = 50;

            this.config.ExtensionSpecificImageQuality.Add("jpg", 80);
            this.config.ExtensionSpecificImageQuality.Add("webp", expectedQuality);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/image.jpg";

            var commandCollection = new CommandCollection
            {
                { "test", "test" },
                { FormatWebProcessor.Format, "webp" },
            };

            var context = new ImageCommandContext(httpContext, commandCollection, new CommandParser(new List<ICommandConverter>()), CultureInfo.InvariantCulture);

            this.middlewareOptionsConfiguration.ParseCommandAsyncHandler(context);

            var quality = context.Commands[QualityWebProcessor.Quality];

            Assert.That(quality, Is.EqualTo(expectedQuality.ToString()));
        }
    }
}
