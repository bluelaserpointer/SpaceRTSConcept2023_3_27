using UnityEngine;

public class ConstantSpeed : MonoBehaviour
{
    public Vector3 move, rotation;

    // Update is called once per frame
    void Update()
    {
        transform.position += move * Time.deltaTime;
        transform.Rotate(rotation * Time.deltaTime);
    }
}
