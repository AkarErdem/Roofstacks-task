using System.Collections;
using GameCode.Data;
using GameCode.Init;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace GameCode.SceneHandler
{
    public static class SceneLoader
    {
        public static AsyncOperationHandle<SceneInstance> LoadingSceneHandle { get; private set; }
        
        public static IReactiveProperty<bool> IsLoading { get; } = new ReactiveProperty<bool>(false);
        
        public static void LoadScene(string sceneName, SceneLoadType sceneLoadType)
        {
            MainThreadDispatcher.StartCoroutine(LoadSceneAsync(sceneName, sceneLoadType));
        }

        /// <summary>
        /// The Application loads the Scene in the background as the current Scene runs.
        /// This is particularly good for creating loading screens.
        /// </summary>
        private static IEnumerator LoadSceneAsync(string sceneName, SceneLoadType sceneLoadType)
        {
            if (IsLoading.Value)
            {
                yield break;
            }

            IsLoading.Value = true;
            
            if (LoadingSceneHandle.IsValid())
                Addressables.UnloadSceneAsync(LoadingSceneHandle);
            
            // LoadingSceneHandle = Addressables.LoadSceneAsync(GameManager.Instance.GameConfig.InitializeSceneName, LoadSceneMode.Additive);

            var time = Time.time;
            var loadingTime = GameManager.Instance.GameConfig.SceneLoadCountdown;
            
            if (sceneLoadType == SceneLoadType.Default)
            {
                var asyncLoad = SceneManager.LoadSceneAsync(sceneName);

                //Don't let the Scene activate until you allow it to
                asyncLoad.allowSceneActivation = false;
                
                // Wait until the timer ends
                while (Time.time - time < loadingTime)
                {
                    yield return null;
                }

                // Wait until the asynchronous scene fully loads
                asyncLoad.allowSceneActivation = true;
                while (!asyncLoad.isDone || !LoadingSceneHandle.IsDone)
                {
                    yield return null;
                }
            }
            else
            {
                PhotonNetwork.LoadLevel(sceneName);
                
                // Wait until the timer ends
                while (Time.time - time < loadingTime)
                {
                    yield return null;
                }

                // Wait until the asynchronous scene fully loads
                while (!LoadingSceneHandle.IsDone)
                {
                    yield return null;
                }
            }
            
            
            IsLoading.Value = false;
        }
    }
}
