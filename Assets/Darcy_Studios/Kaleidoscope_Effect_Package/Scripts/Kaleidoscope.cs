using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaleidoscope : MonoBehaviour
{
	[SerializeField] private bool flipHorizontal = false;
	[SerializeField] private bool flipVertical = false;

	private Camera cameraFlip;
	private Matrix4x4 originalProjectionMatrix;

	private void Start()
	{
		// Get camera on game object
		cameraFlip = GetComponent<Camera>();

		originalProjectionMatrix = cameraFlip.projectionMatrix;
	}

	// Update is called once per frame
	void Update()
    {
	    if (flipHorizontal && !flipVertical)
	    {
			FlipCamera(-1, 1, 1);
		}
		else if (!flipHorizontal && flipVertical)
	    {
			FlipCamera(1, -1, 1); ;
	    }
		else if (flipHorizontal && flipVertical)
	    {
			FlipCamera(-1, -1, 1);
		}
	    else
	    {
		    FlipCamera(1, 1, 1);
		}
	}

	private void FlipCamera(float x, float y, float z)
	{
		cameraFlip.projectionMatrix = originalProjectionMatrix * Matrix4x4.Scale(new Vector3(x, y, z));
	}
}
