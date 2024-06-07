
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaosAPI.Contracts;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Net;

namespace CodeOfChaos.AspNetCore.API;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record ApiResult<T>(
    [property: Description("Status code of the HTTP Response"), ReadOnly(true)]
    HttpStatusCode Status, 
    
    [property: Description("Possible Message to explain error code"), ReadOnly(true)]
    string? Message, 
    
    [property: Description("Response data, can be more than one entry."), ReadOnly(true)] 
    T[] Data
) : IApiResult<T> {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static ApiResult<T> FailureServer(HttpStatusCode? status = null, string? msg = null) {
        return new ApiResult<T>(status ?? HttpStatusCode.InternalServerError, msg, []);
    }
    
    public static ApiResult<T> FailureClient(HttpStatusCode? status = null, string? msg = null) {
        return new ApiResult<T>(status ?? HttpStatusCode.BadRequest, msg, []);
    }

    public static ApiResult<T> Success(params T[] objects) => Success(null,null, objects);
    public static ApiResult<T> Success(HttpStatusCode? status = null, string? msg = null, params T[] objects) {
        return new ApiResult<T>(status ?? HttpStatusCode.OK, msg ?? "", objects);
    }
}

[UsedImplicitly]
public record ApiResult(HttpStatusCode Status, string? Message, object[] Data) : ApiResult<object>(Status, Message, Data);