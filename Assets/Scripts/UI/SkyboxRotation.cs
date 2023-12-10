using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
	const float rotationSpeed = 0.15f;

	void Update()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
	}
}
