// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel;
using System.Net;

namespace CodeOfChaosAPI.Contracts;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IApiResult<T> {
    [Description("Status code of the HTTP Response"), ReadOnly(true)]
    public HttpStatusCode Status {get; init;}
    
    [Description("Possible Message to explain error code"), ReadOnly(true)]
    public string? Message {get; init;}
    
    [Description("Response data, can be more than one entry."), ReadOnly(true)]
    public T[] Data {get; init;}
}

// Alternative constructors
public interface IApiResult : IApiResult<object>;