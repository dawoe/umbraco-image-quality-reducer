// <copyright file="ImageQualityReducerComposer.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco.Community.ImageQualityReducer
{
    /// <summary>
    /// Composer that sets up the image quality reducer.
    /// </summary>
    internal sealed class ImageQualityReducerComposer : IComposer
    {
        /// <inheritdoc/>
        public void Compose(IUmbracoBuilder builder) => this.LoadConfiguration(builder);

        private void LoadConfiguration(IUmbracoBuilder builder) => builder.Services.AddOptions<Configuration>().Bind(builder.Config.GetSection(Configuration.SectionName));
    }
}
