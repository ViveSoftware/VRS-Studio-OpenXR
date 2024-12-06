using UnityEngine;

public class GetPosition : MonoBehaviour
{
    // Start is called before the first frame update

    public Material BoundaryMaterial;
    public Transform Hand_position;

    Vector3 targetObjectPosition;


    void Start()
    {
        BoundaryMaterial = GetComponent<Renderer>().material;


    }

    // Update is called once per frame
    void Update()
    {

        BoundaryMaterial.SetVector("_Hand_pos", Hand_position.position);

    }
}
