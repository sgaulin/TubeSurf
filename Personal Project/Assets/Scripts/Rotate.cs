using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] Vector3 rotationForce;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationForce);
    }
}
