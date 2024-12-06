using UnityEngine;

public class Click_OnOff : MonoBehaviour
{
    // Start is called before the first frame update


    Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))

            anim.SetTrigger("Trigger");
    }
}
