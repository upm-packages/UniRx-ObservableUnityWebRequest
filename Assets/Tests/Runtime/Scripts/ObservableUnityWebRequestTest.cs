using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UniRx
{
    public class ObservableUnityWebRequestTest
    {
        [UnityTest]
        public IEnumerator GetAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .GetAsObservable($"file://{Application.dataPath}/Tests/Runtime/Fixtures/Text.txt")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            Assert.AreEqual("Text", yieldInstruction.Result);
        }

        [UnityTest]
        public IEnumerator GetTexture2DAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .GetTexture2DAsObservable($"file://{Application.dataPath}/Tests/Runtime/Fixtures/Texture2D.png")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            Assert.AreEqual(423, yieldInstruction.Result.width);
            Assert.AreEqual(500, yieldInstruction.Result.height);
        }

        [UnityTest]
        public IEnumerator GetAudioClipAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .GetAudioClipAsObservable($"file://{Application.dataPath}/Tests/Runtime/Fixtures/AudioClip.ogg", AudioType.OGGVORBIS)
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            Assert.AreEqual(8, (int) yieldInstruction.Result.length);
        }

        [UnityTest]
        public IEnumerator GetAssetBundleAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .GetAssetBundleAsObservable($"file://{Application.dataPath}/Tests/Runtime/Fixtures/AssetBundle.assetbundle", 0U, 0U)
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            var assetBundle = yieldInstruction.Result;
            var assetBundleRequest = assetBundle.LoadAssetAsync<TextAsset>(assetBundle.GetAllAssetNames().First());
            yield return assetBundleRequest;
            Assert.AreEqual(typeof(TextAsset), assetBundleRequest.asset.GetType());
            Assert.AreEqual("Sample\n", (assetBundleRequest.asset as TextAsset)?.text);
        }

        [UnityTest]
        public IEnumerator HttpGetAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .GetAsObservable("http://localhost:3001/fruits")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            var fruits = JsonUtility.FromJson<Fruits>($"{{\"fruits\":{yieldInstruction.Result}}}");
            Assert.AreEqual(1, fruits.FruitList[0].Id);
            Assert.AreEqual("Apple", fruits.FruitList[0].Name);
            Assert.AreEqual("りんご", fruits.FruitList[0].NameJapanese);
        }

        [UnityTest]
        public IEnumerator HttpPostAsObservable()
        {
            yield return PrepareForPost();

            var yieldInstruction = ObservableUnityWebRequest
                .PostAsObservable("http://localhost:3001/fruits", "id=4&name=Banana&name_ja=ばなな")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            var fruit = JsonUtility.FromJson<Fruit>(yieldInstruction.Result);
            Assert.AreEqual(4, fruit.Id);
            Assert.AreEqual("Banana", fruit.Name);
            Assert.AreEqual("ばなな", fruit.NameJapanese);
        }

        [UnityTest]
        public IEnumerator HttpPutAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .PutAsObservable("http://localhost:3001/fruits/2", Encoding.UTF8.GetBytes("id=2&name=BloodOrange&name_ja=ぶらっどおれんじ"))
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            var fruit = JsonUtility.FromJson<Fruit>(yieldInstruction.Result);
            Assert.AreEqual(2, fruit.Id);
            Assert.AreEqual("BloodOrange", fruit.Name);
            Assert.AreEqual("ぶらっどおれんじ", fruit.NameJapanese);
        }

        [UnityTest]
        public IEnumerator HttpDeleteAsObservable()
        {
            yield return PrepareForDelete();

            var yieldInstruction = ObservableUnityWebRequest
                .DeleteAsObservable("http://localhost:3001/fruits/3")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            var fruit = JsonUtility.FromJson<Fruit>(yieldInstruction.Result);
            Assert.AreEqual(default(int), fruit.Id);
            Assert.Null(fruit.Name);
            Assert.Null(fruit.NameJapanese);
        }

        [UnityTest]
        public IEnumerator HttpHeadAsObservable()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .HeadAsObservable("http://localhost:3001/fruits")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            var responseHeaders = yieldInstruction.Result;
            Assert.AreEqual("application/json; charset=utf-8", responseHeaders["Content-Type"]);
            Assert.AreEqual("Express", responseHeaders["X-Powered-By"]);
        }

        [UnityTest]
        public IEnumerator Progress()
        {
            var progress = new ScheduledNotifier<float>();
            var hasReported = false;
            var latestProgress = 0.0f;
            var reportCount = 0;
            progress
                .Subscribe(
                    x =>
                    {
                        hasReported = true;
                        latestProgress = x;
                        reportCount++;
                    }
                );
            var yieldInstruction = ObservableUnityWebRequest
                .GetAsObservable("http://localhost:3001/fruits", null, progress)
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            Assert.True(hasReported);
            Assert.AreEqual(1.0f, latestProgress);
            Assert.GreaterOrEqual(reportCount, 1);
        }

        /// <summary>
        /// Remove record if exists for idempotency
        /// </summary>
        /// <returns></returns>
        private static IEnumerator PrepareForPost()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .HeadAsObservable("http://localhost:3001/fruits/4")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            if (!yieldInstruction.HasError)
            {
                yield return ObservableUnityWebRequest
                    .DeleteAsObservable("http://localhost:3001/fruits/4")
                    .ToYieldInstruction(false);
            }
        }

        /// <summary>
        /// Put record if not exists for idempotency
        /// </summary>
        /// <returns></returns>
        private static IEnumerator PrepareForDelete()
        {
            var yieldInstruction = ObservableUnityWebRequest
                .HeadAsObservable("http://localhost:3001/fruits/3")
                .ToYieldInstruction(false);
            yield return yieldInstruction;
            if (yieldInstruction.HasError && yieldInstruction.Error is UnityWebRequestErrorException.NotFound)
            {
                yield return ObservableUnityWebRequest
                    .PostAsObservable("http://localhost:3001/fruits", Encoding.UTF8.GetBytes("id=3&name=Grape&name_ja=ぶどう"))
                    .ToYieldInstruction(false);
            }
        }

        [Serializable]
        private class Fruits
        {
            [SerializeField] private List<Fruit> fruits = default;
            public IList<Fruit> FruitList => fruits;
        }

        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class Fruit
        {
            [SerializeField] private int id = default;
            [SerializeField] private string name = default;
            [SerializeField] private string name_ja = default;

            public int Id => id;
            public string Name => name;
            public string NameJapanese => name_ja;
        }
    }
}