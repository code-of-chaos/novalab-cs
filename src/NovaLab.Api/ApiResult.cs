// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using System.Net;

namespace NovaLab.Api;

using JetBrains.Annotations;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public record ApiResult<T>(
    HttpStatusCode Status, 
    string? Message, 
    T[] Data
    
    ) {

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