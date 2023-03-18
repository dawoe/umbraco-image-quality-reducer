// <copyright file="ImageSharpMiddlewareOptionsConfiguration.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using Umbraco.Extensions;

namespace Umbraco.Community.ImageQualityReducer
{
    /// <summary>
    /// Configuration for image sharp middleware.
    /// </summary>
    internal sealed class ImageSharpMiddlewareOptionsConfiguration : IConfigureOptions<ImageSharpMiddlewareOptions>
    {
        private readonly IOptions<Configuration> configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSharpMiddlewareOptionsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">A <see cref="IOptions{TOptions}"/>.</param>
        public ImageSharpMiddlewareOptionsConfiguration(IOptions<Configuration> configuration) => this.configuration = configuration;

        /// <inheritdoc/>
        public void Configure(ImageSharpMiddlewareOptions options)
        {
            if (this.configuration.Value.Enabled == false || this.configuration.Value.UseQueryString)
            {
                return;
            }

            options.OnParseCommandsAsync = this.ParseCommandAsyncHandler;
        }

        internal Task ParseCommandAsyncHandler(ImageCommandContext context)
        {
            if (context.Commands.Contains(QualityWebProcessor.Quality))
            {
                return Task.CompletedTask;
            }

            context.Commands.Add(QualityWebProcessor.Quality, this.GetImageQualityForRequest(context));

            return Task.CompletedTask;
        }

        private string GetImageQualityForRequest(ImageCommandContext context)
        {
            var quality = this.configuration.Value.DefaultImageQuality.ToString();

            if (this.configuration.Value.ExtensionSpecificImageQuality.Count == 0)
            {
                return quality;
            }

            var extension = this.GetImageType(context);

            if (this.configuration.Value.ExtensionSpecificImageQuality.ContainsKey(extension) == false)
            {
                return quality;
            }

            return this.configuration.Value.ExtensionSpecificImageQuality[extension].ToString();
        }

        private string GetImageType(ImageCommandContext context)
        {
            var extension = context.Context.Request.Path.Value!.GetFileExtension().TrimStart(".");

            if (!context.Commands.Contains(FormatWebProcessor.Format))
            {
                return extension;
            }

            var format = context.Commands[FormatWebProcessor.Format];

            if (string.IsNullOrWhiteSpace(format) == false)
            {
                extension = format;
            }

            return extension;
        }
    }
}
