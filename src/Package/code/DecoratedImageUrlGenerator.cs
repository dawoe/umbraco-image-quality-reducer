// <copyright file="DecoratedImageUrlGenerator.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.ImageQualityReducer
{
    /// <summary>
    /// A decorated <see cref="IImageUrlGenerator"/> that allows us to append the image quality querystring if needed.
    /// </summary>
    internal sealed class DecoratedImageUrlGenerator : IImageUrlGenerator
    {
        private readonly IImageUrlGenerator innerGenerator;
        private readonly IOptions<Configuration> configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedImageUrlGenerator"/> class.
        /// </summary>
        /// <param name="innerGenerator">The <see cref="IImageUrlGenerator"/> implementation were a going to decorate.</param>
        /// <param name="configuration">A <see cref="IOptions{TOptions}"/>.</param>
        public DecoratedImageUrlGenerator(IImageUrlGenerator innerGenerator, IOptions<Configuration> configuration)
        {
            this.innerGenerator = innerGenerator;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public IEnumerable<string> SupportedImageFileTypes => this.innerGenerator.SupportedImageFileTypes;

        /// <inheritdoc/>
        public string? GetImageUrl(ImageUrlGenerationOptions options) => this.innerGenerator.GetImageUrl(options);
    }
}
