// <copyright file="DecoratedImageUrlGenerator.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Processors;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

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
        [ExcludeFromCodeCoverage]
        public IEnumerable<string> SupportedImageFileTypes => this.innerGenerator.SupportedImageFileTypes;

        /// <inheritdoc/>
        public string? GetImageUrl(ImageUrlGenerationOptions options)
        {
            if (this.configuration.Value.Enabled == false || this.configuration.Value.UseQueryString == false)
            {
                return this.innerGenerator.GetImageUrl(options);
            }

            if (options.ImageUrl is null)
            {
                return this.innerGenerator.GetImageUrl(options);
            }

            return options.Quality is not null ? this.innerGenerator.GetImageUrl(options) : this.GetImageUrlWithQuality(options);
        }

        private string? GetImageUrlWithQuality(ImageUrlGenerationOptions options)
        {
            options.Quality = this.configuration.Value.DefaultImageQuality;

            var format = options.ImageUrl!.GetFileExtension().TrimStart(".");

            var otherOptions = QueryHelpers.ParseQuery(options.FurtherOptions);

            if (otherOptions.ContainsKey(FormatWebProcessor.Format))
            {
                format = otherOptions[FormatWebProcessor.Format].ToString();
            }

            if (this.configuration.Value.ExtensionSpecificImageQuality.ContainsKey(format))
            {
                options.Quality = this.configuration.Value.ExtensionSpecificImageQuality[format];
            }

            return this.innerGenerator.GetImageUrl(options);
        }
    }
}
