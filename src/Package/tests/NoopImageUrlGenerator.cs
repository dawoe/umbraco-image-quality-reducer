// <copyright file="NoopImageUrlGenerator.cs" company="Umbraco Community">
// Copyright (c) Dave Woestenborghs and contributors
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;

namespace Umbraco.Community.ImageQualityReducer.Tests
{
    [ExcludeFromCodeCoverage]
    internal class NoopImageUrlGenerator : IImageUrlGenerator
    {
        public IEnumerable<string> SupportedImageFileTypes { get; } = new List<string>() { "jpeg", "webp", "png" };

        public string? GetImageUrl(ImageUrlGenerationOptions options) => options.ImageUrl;
    }
}
