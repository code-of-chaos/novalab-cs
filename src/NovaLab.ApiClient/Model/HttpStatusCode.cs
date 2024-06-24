/*
 * NovaLab API
 *
 * An ASP.NET Core Web API for managing your streams
 *
 * The version of the OpenAPI document: v1
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = NovaLab.ApiClient.Client.OpenAPIDateConverter;

namespace NovaLab.ApiClient.Model
{
    /// <summary>
    /// Defines HttpStatusCode
    /// </summary>
    public enum HttpStatusCode
    {
        /// <summary>
        /// Enum NUMBER_100 for value: 100
        /// </summary>
        NUMBER_100 = 100,

        /// <summary>
        /// Enum NUMBER_101 for value: 101
        /// </summary>
        NUMBER_101 = 101,

        /// <summary>
        /// Enum NUMBER_102 for value: 102
        /// </summary>
        NUMBER_102 = 102,

        /// <summary>
        /// Enum NUMBER_103 for value: 103
        /// </summary>
        NUMBER_103 = 103,

        /// <summary>
        /// Enum NUMBER_200 for value: 200
        /// </summary>
        NUMBER_200 = 200,

        /// <summary>
        /// Enum NUMBER_201 for value: 201
        /// </summary>
        NUMBER_201 = 201,

        /// <summary>
        /// Enum NUMBER_202 for value: 202
        /// </summary>
        NUMBER_202 = 202,

        /// <summary>
        /// Enum NUMBER_203 for value: 203
        /// </summary>
        NUMBER_203 = 203,

        /// <summary>
        /// Enum NUMBER_204 for value: 204
        /// </summary>
        NUMBER_204 = 204,

        /// <summary>
        /// Enum NUMBER_205 for value: 205
        /// </summary>
        NUMBER_205 = 205,

        /// <summary>
        /// Enum NUMBER_206 for value: 206
        /// </summary>
        NUMBER_206 = 206,

        /// <summary>
        /// Enum NUMBER_207 for value: 207
        /// </summary>
        NUMBER_207 = 207,

        /// <summary>
        /// Enum NUMBER_208 for value: 208
        /// </summary>
        NUMBER_208 = 208,

        /// <summary>
        /// Enum NUMBER_226 for value: 226
        /// </summary>
        NUMBER_226 = 226,

        /// <summary>
        /// Enum NUMBER_300 for value: 300
        /// </summary>
        NUMBER_300 = 300,

        /// <summary>
        /// Enum NUMBER_301 for value: 301
        /// </summary>
        NUMBER_301 = 301,

        /// <summary>
        /// Enum NUMBER_302 for value: 302
        /// </summary>
        NUMBER_302 = 302,

        /// <summary>
        /// Enum NUMBER_303 for value: 303
        /// </summary>
        NUMBER_303 = 303,

        /// <summary>
        /// Enum NUMBER_304 for value: 304
        /// </summary>
        NUMBER_304 = 304,

        /// <summary>
        /// Enum NUMBER_305 for value: 305
        /// </summary>
        NUMBER_305 = 305,

        /// <summary>
        /// Enum NUMBER_306 for value: 306
        /// </summary>
        NUMBER_306 = 306,

        /// <summary>
        /// Enum NUMBER_307 for value: 307
        /// </summary>
        NUMBER_307 = 307,

        /// <summary>
        /// Enum NUMBER_308 for value: 308
        /// </summary>
        NUMBER_308 = 308,

        /// <summary>
        /// Enum NUMBER_400 for value: 400
        /// </summary>
        NUMBER_400 = 400,

        /// <summary>
        /// Enum NUMBER_401 for value: 401
        /// </summary>
        NUMBER_401 = 401,

        /// <summary>
        /// Enum NUMBER_402 for value: 402
        /// </summary>
        NUMBER_402 = 402,

        /// <summary>
        /// Enum NUMBER_403 for value: 403
        /// </summary>
        NUMBER_403 = 403,

        /// <summary>
        /// Enum NUMBER_404 for value: 404
        /// </summary>
        NUMBER_404 = 404,

        /// <summary>
        /// Enum NUMBER_405 for value: 405
        /// </summary>
        NUMBER_405 = 405,

        /// <summary>
        /// Enum NUMBER_406 for value: 406
        /// </summary>
        NUMBER_406 = 406,

        /// <summary>
        /// Enum NUMBER_407 for value: 407
        /// </summary>
        NUMBER_407 = 407,

        /// <summary>
        /// Enum NUMBER_408 for value: 408
        /// </summary>
        NUMBER_408 = 408,

        /// <summary>
        /// Enum NUMBER_409 for value: 409
        /// </summary>
        NUMBER_409 = 409,

        /// <summary>
        /// Enum NUMBER_410 for value: 410
        /// </summary>
        NUMBER_410 = 410,

        /// <summary>
        /// Enum NUMBER_411 for value: 411
        /// </summary>
        NUMBER_411 = 411,

        /// <summary>
        /// Enum NUMBER_412 for value: 412
        /// </summary>
        NUMBER_412 = 412,

        /// <summary>
        /// Enum NUMBER_413 for value: 413
        /// </summary>
        NUMBER_413 = 413,

        /// <summary>
        /// Enum NUMBER_414 for value: 414
        /// </summary>
        NUMBER_414 = 414,

        /// <summary>
        /// Enum NUMBER_415 for value: 415
        /// </summary>
        NUMBER_415 = 415,

        /// <summary>
        /// Enum NUMBER_416 for value: 416
        /// </summary>
        NUMBER_416 = 416,

        /// <summary>
        /// Enum NUMBER_417 for value: 417
        /// </summary>
        NUMBER_417 = 417,

        /// <summary>
        /// Enum NUMBER_421 for value: 421
        /// </summary>
        NUMBER_421 = 421,

        /// <summary>
        /// Enum NUMBER_422 for value: 422
        /// </summary>
        NUMBER_422 = 422,

        /// <summary>
        /// Enum NUMBER_423 for value: 423
        /// </summary>
        NUMBER_423 = 423,

        /// <summary>
        /// Enum NUMBER_424 for value: 424
        /// </summary>
        NUMBER_424 = 424,

        /// <summary>
        /// Enum NUMBER_426 for value: 426
        /// </summary>
        NUMBER_426 = 426,

        /// <summary>
        /// Enum NUMBER_428 for value: 428
        /// </summary>
        NUMBER_428 = 428,

        /// <summary>
        /// Enum NUMBER_429 for value: 429
        /// </summary>
        NUMBER_429 = 429,

        /// <summary>
        /// Enum NUMBER_431 for value: 431
        /// </summary>
        NUMBER_431 = 431,

        /// <summary>
        /// Enum NUMBER_451 for value: 451
        /// </summary>
        NUMBER_451 = 451,

        /// <summary>
        /// Enum NUMBER_500 for value: 500
        /// </summary>
        NUMBER_500 = 500,

        /// <summary>
        /// Enum NUMBER_501 for value: 501
        /// </summary>
        NUMBER_501 = 501,

        /// <summary>
        /// Enum NUMBER_502 for value: 502
        /// </summary>
        NUMBER_502 = 502,

        /// <summary>
        /// Enum NUMBER_503 for value: 503
        /// </summary>
        NUMBER_503 = 503,

        /// <summary>
        /// Enum NUMBER_504 for value: 504
        /// </summary>
        NUMBER_504 = 504,

        /// <summary>
        /// Enum NUMBER_505 for value: 505
        /// </summary>
        NUMBER_505 = 505,

        /// <summary>
        /// Enum NUMBER_506 for value: 506
        /// </summary>
        NUMBER_506 = 506,

        /// <summary>
        /// Enum NUMBER_507 for value: 507
        /// </summary>
        NUMBER_507 = 507,

        /// <summary>
        /// Enum NUMBER_508 for value: 508
        /// </summary>
        NUMBER_508 = 508,

        /// <summary>
        /// Enum NUMBER_510 for value: 510
        /// </summary>
        NUMBER_510 = 510,

        /// <summary>
        /// Enum NUMBER_511 for value: 511
        /// </summary>
        NUMBER_511 = 511
    }

}
