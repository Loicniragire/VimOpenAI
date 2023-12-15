using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualPaymentService.Business.Tests.Extensions
{
    public static class MockHttpMessageHandlerExtension
    {
        /// <summary>
        /// Extension method to mock an Http POST action
        /// </summary>
        public static MockReturn MockPost(this Mock<HttpMessageHandler> target)
        {
            return new MockReturn(target, HttpMethod.Post);
        }

        /// <summary>
        /// Extension method to mock an Http GET action
        /// </summary>
        public static MockReturn MockGet(this Mock<HttpMessageHandler> target)
        {
            return new MockReturn(target, HttpMethod.Get);
        }

        /// <summary>
        /// Extension method to mock an Http PUT action
        /// </summary>
        public static MockReturn MockPut(this Mock<HttpMessageHandler> target)
        {
            return new MockReturn(target, HttpMethod.Put);
        }

        /// <summary>
        /// Extension method to mock an Http PATCH action
        /// </summary>
        public static MockReturn MockPatch(this Mock<HttpMessageHandler> target)
        {
            return new MockReturn(target, HttpMethod.Patch);
        }

        /// <summary>
        /// Extension method to mock an Http DELETE action
        /// </summary>
        public static MockReturn MockDelete(this Mock<HttpMessageHandler> target)
        {
            return new MockReturn(target, HttpMethod.Delete);
        }
    }

    public class MockReturn
    {
        private readonly Mock<HttpMessageHandler> _target;
        private readonly HttpMethod _method;

        /// <summary>
        /// Actions which can be taken on an Http message handler
        /// </summary>
        /// <param name="target">The HttpMessageHandler being mocked</param>
        /// <param name="method">The <see cref="HttpMethod"/> being mocked</param>
        public MockReturn(Mock<HttpMessageHandler> target, HttpMethod method)
        {
            _target = target;
            _method = method;
        }

        /// <summary>
        /// Mocks a Http response and allows the user to see the request body that was sent.
        /// The body is deserialized for the developer.
        /// </summary>
        /// <typeparam name="TRequest">The Type of the request body being sent.</typeparam>
        /// <param name="statusCode">The desired response <see cref="HttpStatusCode"/></param>
        /// <param name="mockResponse">The desired response body</param>
        /// <returns>
        /// <see cref="Task"/> of a 
        /// <see cref="Tuple{T1, T2}"/>(<typeparamref name="TRequest"/>, 
        /// <see cref="Uri"/>), 
        /// </returns>
        public Task<(TRequest, Uri)> ReturnsAsync<TRequest>(HttpStatusCode statusCode, object mockResponse)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
            var completionSource = new TaskCompletionSource<(TRequest, Uri)>();

            _target.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.Method.Equals(_method)),
                    ItExpr.IsAny<CancellationToken>())
                    .Callback<HttpRequestMessage, CancellationToken>(
                        (httpRequestMessage, cancellationToken) =>
                        {
                            var result = httpRequestMessage.Content
                                .ReadAsStringAsync()
                                .GetAwaiter()
                                .GetResult();

                            var requestBody = JsonSerializer.Deserialize<TRequest>(result);
                            var requestPath = httpRequestMessage.RequestUri;

                            completionSource.TrySetResult((requestBody, requestPath));
                        })
                    .ReturnsAsync(response);

            return completionSource.Task;
        }

        /// <summary>
        /// Mocks a Http response and allows the user to see the Url the request was sent to.
        /// </summary>
        /// <param name="statusCode">The desired response <see cref="HttpStatusCode"/></param>
        /// <param name="mockResponse">The desired response body</param>
        /// <returns><see cref="Task"/> of the <see cref="Uri"/>.</returns>
        public Task<Uri> ReturnsAsync(HttpStatusCode statusCode, object mockResponse)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(mockResponse))
            };
            var completionSource = new TaskCompletionSource<Uri>();

            _target.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.Method.Equals(_method)),
                    ItExpr.IsAny<CancellationToken>())
                    .Callback<HttpRequestMessage, CancellationToken>(
                        (httpRequestMessage, cancellationToken) =>
                        {
                            var uri = httpRequestMessage.RequestUri;
                            completionSource.TrySetResult(uri);
                        })
                .ReturnsAsync(response);

            return completionSource.Task;
        }

        /// <summary>
        /// Throws an exception
        /// </summary>
        public void ThrowsAsync(Exception ex)
        {

            _target.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.Method.Equals(_method)),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(ex);
        }

        /// <summary>
        /// Verifies the method has been called x number of times.
        /// </summary>
        public void VerifyIsCalled(int times)
        {

            _target.Protected()
                .Verify(
                "SendAsync",
                Times.Exactly(times),
                ItExpr.Is<HttpRequestMessage>(rm => rm.Method.Equals(_method)),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
