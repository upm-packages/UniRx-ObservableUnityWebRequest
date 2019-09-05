using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace UniRx
{
    [PublicAPI]
    public class UnityWebRequestErrorException : Exception
    {
        public string RawErrorMessage { get; }
        public bool HasResponse { get; }
        public string Text { get; }
        public System.Net.HttpStatusCode StatusCode { get; }
        public Dictionary<string, string> ResponseHeaders { get; }
        public UnityWebRequest Request { get; }

        // cache the text because if www was disposed, can't access it.
        public UnityWebRequestErrorException(UnityWebRequest request)
        {
            Request = request;
            RawErrorMessage = request.error;
            ResponseHeaders = request.GetResponseHeaders();
            HasResponse = false;
            StatusCode = (System.Net.HttpStatusCode) request.responseCode;


            if (request.downloadHandler is DownloadHandlerBuffer)
            {
                Text = request.downloadHandler.text;
            }

            if (request.responseCode != 0)
            {
                HasResponse = true;
            }
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
    }
}