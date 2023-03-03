// <copyright file="ImageQualityReducerComposer.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Web.Common.Media;

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
            var config = new Configuration();

            builder.Config.GetSection(Configuration.SectionName).Bind(config);

            if (config.Enabled == false)
            {
                return;
            }

            if (config.UseQueryString)
            {
                this.DecorateImageUrlGenerator(builder);
            }
        }

        private void DecorateImageUrlGenerator(IUmbracoBuilder builder)
        {
            // decorate
            var imageUrlGenerator =
                builder.Services.SingleOrDefault(x => x.ServiceType == typeof(IImageUrlGenerator));

            if (imageUrlGenerator is null)
            {
                return;
            }

            // get the instance
            var instance = builder.Services.BuildServiceProvider().GetRequiredService<IImageUrlGenerator>();

            // remove the existing registration.
            builder.Services.Remove(imageUrlGenerator);

            builder.Services.AddSingleton<IImageUrlGenerator>(_ => new DecoratedImageUrlGenerator(instance));
        }

        private void LoadConfiguration(IUmbracoBuilder builder) => builder.Services.AddOptions<Configuration>().Bind(builder.Config.GetSection(Configuration.SectionName));
    }
}
