using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace StructuredCablingStudio.Extensions.IServiceCollectionExtensions
{
    public static class AddLocalizationBasisExtension
    {
        public static IServiceCollection AddLocalizationBasis(this IServiceCollection services)
            => services.AddLocalization(options => options.ResourcesPath = "Resources").Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("ru"),
                    new CultureInfo("en"),
                    new CultureInfo("uk")
                };
                options.DefaultRequestCulture = new RequestCulture("ru");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
    }
}
