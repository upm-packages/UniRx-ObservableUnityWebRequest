using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace UniRx
{
    [PublicAPI]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public partial class UnityWebRequestErrorException : Exception
    {
        public string RawErrorMessage { get; }
        public bool HasResponse { get; }
        public string Text { get; }
        public HttpStatusCode StatusCode { get; }
        public Dictionary<string, string> ResponseHeaders { get; }
        public UnityWebRequest Request { get; }

        public override string Message => ToString();

        // cache the text because if www was disposed, can't access it.
        public UnityWebRequestErrorException(UnityWebRequest request)
        {
            Request = request;
            RawErrorMessage = request.error;
            ResponseHeaders = request.GetResponseHeaders();
            HasResponse = false;
            StatusCode = (HttpStatusCode) request.responseCode;

            if (request.downloadHandler is DownloadHandlerBuffer)
            {
                Text = request.downloadHandler.text;
            }

            if (request.responseCode != 0)
            {
                HasResponse = true;
            }
        }

        public UnityWebRequestErrorException(UnityWebRequest request, HttpStatusCode statusCode) : this(request)
        {
            StatusCode = statusCode;
        }

        public override string ToString()
        {
            var text = Text;
            if (string.IsNullOrEmpty(text))
            {
                return RawErrorMessage;
            }

            return RawErrorMessage + " " + text;
        }

        public class BadRequest : UnityWebRequestErrorException
        {
            public BadRequest(UnityWebRequest request) : base(request, HttpStatusCode.BadRequest)
            {
            }
        }

        public class Unauthorized : UnityWebRequestErrorException
        {
            public Unauthorized(UnityWebRequest request) : base(request, HttpStatusCode.Unauthorized)
            {
            }
        }

        public class Forbidden : UnityWebRequestErrorException
        {
            public Forbidden(UnityWebRequest request) : base(request, HttpStatusCode.Forbidden)
            {
            }
        }

        public class NotFound : UnityWebRequestErrorException
        {
            public NotFound(UnityWebRequest request) : base(request, HttpStatusCode.NotFound)
            {
            }
        }

        public class InternalServerError : UnityWebRequestErrorException
        {
            public InternalServerError(UnityWebRequest request) : base(request, HttpStatusCode.InternalServerError)
            {
            }
        }

        public class BadGateway : UnityWebRequestErrorException
        {
            public BadGateway(UnityWebRequest request) : base(request, HttpStatusCode.BadGateway)
            {
            }
        }

        public class ServiceUnavailable : UnityWebRequestErrorException
        {
            public ServiceUnavailable(UnityWebRequest request) : base(request, HttpStatusCode.ServiceUnavailable)
            {
            }
        }
    }
}