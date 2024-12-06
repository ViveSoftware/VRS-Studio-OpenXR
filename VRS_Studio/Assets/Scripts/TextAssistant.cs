using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextAssistant : MonoBehaviour
{
    public GameObject textRoot;
    public Text text;

    private static TextAssistant instance;
    public static TextAssistant Instance { get { return instance; } private set { instance = value; } }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        Scene soundScene = SceneManager.GetSceneByName("v130_demo");
        Scene currentScene = SceneManager.GetActiveScene();
        if (soundScene.IsValid() && soundScene == currentScene)
        {
            textRoot.SetActive(false);
        }
        //else if (scene.name == "VRSS_Environment") textRoot.SetActive(true);
    }

    public void SetText(string strText)
    {
        text.text = strText;
    }

    public void SetActive(bool active)
    {
        textRoot.SetActive(active);
    }
}
