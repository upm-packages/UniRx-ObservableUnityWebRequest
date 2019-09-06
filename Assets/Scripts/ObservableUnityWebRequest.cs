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

        private static Func<DownloadHandler, string> DownloadCallbackString { get; } = downloadHandler => Encoding.UTF8.GetString(downloadHandler.data);
        private static Func<DownloadHandler, IEnumerable<byte>> DownloadCallbackBytes { get; } = downloadHandler => downloadHandler.data;
        private static Func<DownloadHandler, Texture2D> DownloadCallbackTexture { get; } = downloadHandler => (downloadHandler as DownloadHandlerTexture)?.texture;
        private static Func<DownloadHandler, AudioClip> DownloadCallbackAudioClip { get; } = downloadHandler => (downloadHandler as DownloadHandlerAudioClip)?.audioClip;
        private static Func<DownloadHandler, AssetBundle> DownloadCallbackAssetBundle { get; } = downloadHandler => (downloadHandler as DownloadHandlerAssetBundle)?.assetBundle;

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
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> GetAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<IEnumerable<byte>> GetBytesAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackBytes, progress);
        }

        public static IObservable<IEnumerable<byte>> GetBytesAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodGet).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackBytes, progress);
        }

        #endregion

        #region POST

        public static IObservable<string> PostAsObservable(string url, string requestBody, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PostAsObservable(string url, byte[] requestBody, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PostAsObservable(string url, IEnumerable<byte> requestBody, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PostAsObservable(Uri uri, string requestBody, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PostAsObservable(Uri uri, byte[] requestBody, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PostAsObservable(Uri uri, IEnumerable<byte> requestBody, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPost).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestBody(requestBody).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        #endregion

        #region PUT

        public static IObservable<string> PutAsObservable(string url, byte[] rawData, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData)).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PutAsObservable(string url, IEnumerable<byte> rawData, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData.ToArray())).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PutAsObservable(string url, FileInfo fileInfo, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerFile(fileInfo.FullName)).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PutAsObservable(Uri uri, IEnumerable<byte> rawData, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData.ToArray())).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PutAsObservable(Uri uri, byte[] rawData, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerRaw(rawData)).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> PutAsObservable(Uri uri, FileInfo fileInfo, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodPut).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyUploadHandler(new UploadHandlerFile(fileInfo.FullName)).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        #endregion

        #region HEAD

        public static IObservable<string> HeadAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodHead).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> HeadAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodHead).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        #endregion

        #region DELETE

        public static IObservable<string> DeleteAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(url, RequestMethodDelete).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        public static IObservable<string> DeleteAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => new UnityWebRequest(uri, RequestMethodDelete).ApplyDownloadHandler(new DownloadHandlerBuffer()).ApplyRequestHeaders(requestHeaders), DownloadCallbackString, progress);
        }

        #endregion

        #endregion

        #region Fetch UnityEngine.Object

        #region Texture2D

        public static IObservable<Texture2D> GetTextureAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(url).ApplyRequestHeaders(requestHeaders), DownloadCallbackTexture, progress);
        }

        public static IObservable<Texture2D> GetTextureAsObservable(string url, bool nonReadable, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(url, nonReadable).ApplyRequestHeaders(requestHeaders), DownloadCallbackTexture, progress);
        }

        public static IObservable<Texture2D> GetTextureAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(uri).ApplyRequestHeaders(requestHeaders), DownloadCallbackTexture, progress);
        }

        public static IObservable<Texture2D> GetTextureAsObservable(Uri uri, bool nonReadable, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestTexture.GetTexture(uri, nonReadable).ApplyRequestHeaders(requestHeaders), DownloadCallbackTexture, progress);
        }

        #endregion

        #region AudioClip

        public static IObservable<AudioClip> GetAudioClipAsObservable(string url, AudioType audioType, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestMultimedia.GetAudioClip(url, audioType).ApplyRequestHeaders(requestHeaders), DownloadCallbackAudioClip, progress);
        }

        public static IObservable<AudioClip> GetAudioClipAsObservable(Uri uri, AudioType audioType, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestMultimedia.GetAudioClip(uri, audioType).ApplyRequestHeaders(requestHeaders), DownloadCallbackAudioClip, progress);
        }

        #endregion

        #region AssetBundle

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, CachedAssetBundle cachedAssetBundle, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, cachedAssetBundle, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, Hash128 hash, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(string url, uint version, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(url, version, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, CachedAssetBundle cachedAssetBundle, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, cachedAssetBundle, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, Hash128 hash, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, hash, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        public static IObservable<AssetBundle> GetAssetBundleAsObservable(Uri uri, uint version, uint crc = 0U, IDictionary<string, string> requestHeaders = default, IProgress<float> progress = default)
        {
            return RequestAsObservable(() => UnityWebRequestAssetBundle.GetAssetBundle(uri, version, crc).ApplyRequestHeaders(requestHeaders), DownloadCallbackAssetBundle, progress);
        }

        #endregion

        #endregion

        private static IObservable<T> RequestAsObservable<T>(Func<UnityWebRequest> factory, Func<DownloadHandler, T> downloadCallback, IProgress<float> progress = default)
        {
            return Observable
                .Create<UnityWebRequest>(
                    observer =>
                    {
                        observer.OnNext(factory());

                        return Disposable.Empty;
                    }
                )
                .SelectMany(x => Observable.FromCoroutine<T>((observer, cancellationToken) => Fetch(x, observer, downloadCallback, progress, cancellationToken)));
        }

        private static IEnumerator Fetch<T>(UnityWebRequest uwr, IObserver<T> observer, Func<DownloadHandler, T> downloadCallback, IProgress<float> progress, CancellationToken cancellationToken)
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
            }
        }
    }
}