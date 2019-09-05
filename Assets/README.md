# UniRx ObservableUnityWebRequest

Provides extension methods to fetch as `IObservable` using UnityWebRequest

## Installation

```bash
upm add package dev.upm-packages.unirx-observableunitywebrequest
```

## Usages

### `UnityWebRequest.SendWebRequestAsObservable()`

Send request to server to fetch response.

#### Example

```csharp
using UniRx;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        new UnityWebRequest("https://www.google.com/")
            .SendWebRequestAsObservable()
            .Subscribe(responseBody => Debug.Log(responseBody));
    }
}
```

### `UnityWebRequest.SendWebRequestAsObservable<T>()`

Send request to server to fetch Unity Objects such as `Texture`, `AudioClip` and `AssetBundle`.

To fetch an instance inheriting UnityEngine.Object, it is necessary to use the dedicated UnityWebRequest class shown in the table below.

| Type | Class |
| --- | --- |
| Texture | UnityWebRequestTexture |
| AudioClip | UnityWebRequestMultimedia |
| AssetBundle | UnityWebRequestAssetBundle |

#### Example

```csharp
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        UnityWebRequestAssetBundle.GetAssetBundle("https://example.com/path/to/assetbundle")
            .SendWebRequestAsObservable<AssetBundle>()
            .Subscribe(assetBundle => /* Do something */);
    }
}
```
