// <copyright file="Configuration.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Umbraco.Community.ImageQualityReducer
{
    /// <summary>
    /// The configuration for the image quality reducer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class Configuration
    {
        /// <summary>
        /// The config section name.
        /// </summary>
        public const string SectionName = "Umbraco:Community:ImageQualityReducer";

        /// <summary>
        /// Gets or sets a value indicating whether image quality reducing is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the image quality should be set as a querystring parameter.
        /// </summary>
        public bool UseQueryString { get; set; }

        /// <summary>
        /// Gets or sets the default image quality.
        /// </summary>
        /// <remarks>Defaults to 85. This will only be applied when no image quality is specified when generating a crop url.</remarks>
        public int DefaultImageQuality { get; set; } = 85;

        /// <summary>
        /// Gets or sets extension specific image quality settings.
        /// </summary>
        /// <remarks>By default nothing is set. If a specific value is not found the <see cref="DefaultImageQuality"/> value will be used.</remarks>
        public IDictionary<string, int> ExtensionSpecificImageQuality { get; set; } = new Dictionary<string, int>();
    }
}
