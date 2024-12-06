using UnityEngine;


public class AvatarTransformUpdate : MonoBehaviour
{
    [SerializeField] public Transform[] EyesModels = new Transform[0];
    Vector3 rot = Camera.main.transform.rotation.eulerAngles;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update(6) obj pos: "+ this.transform.position.x+", "+ this.transform.position.y+","+ this.transform.position.z);
        //Debug.Log("Update(6) obj localpos: " + this.transform.localPosition.x + ", " + this.transform.localPosition.y + "，" + this.transform.localPosition.z);
        //Debug.Log("Update(6) obj pos: " + this.transform.rotation.x + ", " + this.transform.rotation.y + "," + this.transform.rotation.z+", "+ this.transform.rotation.w);
        //Debug.Log("Update(6) obj localpos: " + this.transform.localRotation.x + ", " + this.transform.localRotation.y + "，" + this.transform.localRotation.z+", "+ this.transform.localRotation.w);

        this.transform.position = new Vector3(Camera.main.transform.localPosition.x, 0.0f, Camera.main.transform.localPosition.z+0.9f);
        /*var*/ rot = Camera.main.transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(new Vector3(0, rot.y+180, 0));

    }

}
