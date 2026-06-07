using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;

namespace DlmsWebApi
{
    /// <summary>
    /// Configures Swagger generation options to create one Swagger doc per discovered API version.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var info = new OpenApiInfo()
                {
                    Title = "DlmsWebApi",
                    Version = description.ApiVersion.ToString(),
                    Description = "DlmsWebApi HTTP API",
                };

                if (description.IsDeprecated)
                {
                    info.Description += " (DEPRECATED)";
                }

                options.SwaggerDoc(description.GroupName, info);
            }

            // Ensure actions are included in the correct swagger doc by group name
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.GroupNameEnumerable().Any())
                {
                    return docName == apiDesc.GroupName;
                }

                return apiDesc.GroupName == docName;
            });
        }
    }

    static class ApiDescriptionExtensions
    {
        public static string? GroupName(this Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription desc) => desc.GroupName;

        public static IEnumerable<string> GroupNameEnumerable(this Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription desc)
        {
            if (desc.GroupName is null) yield break;
            yield return desc.GroupName;
        }
    }
}

