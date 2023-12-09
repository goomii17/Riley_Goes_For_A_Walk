using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
	const float rotationSpeed = 0.1f;

	void Update()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
	}
}
