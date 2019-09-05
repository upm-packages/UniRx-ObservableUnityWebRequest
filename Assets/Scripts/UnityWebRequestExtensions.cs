using System;
using System.Collections;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace UniRx
{
    [PublicAPI]
    public static class UnityWebRequestExtensions
    {
        public static IObservable<string> SendWebRequestAsObservable(this UnityWebRequest self, IProgress<float> progress = default)
        {
            return Observable
                .FromCoroutine<string>((observer, cancellationToken) => Fetch(self, progress, observer, cancellationToken));
        }

        public static IObservable<T> SendWebRequestAsObservable<T>(this UnityWebRequest self, IProgress<float> progress = default) where T : Object
        {
            return Observable
                .FromCoroutine<T>((observer, cancellationToken) => Fetch(self, progress, observer, cancellationToken));
        }

        private static IEnumerator Fetch<T>(UnityWebRequest uwr, IProgress<float> progress, IObserver<T> observer, CancellationToken cancellationToken)
        {
            var operation = uwr.SendWebRequest();
            while (!uwr.isDone && !cancellationToken.IsCancellationRequested)
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
            }

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            try
            {
                progress?.Report(uwr.downloadProgress);
            }
            catch (Exception e)
            {
                observer.OnError(e);
                yield break;
            }

            if (!string.IsNullOrEmpty(uwr.error))
            {
                observer.OnError(new UnityWebRequestErrorException(uwr));
            }
            else
            {
                switch (uwr.downloadHandler)
                {
                    case DownloadHandlerBuffer downloadHandlerBuffer:
                        if (typeof(T) == typeof(string))
                        {
                            observer.OnNext((T) (object) Encoding.UTF8.GetString(downloadHandlerBuffer.data));
                        }
                        else if (typeof(T) == typeof(byte[]))
                        {
                            observer.OnNext((T) (object) downloadHandlerBuffer.data);
                        }
                        break;
                    case DownloadHandlerTexture downloadHandlerTexture when downloadHandlerTexture.texture is T texture:
                        observer.OnNext(texture);
                        break;
                    case DownloadHandlerAudioClip downloadHandlerAudioClip when downloadHandlerAudioClip.audioClip is T audioClip:
                        observer.OnNext(audioClip);
                        break;
                    case DownloadHandlerAssetBundle downloadHandlerAssetBundle when downloadHandlerAssetBundle.assetBundle is T assetBundle:
                        observer.OnNext(assetBundle);
                        break;
                    default:
                        var text = Encoding.UTF8.GetString(uwr.downloadHandler.data);
                        if (text is T value)
                        {
                            observer.OnNext(value);
                        }

                        break;
                }
            }
        }
    }
}