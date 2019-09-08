using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace UniRx
{
    [PublicAPI]
    public static class UnityWebRequestExtensions
    {

        public static IObservable<string> SendWebRequestAsObservable(this UnityWebRequest self, IProgress<float> progress = default)
        {
            return ObservableUnityWebRequest.RequestAsObservable(self, ObservableUnityWebRequest.FetchString, progress);
        }

        private static IObservable<IEnumerable<byte>> SendBytesWebRequestAsObservable(this UnityWebRequest self, IProgress<float> progress = default)
        {
            return ObservableUnityWebRequest.RequestAsObservable(self, ObservableUnityWebRequest.FetchBytes, progress);
        }

        private static IObservable<Texture2D> SendTexture2DWebRequestAsObservable(this UnityWebRequest self, IProgress<float> progress = default)
        {
            return ObservableUnityWebRequest.RequestAsObservable(self, ObservableUnityWebRequest.FetchTexture2D, progress);
        }

        private static IObservable<AudioClip> SendAudioClipWebRequestAsObservable(this UnityWebRequest self, IProgress<float> progress = default)
        {
            return ObservableUnityWebRequest.RequestAsObservable(self, ObservableUnityWebRequest.FetchAudioClip, progress);
        }

        private static IObservable<AssetBundle> SendAssetBundleWebRequestAsObservable(this UnityWebRequest self, IProgress<float> progress = default)
        {
            return ObservableUnityWebRequest.RequestAsObservable(self, ObservableUnityWebRequest.FetchAssetBundle, progress);
        }
    }
}