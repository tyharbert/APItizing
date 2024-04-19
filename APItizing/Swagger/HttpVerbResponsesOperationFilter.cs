using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace APItizing.Swagger
{
    /// <summary>
    /// IOperationFilter implementation to add the standard HTTP Verb Responses to Swagger
    /// </summary>
    public class HttpVerbResponsesOperationFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("401", new OpenApiResponse() { Description = "Unauthorized" });
            operation.Responses.Add("400", new OpenApiResponse() { Description = "Bad Request" });
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Internal Server Error" });

            var method = context.MethodInfo.GetCustomAttributes(true)
                .OfType<HttpMethodAttribute>()
                .Single();

            var template = method.Template ?? string.Empty;

            if ((method is HttpGetAttribute && template.Contains('{')) || method is HttpPutAttribute || method is HttpPatchAttribute)
            {
                operation.Responses.Add("404", new OpenApiResponse() { Description = "Not Found" });
            }

            if (method is HttpDeleteAttribute)
            {
                operation.Responses.Remove("200");
                operation.Responses.Add("204", new OpenApiResponse() { Description = "No Content" });
            }
        }
    }
}
