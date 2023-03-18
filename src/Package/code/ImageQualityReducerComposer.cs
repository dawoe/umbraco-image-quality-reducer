// <copyright file="ImageQualityReducerComposer.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Media;

namespace Umbraco.Community.ImageQualityReducer
{
    /// <summary>
    /// Composer that sets up the image quality reducer.
    /// </summary>
    internal sealed class ImageQualityReducerComposer : IComposer
    {
        /// <inheritdoc/>
        public void Compose(IUmbracoBuilder builder)
        {
            this.LoadConfiguration(builder);
            this.Configure(builder);
        }

        private void Configure(IUmbracoBuilder builder)
        {
            var config = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>();

            if (config.Value.Enabled == false)
            {
                return;
            }

            if (config.Value.UseQueryString)
            {
                this.DecorateImageUrlGenerator(builder, config);
            }
            else
            {
                builder.Services.ConfigureOptions<ImageSharpMiddlewareOptionsConfiguration>();
            }
        }

        private void DecorateImageUrlGenerator(IUmbracoBuilder builder, IOptions<Configuration> config)
        {
            // get the actual instance
            var instance = builder.Services.BuildServiceProvider().GetRequiredService<IImageUrlGenerator>();

            // add decorated instance.
            builder.Services.AddSingleton<IImageUrlGenerator>(_ => new DecoratedImageUrlGenerator(instance, config));
        }

        private void LoadConfiguration(IUmbracoBuilder builder) => builder.Services.AddOptions<Configuration>().Bind(builder.Config.GetSection(Configuration.SectionName));
    }
}
