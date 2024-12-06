using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionPanelController : MonoBehaviour
{
    public GameObject panel;

    void Update()
    {
        var scene = SceneManager.GetActiveScene();

        if (scene.name == "v130_demo") panel.SetActive(false);
        else if (scene.name == "VRSS_Environment") panel.SetActive(true);
    }
}
