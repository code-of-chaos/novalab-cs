/*
 * NovaLab API
 *
 * An ASP.NET Core Web API for managing your streams
 *
 * The version of the OpenAPI document: v1
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using NovaLab.ApiClient.Client;
using NovaLab.ApiClient.Model;

namespace NovaLab.ApiClient.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITwitchManagedRewardApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>TwitchManagedRewardDtoApiResult</returns>
        TwitchManagedRewardDtoApiResult GetManagedRewards(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of TwitchManagedRewardDtoApiResult</returns>
        ApiResponse<TwitchManagedRewardDtoApiResult> GetManagedRewardsWithHttpInfo(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>TwitchManagedRewardDtoApiResult</returns>
        TwitchManagedRewardDtoApiResult PostManagedReward(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of TwitchManagedRewardDtoApiResult</returns>
        ApiResponse<TwitchManagedRewardDtoApiResult> PostManagedRewardWithHttpInfo(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResult</returns>
        ApiResult PostNewLastCleared(string? managedRewardId = default(string?), int operationIndex = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of ApiResult</returns>
        ApiResponse<ApiResult> PostNewLastClearedWithHttpInfo(string? managedRewardId = default(string?), int operationIndex = 0);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITwitchManagedRewardApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of TwitchManagedRewardDtoApiResult</returns>
        System.Threading.Tasks.Task<TwitchManagedRewardDtoApiResult> GetManagedRewardsAsync(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (TwitchManagedRewardDtoApiResult)</returns>
        System.Threading.Tasks.Task<ApiResponse<TwitchManagedRewardDtoApiResult>> GetManagedRewardsWithHttpInfoAsync(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of TwitchManagedRewardDtoApiResult</returns>
        System.Threading.Tasks.Task<TwitchManagedRewardDtoApiResult> PostManagedRewardAsync(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (TwitchManagedRewardDtoApiResult)</returns>
        System.Threading.Tasks.Task<ApiResponse<TwitchManagedRewardDtoApiResult>> PostManagedRewardWithHttpInfoAsync(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResult</returns>
        System.Threading.Tasks.Task<ApiResult> PostNewLastClearedAsync(string? managedRewardId = default(string?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (ApiResult)</returns>
        System.Threading.Tasks.Task<ApiResponse<ApiResult>> PostNewLastClearedWithHttpInfoAsync(string? managedRewardId = default(string?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITwitchManagedRewardApi : ITwitchManagedRewardApiSync, ITwitchManagedRewardApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class TwitchManagedRewardApi : ITwitchManagedRewardApi
    {
        private NovaLab.ApiClient.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitchManagedRewardApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TwitchManagedRewardApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitchManagedRewardApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TwitchManagedRewardApi(string basePath)
        {
            this.Configuration = NovaLab.ApiClient.Client.Configuration.MergeConfigurations(
                NovaLab.ApiClient.Client.GlobalConfiguration.Instance,
                new NovaLab.ApiClient.Client.Configuration { BasePath = basePath }
            );
            this.Client = new NovaLab.ApiClient.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new NovaLab.ApiClient.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = NovaLab.ApiClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitchManagedRewardApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public TwitchManagedRewardApi(NovaLab.ApiClient.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = NovaLab.ApiClient.Client.Configuration.MergeConfigurations(
                NovaLab.ApiClient.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new NovaLab.ApiClient.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new NovaLab.ApiClient.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = NovaLab.ApiClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitchManagedRewardApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public TwitchManagedRewardApi(NovaLab.ApiClient.Client.ISynchronousClient client, NovaLab.ApiClient.Client.IAsynchronousClient asyncClient, NovaLab.ApiClient.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = NovaLab.ApiClient.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public NovaLab.ApiClient.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public NovaLab.ApiClient.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public NovaLab.ApiClient.Client.IReadableConfiguration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public NovaLab.ApiClient.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>TwitchManagedRewardDtoApiResult</returns>
        public TwitchManagedRewardDtoApiResult GetManagedRewards(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0)
        {
            NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult> localVarResponse = GetManagedRewardsWithHttpInfo(userId, limit, includeInvalid);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of TwitchManagedRewardDtoApiResult</returns>
        public NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult> GetManagedRewardsWithHttpInfo(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0)
        {
            NovaLab.ApiClient.Client.RequestOptions localVarRequestOptions = new NovaLab.ApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (userId != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "userId", userId));
            }
            if (limit != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "limit", limit));
            }
            if (includeInvalid != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "include-invalid", includeInvalid));
            }

            localVarRequestOptions.Operation = "TwitchManagedRewardApi.GetManagedRewards";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = this.Client.Get<TwitchManagedRewardDtoApiResult>("/api/twitch/managed-rewards", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetManagedRewards", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of TwitchManagedRewardDtoApiResult</returns>
        public async System.Threading.Tasks.Task<TwitchManagedRewardDtoApiResult> GetManagedRewardsAsync(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult> localVarResponse = await GetManagedRewardsWithHttpInfoAsync(userId, limit, includeInvalid, operationIndex, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"> (optional)</param>
        /// <param name="limit"> (optional)</param>
        /// <param name="includeInvalid"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (TwitchManagedRewardDtoApiResult)</returns>
        public async System.Threading.Tasks.Task<NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult>> GetManagedRewardsWithHttpInfoAsync(string? userId = default(string?), int? limit = default(int?), bool? includeInvalid = default(bool?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            NovaLab.ApiClient.Client.RequestOptions localVarRequestOptions = new NovaLab.ApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (userId != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "userId", userId));
            }
            if (limit != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "limit", limit));
            }
            if (includeInvalid != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "include-invalid", includeInvalid));
            }

            localVarRequestOptions.Operation = "TwitchManagedRewardApi.GetManagedRewards";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.GetAsync<TwitchManagedRewardDtoApiResult>("/api/twitch/managed-rewards", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetManagedRewards", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>TwitchManagedRewardDtoApiResult</returns>
        public TwitchManagedRewardDtoApiResult PostManagedReward(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0)
        {
            NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult> localVarResponse = PostManagedRewardWithHttpInfo(postManagedRewardDto);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of TwitchManagedRewardDtoApiResult</returns>
        public NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult> PostManagedRewardWithHttpInfo(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0)
        {
            NovaLab.ApiClient.Client.RequestOptions localVarRequestOptions = new NovaLab.ApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
                "application/json",
                "text/json",
                "application/*+json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.Data = postManagedRewardDto;

            localVarRequestOptions.Operation = "TwitchManagedRewardApi.PostManagedReward";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = this.Client.Post<TwitchManagedRewardDtoApiResult>("/api/twitch/managed-rewards", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("PostManagedReward", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of TwitchManagedRewardDtoApiResult</returns>
        public async System.Threading.Tasks.Task<TwitchManagedRewardDtoApiResult> PostManagedRewardAsync(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult> localVarResponse = await PostManagedRewardWithHttpInfoAsync(postManagedRewardDto, operationIndex, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="postManagedRewardDto"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (TwitchManagedRewardDtoApiResult)</returns>
        public async System.Threading.Tasks.Task<NovaLab.ApiClient.Client.ApiResponse<TwitchManagedRewardDtoApiResult>> PostManagedRewardWithHttpInfoAsync(PostManagedRewardDto? postManagedRewardDto = default(PostManagedRewardDto?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            NovaLab.ApiClient.Client.RequestOptions localVarRequestOptions = new NovaLab.ApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/*+json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.Data = postManagedRewardDto;

            localVarRequestOptions.Operation = "TwitchManagedRewardApi.PostManagedReward";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.PostAsync<TwitchManagedRewardDtoApiResult>("/api/twitch/managed-rewards", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("PostManagedReward", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResult</returns>
        public ApiResult PostNewLastCleared(string? managedRewardId = default(string?), int operationIndex = 0)
        {
            NovaLab.ApiClient.Client.ApiResponse<ApiResult> localVarResponse = PostNewLastClearedWithHttpInfo(managedRewardId);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <returns>ApiResponse of ApiResult</returns>
        public NovaLab.ApiClient.Client.ApiResponse<ApiResult> PostNewLastClearedWithHttpInfo(string? managedRewardId = default(string?), int operationIndex = 0)
        {
            NovaLab.ApiClient.Client.RequestOptions localVarRequestOptions = new NovaLab.ApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (managedRewardId != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "managedRewardId", managedRewardId));
            }

            localVarRequestOptions.Operation = "TwitchManagedRewardApi.PostNewLastCleared";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = this.Client.Post<ApiResult>("/api/twitch/managed-rewards/clear", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("PostNewLastCleared", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResult</returns>
        public async System.Threading.Tasks.Task<ApiResult> PostNewLastClearedAsync(string? managedRewardId = default(string?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            NovaLab.ApiClient.Client.ApiResponse<ApiResult> localVarResponse = await PostNewLastClearedWithHttpInfoAsync(managedRewardId, operationIndex, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="NovaLab.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="managedRewardId"> (optional)</param>
        /// <param name="operationIndex">Index associated with the operation.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (ApiResult)</returns>
        public async System.Threading.Tasks.Task<NovaLab.ApiClient.Client.ApiResponse<ApiResult>> PostNewLastClearedWithHttpInfoAsync(string? managedRewardId = default(string?), int operationIndex = 0, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            NovaLab.ApiClient.Client.RequestOptions localVarRequestOptions = new NovaLab.ApiClient.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = NovaLab.ApiClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (managedRewardId != null)
            {
                localVarRequestOptions.QueryParameters.Add(NovaLab.ApiClient.Client.ClientUtils.ParameterToMultiMap("", "managedRewardId", managedRewardId));
            }

            localVarRequestOptions.Operation = "TwitchManagedRewardApi.PostNewLastCleared";
            localVarRequestOptions.OperationIndex = operationIndex;


            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.PostAsync<ApiResult>("/api/twitch/managed-rewards/clear", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("PostNewLastCleared", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

    }
}