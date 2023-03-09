// <copyright file="DecoratedImageUrlGeneratorTests.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.Options;
using Moq;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.ImageQualityReducer.Tests
{
    [TestFixture]
    internal class DecoratedImageUrlGeneratorTests
    {
        private Mock<IOptions<Configuration>> configurationMock = null!;
        private DecoratedImageUrlGenerator imageUrlGenerator = null!;
        private Mock<IImageUrlGenerator> innerGeneratorMock = null!;
        private Configuration config = null!;

        [SetUp]
        public void SetUp()
        {
            this.config = new Configuration();

            this.configurationMock = new Mock<IOptions<Configuration>>();
            this.configurationMock.SetupGet(x => x.Value).Returns(this.config);

            this.innerGeneratorMock = new Mock<IImageUrlGenerator>();
            this.innerGeneratorMock.Setup(x => x.GetImageUrl(It.IsAny<ImageUrlGenerationOptions>()))
                .Returns("http://www.foo.jpg");

            this.imageUrlGenerator =
                new DecoratedImageUrlGenerator(this.innerGeneratorMock.Object, this.configurationMock.Object);
        }

        [Test]
        public void When_Disabled_Default_ImageUrl_ShouldBe_Generated()
        {
            var options = new ImageUrlGenerationOptions("http://www.foo.jpg");

            var result = this.imageUrlGenerator.GetImageUrl(options);

            this.innerGeneratorMock.Verify(x => x.GetImageUrl(options), Times.Once());
        }

        [Test]
        public void When_UseQueryString_Is_Disable_Default_ImageUrl_ShouldBe_Generated()
        {
            this.config.Enabled = true;
            this.config.UseQueryString = false;

            var options = new ImageUrlGenerationOptions("http://www.foo.jpg");

            var result = this.imageUrlGenerator.GetImageUrl(options);

            this.innerGeneratorMock.Verify(x => x.GetImageUrl(options), Times.Once());
        }

        [Test]
        public void When_ImageUrl_Is_Null_Default_ImageUrl_ShouldBe_Generated()
        {
            this.config.Enabled = true;
            this.config.UseQueryString = true;

            var options = new ImageUrlGenerationOptions(null);

            var result = this.imageUrlGenerator.GetImageUrl(options);

            this.innerGeneratorMock.Verify(x => x.GetImageUrl(options), Times.Once());
        }

        [Test]
        public void When_Quality_Is_Set_Default_ImageUrl_ShouldBe_Generated()
        {
            this.config.Enabled = true;
            this.config.UseQueryString = true;

            var options = new ImageUrlGenerationOptions("http://www.foo.jpg") { Quality = 80 };

            var result = this.imageUrlGenerator.GetImageUrl(options);

            this.innerGeneratorMock.Verify(x => x.GetImageUrl(options), Times.Once());
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public void When_Quality_Not_Set_Quality_Should_Be_Applied(bool hasFormat)
        {
            this.config.Enabled = true;
            this.config.UseQueryString = true;
            this.config.DefaultImageQuality = 75;

            if (hasFormat)
            {
                this.config.ExtensionSpecificImageQuality = new Dictionary<string, int>
                {
                    { "jpg", 65 },
                    { "webp", 45 },
                };
            }

            var options = new ImageUrlGenerationOptions("http://www.foo.jpg");

            if (hasFormat)
            {
                options.FurtherOptions = "format=webp";
            }

            this.innerGeneratorMock.Setup(x => x.GetImageUrl(It.IsAny<ImageUrlGenerationOptions>()))
                .Returns("http://www.foo.jpg");

            var result = this.imageUrlGenerator.GetImageUrl(options);

            this.innerGeneratorMock.Verify(x => x.GetImageUrl(It.IsAny<ImageUrlGenerationOptions>()), Times.Once);

            if (hasFormat)
            {
                Assert.That(options.Quality, Is.EqualTo(this.config.ExtensionSpecificImageQuality["webp"]));
            }
            else
            {
                Assert.That(options.Quality, Is.EqualTo(this.config.DefaultImageQuality));
            }
        }
    }
}
