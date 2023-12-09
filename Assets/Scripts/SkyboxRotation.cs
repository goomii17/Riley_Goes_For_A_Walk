using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
