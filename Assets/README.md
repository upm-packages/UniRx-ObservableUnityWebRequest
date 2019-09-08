# UniRx ObservableUnityWebRequest

Provides extension methods to fetch as `IObservable` using UnityWebRequest

## Installation

```bash
upm add package dev.upm-packages.unirx-observableunitywebrequest
```

Please refer to [this repository](https://github.com/upm-packages/upm-cli) about the `upm` command.

## Usages

### Observable factories

#### `ObservableUnityWebRequest.[HTTPMethod]AsObservable()`

Create `IObservable<string>` instance that send request to server to fetch response as string.

The supported HTTP methods are: GET, POST, PUT, HEAD and DELETE

| HTTP Method | Method |
| --- | --- |
| GET | `ObservableUnityWebRequest.GetAsObservable()` |
| POST | `ObservableUnityWebRequest.PostAsObservable()` |
| PUT | `ObservableUnityWebRequest.PutAsObservable()` |
| HEAD | `ObservableUnityWebRequest.HeadAsObservable()` |
| DELETE | `ObservableUnityWebRequest.DeleteAsObservable()` |

##### Arguments

| Name | Type | Description |
| --- | --- | --- |
| `url` | `string` | The URL string of the image to download. |
| `uri` | `System.Net.Uri` | The URI of the image to download. |
| `requestHeaders` | `System.Collections.Generic.IDictionary<string, string>` | Name-value pair what to append to request headers |
| `progress` | `System.IProgress<float>` | Instance of `System.IProgress<float>` that to report progress of download and upload |

##### Example

```csharp
using UniRx;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        ObservableUnityWebRequest
            .GetAsObservable("https://www.google.com/")
            .Subscribe(responseBody => Debug.Log(responseBody));
    }
}
```

#### `ObservableUnityWebRequest.GetTexture2DAsObservable()`

Create `IObservable<Texture2D>` instance that send request to server to fetch response as `UnityEngine.Texture2D`.

##### Arguments

| Name | Type | Description |
| --- | --- | --- |
| `url` | `string` | The URL string of the image to download. |
| `uri` | `System.Net.Uri` | The URI of the image to download. |
| `nonReadable` | `bool` | If `true`, the texture's raw data will not be accessible to script. This can conserve memory. Default: `false`. |
| `requestHeaders` | `System.Collections.Generic.IDictionary<string, string>` | Name-value pair what to append to request headers |
| `progress` | `System.IProgress<float>` | Instance of `System.IProgress<float>` that to report progress of download and upload |

##### Example

```csharp
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField] private RawImage rawImage = default;

    private void Start()
    {
        ObservableUnityWebRequest
            .GetTexture2DAsObservable("https://www.example.com/path/to/texture2d.png")
            .Subscribe(texture2D => rawImage.texture = texture2D);
    }
}
```

#### `ObservableUnityWebRequest.GetAudioClipAsObservable()`

Create `IObservable<AudioClip>` instance that send request to server to fetch response as `UnityEngine.AudioClip`.

##### Arguments

| Name | Type | Description |
| --- | --- | --- |
| `url` | `string` | The URL string of the AudioClip to download. |
| `uri` | `System.Net.Uri` | The URI of the AudioClip to download. |
| `audioType` | `UnityEngine.AudioType` | The type of audio encoding for the downloaded audio clip. See `UnityEngine.AudioType`. |
| `requestHeaders` | `System.Collections.Generic.IDictionary<string, string>` | Name-value pair what to append to request headers |
| `progress` | `System.IProgress<float>` | Instance of `System.IProgress<float>` that to report progress of download and upload |

##### Example

```csharp
using UniRx;
using UnityEngine;

public class Sample : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = default;

    private void Start()
    {
        ObservableUnityWebRequest
            .GetAudioClipAsObservable("https://www.example.com/path/to/audioclip.mp3", AudioType.MPEG)
            .Subscribe(audioClip => audioSource.clip = audioClip);
    }
}
```

#### `ObservableUnityWebRequest.GetAssetBundleAsObservable()`

Create `IObservable<AssetBundle>` instance that send request to server to fetch response as `UnityEngine.AssetBundle`.

##### Arguments

| Name | Type | Description |
| --- | --- | --- |
| `url` | `string` | The URL string of the AssetBundle to download. |
| `uri` | `System.Net.Uri` | The URI of the AssetBundle to download. |
| `crc` | `uint` | If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped. |
| `version` | `uint` | An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle. Analogous to the version parameter for WWW.LoadFromCacheOrDownload. |
| `hash` | `UnityEngine.Hash128` | A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.. |
| `cachedAssetBundle` | `UnityEngine.CachedAssetBundle` | A structure used to download a given version of AssetBundle to a customized cache path. |
| `requestHeaders` | `System.Collections.Generic.IDictionary<string, string>` | Name-value pair what to append to request headers |
| `progress` | `System.IProgress<float>` | Instance of `System.IProgress<float>` that to report progress of download and upload |

##### Example

```csharp
using UniRx;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        ObservableUnityWebRequest
            .GetAssetBundleAsObservable("https://www.example.com/path/to/assetbundle")
            .Subscribe(assetBundle => /* Do something */);
    }
}
```

### Extension methods

#### `UnityWebRequest.SendWebRequestAsObservable()`

Send request to server to fetch response.

##### Example

```csharp
using UniRx;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        new UnityWebRequest("https://www.google.com/", "GET")
            .SendWebRequestAsObservable()
            .Subscribe(responseBody => Debug.Log(responseBody));
    }
}
```

#### `UnityWebRequest.SendXxxWebRequestAsObservable()`

Send request to server to fetch Unity Objects such as `Texture2D`, `AudioClip` and `AssetBundle`.

To fetch an instance inheriting UnityEngine.Object, it is necessary to use the dedicated UnityWebRequest class shown in the table below.

| Type | Class |
| --- | --- |
| Texture2D | UnityWebRequestTexture2D |
| AudioClip | UnityWebRequestMultimedia |
| AssetBundle | UnityWebRequestAssetBundle |

##### Example

```csharp
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        UnityWebRequestAssetBundle.GetAssetBundle("https://example.com/path/to/assetbundle")
            .SendAssetBundleWebRequestAsObservable()
            .Subscribe(assetBundle => /* Do something */);
    }
}
```
