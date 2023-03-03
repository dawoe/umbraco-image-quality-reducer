// <copyright file="DecoratedImageUrlGenerator.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.ImageQualityReducer
{
    /// <summary>
    /// A decorated <see cref="IImageUrlGenerator"/> that allows us to append the image quality querystring if needed.
    /// </summary>
    internal sealed class DecoratedImageUrlGenerator: IImageUrlGenerator
    {
        private readonly IImageUrlGenerator innerGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedImageUrlGenerator"/> class.
        /// </summary>
        /// <param name="innerGenerator">The default <see cref="IImageUrlGenerator"/>.</param>
        public DecoratedImageUrlGenerator(IImageUrlGenerator innerGenerator) => this.innerGenerator = innerGenerator;

        /// <inheritdoc/>
        public IEnumerable<string> SupportedImageFileTypes => this.innerGenerator.SupportedImageFileTypes;

        /// <inheritdoc/>
        public string? GetImageUrl(ImageUrlGenerationOptions options) => this.innerGenerator.GetImageUrl(options);
    }
}
