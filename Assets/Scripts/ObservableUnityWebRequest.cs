using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using ExtraUnityEngine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace UniRx
{
    [PublicAPI]
    public static class ObservableUnityWebRequest
    {
        private const string RequestMethodGet = "GET";
        private const string RequestMethodPost = "POST";
        private const string RequestMethodPut = "PUT";
        private const string RequestMethodHead = "HEAD";
        private const string RequestMethodDelete = "DELETE";

        public static class RequestHeader
        {
            public static class ContentType
            {
                public static class Application
                {
                    public const string JSON = "application/json";
                    public const string XWwwFormUrlencoded = "application/x-www-form-urlencoded";
                }
            }
        }

        internal static Func<UnityWebRequest, string> FetchString { get; } = uwr => Encoding.UTF8.GetString(uwr.downloadHandler.data);
        internal static Func<UnityWebRequest, IEnumerable<byte>> FetchBytes { get; } = uwr => uwr.downloadHandler.data;
        internal static Func<UnityWebRequest, IDictionary<string, string>> FetchResponseHeaders { get; } = uwr => uwr.GetResponseHeaders();
        internal static Func<UnityWebRequest, Texture2D> FetchTexture2D { get; } = uwr => (uwr.downloadHandler as DownloadHandlerTexture)?.texture;
        internal static Func<UnityWebRequest, AudioClip> FetchAudioClip { get; } = uwr => (uwr.downloadHandler as DownloadHandlerAudioClip)?.audioClip;
        internal static Func<UnityWebRequest, AssetBundle> FetchAssetBundle { get; } = uwr => (uwr.downloadHandler as DownloadHandlerAssetBundle)?.assetBundle;

        private static IDictionary<HttpStatusCode, Func<UnityWebRequest, UnityWebRequestErrorException>> ExceptionFactoryMap { get; } = new Dictionary<HttpStatusCode, Func<UnityWebRequest, UnityWebRequestErrorException>>
        {
            {HttpStatusCode.BadRequest, uwr => new UnityWebRequestErrorException.BadRequest(uwr)},
            {HttpStatusCode.Unauthorized, uwr => new UnityWebRequestErrorException.Unauthorized(uwr)},
            {HttpStatusCode.Forbidden, uwr => new UnityWebRequestErrorException.Forbidden(uwr)},
            {HttpStatusCode.NotFound, uwr => new UnityWebRequestErrorException.NotFound(uwr)},
            {HttpStatusCode.InternalServerError, uwr => new UnityWebRequestErrorException.InternalServerError(uwr)},
            {HttpStatusCode.BadGateway, uwr => new UnityWebRequestErrorException.BadGateway(uwr)},
            {HttpStatusCode.ServiceUnavailable, uwr => new UnityWebRequestErrorException.ServiceUnavailable(uwr)},
        };

        #region Basic HTTP Methods

        #region GET

        public static IObservable<string> GetAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> GetAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<IEnumerable<byte>> GetBytesAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchBytes, progress);
        }

        public static IObservable<IEnumerable<byte>> GetBytesAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchBytes, progress);
        }

        #endregion

        #region POST

        public static IObservable<string> PostAsObservable(string url, string requestBody, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PostAsObservable(string url, byte[] requestBody, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PostAsObservable(string url, IEnumerable<byte> requestBody, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PostAsObservable(Uri uri, string requestBody, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PostAsObservable(Uri uri, byte[] requestBody, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PostAsObservable(Uri uri, IEnumerable<byte> requestBody, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        #endregion

        #region PUT

        public static IObservable<string> PutAsObservable(string url, byte[] rawData, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData)).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PutAsObservable(string url, IEnumerable<byte> rawData, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData.ToArray())).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PutAsObservable(string url, FileInfo fileInfo, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerFile(fileInfo.FullName)).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PutAsObservable(Uri uri, IEnumerable<byte> rawData, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData.ToArray())).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PutAsObservable(Uri uri, byte[] rawData, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData)).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> PutAsObservable(Uri uri, FileInfo fileInfo, string contentType = RequestHeader.ContentType.Application.XWwwFormUrlencoded, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerFile(fileInfo.FullName)).ApplyRequestHeader("Content-Type", contentType).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        #endregion

        #region HEAD

        public static IObservable<IDictionary<string, string>> HeadAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodHead).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchResponseHeaders, progress);
        }

        public static IObservable<IDictionary<string, string>> HeadAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodHead).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchResponseHeaders, progress);
        }

        #endregion

        #region DELETE

        public static IObservable<string> DeleteAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodDelete).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        public static IObservable<string> DeleteAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodDelete).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), FetchString, progress);
        }

        #endregion

        #endregion

        #region Fetch UnityEngine.Object

        #region Texture2D

        public static IObservable<Texture2D> GetTexture2DAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(url).ApplyRequestHeaders(requestHeaders), FetchTexture2D, progress);
        }

        public static IObservable<Texture2D> GetTexture2DAsObservable(string url, bool nonReadable, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(url, nonReadable).ApplyRequestHeaders(requestHeaders), FetchTexture2D, progress);
        }

        public static IObservable<Texture2D> GetTexture2DAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(uri).ApplyRequestHeaders(requestHeaders), FetchTexture2D, progress);
        }

        public static IObservable<Texture2D> GetTexture2DAsObservable(Uri uri, bool nonReadable, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(uri, nonReadable).ApplyRequestHeaders(requestHeaders), FetchTexture2D, progress);
        }

        #endregion

        #region AudioClip

        public static IObservable<AudioClip> GetAudioClipAsObservable(string url, AudioType audioType, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestMultimedia.GetAudioClip(url, audioType).ApplyRequestHeaders(requestHeaders), FetchAudioClip, progress);
        }

        public static IObservable<AudioClip> GetAudioClipAsObservable(Uri uri, AudioType audioType, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestMultimedia.GetAudioClip(uri, audioType).ApplyRequestHeaders(requestHeaders), FetchAudioClip, progress);
        }

        #endregion

        #region AssetBundle

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, CachedAssetBundle cachedAssetBundle, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, cachedAssetBundle, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, Hash128 hash, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, uint version, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, version, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, CachedAssetBundle cachedAssetBundle, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, cachedAssetBundle, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, Hash128 hash, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, hash, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, uint version, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, version, crc).ApplyRequestHeaders(requestHeaders), FetchAssetBundle, progress);
        }

        #endregion

        #endregion

        internal static IObservable<T> RequestAsObservable<T>(UnityWebRequest uwr, Func<UnityWebRequest, T> fetchCallback, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => uwr, fetchCallback, progress);
        }

        internal static IObservable<T> RequestAsObservable<T>(Func<UnityWebRequest> factory, Func<UnityWebRequest, T> fetchCallback, IProgress<float> progress = default)
        {
            return Observable.FromCoroutine<T>((observer, cancellationToken) => Fetch(factory(), observer, fetchCallback, progress, cancellationToken));
        }

        private static IEnumerator Fetch<T>(UnityWebRequest uwr, IObserver<T> observer, Func<UnityWebRequest, T> fetchCallback, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var operation = uwr.SendWebRequest();
            do
            {
                try
                {
                    progress?.Report(operation.progress);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                    yield break;
                }

                yield return null;
            } while (!operation.isDone && !cancellationToken.IsCancellationRequested);

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            if (!string.IsNullOrEmpty(uwr.error))
            {
                observer
                    .OnError(
                        ExceptionFactoryMap.ContainsKey((HttpStatusCode) uwr.responseCode)
                            ? ExceptionFactoryMap[(HttpStatusCode) uwr.responseCode](uwr)
                            : new UnityWebRequestErrorException(uwr)
                    );
            }
            else if (uwr.downloadHandler != default)
            {
                observer.OnNext(downloadCallback(uwr.downloadHandler));
                observer.OnCompleted();
            }
        }
    }
}