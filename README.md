# 🌟 APItizing 🌟

This .NET 8 Web API is not just your average API—it's a thrilling adventure through the world of HTTP Verbs! 🚀 Join us as we demonstrate the magic of GET, POST, PUT, DELETE, and sometimes PATCH in action. Whether you're a seasoned developer or just starting out, this repo is your gateway to mastering the art of API communication. Let's explore, learn, and have some fun with HTTP Verbs together! 🎉

## Self Documenting API

One step towards exposing a user-friendly API is self-documentation. This is made extremely easy in the default .NET Web API template with the automatic addition of [Swagger](https://swagger.io/). Let's take this further!

1. Check out our implementation of `HttpVerbResponsesOperationFilter`. This class automatically adds the expected response types to each API endpoint in Swagger. This can also be achieved by decorating your endpoints with the `[SwaggerResponse]` attribute, however I prefer this automated method.
2. Are you already doing your due diligence with detailed XML documentation? Why not automatically expose this to your API consumers through swagger using [this guide](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio#xml-comments).

Documentation is only as useful as it is kept up to date, so I recommend these self-documentation techniques that remain close to the code.

# Explore HTTP Verbs through Swagger

Run the API to use Swagger to learn about the typical response types and idempotency for each HTTP Verb. These are important to understand because these are the behaviors that API consumers will expect, but it is on the API developer to make sure we adhere to these rules.

## To 404, or not to 404

Browsing through the different API endpoints, did you notice how some endpoints return 404 while others do not? This is intentional!

The reason for this is that 404 represents Resource Not Found. That is why only the endpoints with an ID in the route (therefore referencing a specific resource) return 404 if the resource is not found.

The default GET endpoint without an ID in the route should never explicitly return 404 because the API consumer would interpret this as the API itself being inaccessible. The correct thing to do in this instance is to return an empty collection. If you absolutely must indicate that the resource does not exist, consider a different response type such as 204 which indicates a successful request but no content to return.

## What's up with PATCH

HTTP PATCH is like PUT, however it allows for partial updates to a resource instead of a full update to the resource. There are pros and cons to each approach and multiple different frameworks to utilize PATCH.

In most cases it is probably best to utilize PUT because it is the simplest approach to update the full model on each request. However, the downside to this is that you expend effort in updating fields that don't matter (which is usually negligible) and you open yourself up to miss an update to a field or update a field by mistake. PATCH mitigates these issues by only changing the fields specified in the request, with the tradeoff of added complexity.

Compare these scenarios from the sample API that could mimic real-life scenarios for PUT vs PATCH.

PUT
```c#
// Using Entity Framework you either have to update the existing model properties or bind the new model to the context and mark it as modified
dbSnack.Name = snack.Name;
dbSnack.Calories = snack.Calories;
```
In this scenario we update each DB field based on the request body.

* The API user could accidentally overwrite a field that we were not intending to if they exclude it from their request payload.
* If the API developer does not properly update the request payload to map to a new database field, the API user could be left baffled when their field isn't changing.

PATCH
```c#
snackPatch.ApplyTo(dbSnack);
```

In this scenario, only explicit changes in the `snackPatch` document will be applied to the database model. 

For this API I have used the [JSON PATCH](https://jsonpatch.com/) standard through the [Newtonsoft](https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-8.0) implementation. However, there are other implementations such as [OData's](https://learn.microsoft.com/en-us/odata/webapi-8/fundamentals/entity-routing?tabs=net60%2Cvisual-studio#patching-a-single-entity).

### So what are the tradeoffs?

This comes at the cost of added complexity. For example using JSON PATCH, your client needs knowledge of the different patch operations that exist and needs to be able to create a request body containing them. Compare the requests for this API.

PUT
```json
{
"id": 5,
"name": "Dark Chocolate",
"calories": 250
}
```
PATCH
```json
[
  {
    "path": "/calories",
    "op": "replace",
    "value": "250"
  }
]
```

Also, I don't recommend updating items of a child collection via this method as they use the index of the element in the collection to update them. Attempting to keep collection indexes in sync between client in server seems to be much more trouble than it is worth. If I were to use this in a real project, I would likely roll my own middleware to detect the usage of collection indexing in the PATCH operations and return a 400 (bad request).

In my opinion, the OData Delta implementation is much simpler to use and if there were a similar alternative I would pursue that.