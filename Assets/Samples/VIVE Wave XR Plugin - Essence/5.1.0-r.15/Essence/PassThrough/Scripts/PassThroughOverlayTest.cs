// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using Wave.Native;

namespace Wave.Essence.Samples.PassThrough
{
	public class PassThroughOverlayTest : MonoBehaviour
	{
		private static string LOG_TAG = "Wave.Essence.Samples.PassThrough.PassThroughOverlayTest";

		private bool passThroughOverlayFlag = false;
		private bool showPassThroughOverlay = false;
		bool delaySubmit = false;
		bool showIndicator = false;
		float alpha = 1.0f;
		float alpha2 = 1.0f;
		int steps = 0;
		// Start is called before the first frame update

		public bool isInLobby;
		public GameObject XROrigin;
		void Start()
		{
			Interop.WVR_ShowPassthroughUnderlay(false);
			isInLobby = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (isInLobby && WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
			{
				isInLobby = false;
				XROrigin.transform.position += new Vector3(0,0,40f);
				Interop.WVR_ShowPassthroughUnderlay(true);
			}
			if (!isInLobby) {
					XROrigin.transform.position += new Vector3(0,0,0.09f);
			}
		}
	}


}
