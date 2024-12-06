using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRSStudioSceneManager : MonoBehaviour
{
    private string[] Scenes;
    private Scene LastScene;
    private string NextSceneName;

    private SceneList sceneList = null;
    public SceneList SceneList => sceneList;

    private string m_SceneIntentString = "";

    private bool isSceneUnloading = false;

    private void Awake()
    {
        sceneList = Resources.Load<SceneList>(SceneList.AssetPathShort);
        if (sceneList == null)
        {
            Debug.LogError("[VRSS][VRSStudioSceneManager][Awake] sceneList is not found!");
            sceneList = new SceneList();
            sceneList.Scenes = new VRSStudioScenes();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        //Load environment.
        SceneManager.LoadSceneAsync(SceneList.Scenes.TrackerSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.EnvironmentSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.SpectatorSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.AvatarSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.BottleSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.RobotAssistantSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.KeyboardSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.Object3DSceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneList.Scenes.FacialTrackingSceneName, LoadSceneMode.Additive);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    public void GotoFirstScene()
    {
        NextSceneName = Scenes[0];
        Debug.Log("[VRSS][VRSStudioSceneManager][GotoFirstScene] Go to " + NextSceneName + " from " + SceneManager.GetActiveScene().name);
        StartLoadNextScene();
    }

    public void GotoNextScene()
    {
        NextSceneName = GetNextSceneName();
        Debug.Log("[VRSS][VRSStudioSceneManager][GotoNextScene] Go to " + NextSceneName + " from " + SceneManager.GetActiveScene().name);
        StartLoadNextScene();
    }

    private void HandleSceneIntentInvoke()
    {
        if (!string.IsNullOrEmpty(m_SceneIntentString))
        {
            HandleSceneIntent(m_SceneIntentString, false);
            m_SceneIntentString = "";
        }
    }

    public void HandleSceneIntent(string JSSceneIntentString, bool isIntent)
    {
        // TODO
    }

    public void UnloadActiveScene()
    {
        string ActiveSceneName = SceneManager.GetActiveScene().name;
        if (ActiveSceneName != SceneList.Scenes.MainSceneName)
        {
            SceneManager.UnloadSceneAsync(ActiveSceneName);
        }
    }

    public void UnloadScene(string SceneName)
    {
        if ((!string.IsNullOrEmpty(SceneName)) && (SceneName != SceneList.Scenes.MainSceneName))
        {
            isSceneUnloading = true;
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }

    private string GetNextSceneName()
    {
        int LastSceneIndex = System.Array.FindIndex<string>(Scenes, ele => ele.Equals(LastScene.name));
        string CandidateScene = sceneList.Scenes.EnvironmentSceneName;

        if (NextSceneName != string.Empty)
        {
            CandidateScene = NextSceneName;
            NextSceneName = string.Empty;
        }
        else if ((LastSceneIndex >= 0) && (LastSceneIndex < Scenes.Length - 1))
        {
            for (int i = 1; LastSceneIndex + i < Scenes.Length; i++)
            {
                CandidateScene = Scenes[LastSceneIndex + i];
                if (!string.IsNullOrEmpty(CandidateScene))
                {
                    break;
                }
            }
        }

        //Check if scene exists.
        if (CandidateScene == SceneManager.GetActiveScene().name)
        {
            Debug.LogWarning("[VRSS][VRSStudioSceneManager][GetNextSceneName] " + CandidateScene + " has been loaded!");
            return string.Empty;
        }

        return CandidateScene;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[VRSS][VRSStudioSceneManager][OnSceneLoaded] " + scene.name + " (" + mode + ")");
        LastScene = scene;
        NextSceneName = string.Empty;

        if (scene.name == SceneList.Scenes.EnvironmentSceneName)
        {
            SceneManager.SetActiveScene(scene);
        }

        if (fadeInCoroutine != null)
            StopCoroutine(fadeInCoroutine);

        fadeInCoroutine = StartCoroutine(FadeInLoadedScene());
    }

    private Coroutine fadeInCoroutine = null;
    private IEnumerator FadeInLoadedScene()
    {
        while (isSceneUnloading)
        {
            yield return null;
        }

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.EnterSceneTransition();
        }
        fadeInCoroutine = null;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("[VRSS][VRSStudioSceneManager][OnSceneUnloaded] " + scene.name + " is unloaded.");
        isSceneUnloading = false;
    }

    private void OnActiveSceneChanged(Scene scene1, Scene scene2)
    {
        //Note: It takes effect after Awake()/Enable(), before Start().
        Debug.Log("[VRSS][VRSStudioSceneManager][OnActiveSceneChanged] " + scene1.name + " -> " + scene2.name);

        UnloadScene(scene1.name);
    }

    private void StartLoadNextScene()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string nextSceneName = NextSceneName;

        yield return null;

        Debug.Log("[VRSS][VRSStudioSceneManager][LoadNextScene] Start to load " + NextSceneName);

        bool transitionIsDone = false;
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LeaveSceneTransition(() => { transitionIsDone = true; });
        }
        else
            transitionIsDone = true;

        float timer = 0f, timeLimit = 2f;
        while (!transitionIsDone)
        {
            timer += Time.deltaTime;
            yield return null;

            if (timer > timeLimit)
                break;
        }

        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(NextSceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
        Debug.Log("[VRSS][VRSStudioSceneManager][LoadNextScene] Scene has been loaded.");

        OnLoadSceneComplete(currentSceneName, nextSceneName);

        yield return null;
    }

    private void OnLoadSceneComplete(string fromScene, string toScene)
    {
        if (toScene == SceneList.Scenes.EnvironmentSceneName)
        {
            SceneManager.LoadSceneAsync(SceneList.Scenes.TrackerSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.SpectatorSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.AvatarSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.BottleSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.RobotAssistantSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.KeyboardSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.Object3DSceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(SceneList.Scenes.FacialTrackingSceneName, LoadSceneMode.Additive);
        }
    }

    static private VRSStudioSceneManager instance;
    public static VRSStudioSceneManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(VRSStudioSceneManager)) as VRSStudioSceneManager;

                if (!instance)
                {
                    Debug.LogError("There needs to be one active VRSStudioSceneManager script on a GameObject in your scene.");
                }
            }
            return instance;
        }
    }
}
