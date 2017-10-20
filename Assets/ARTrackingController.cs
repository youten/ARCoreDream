//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------
// modified https://blogs.unity3d.com/jp/2017/10/18/mobile-inside-out-vr-tracking-now-readily-available-on-your-phone-with-unity/

namespace GoogleARCore.HelloAR
{
    using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
    using UnityEngine.Rendering;
    using GoogleARCore;

    /// <summary>
    /// Controlls the HelloAR example.
    /// </summary>
    public class ARTrackingController : MonoBehaviour
    {
		public Text m_camPoseText;

		public GameObject m_CameraParent;

		public float m_XZScaleFactor = 10;

		public float m_YScaleFactor = 2;

		public bool m_showPoseData = true;

		private bool trackingStarted = false;

		private Vector3 m_prevARPosePosition;

		public void Update (){
			_QuitOnConnectionErrors();

			if (Frame.TrackingState != FrameTrackingState.Tracking) {
				trackingStarted = false;  // if tracking lost or not initialized
				m_camPoseText.text = "Lost tracking, wait ...";
				const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
				Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
				return;
			} else {
				m_camPoseText.text = "";
			}

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Vector3 currentARPosition = Frame.Pose.position;

			if (!trackingStarted) {
				trackingStarted = true;
				m_prevARPosePosition = Frame.Pose.position;
			}

			//Remember the previous position so we can apply deltas
			Vector3 deltaPosition = currentARPosition - m_prevARPosePosition;
			m_prevARPosePosition = currentARPosition;

			if (m_CameraParent != null) {
				Vector3 scaledTranslation = new Vector3 (m_XZScaleFactor * deltaPosition.x, m_YScaleFactor * deltaPosition.y, m_XZScaleFactor * deltaPosition.z);
				m_CameraParent.transform.Translate (scaledTranslation);

				if (m_showPoseData) {
					//m_camPoseText.text = "Pose = " + currentARPosition + "\n" + GetComponent<FPSARCoreScript> ().FPSstring + "\n" + m_CameraParent.transform.position;
					m_camPoseText.text = "Pose  :" + currentARPosition + "\n" + "Scaled:" + m_CameraParent.transform.position;
				}
			}
		}

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            // Do not update if ARCore is not tracking.
            if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
            {
				m_camPoseText.text = "This device does not support ARCore.";
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
            {
				m_camPoseText.text = "Camera permission is needed to run this application.";
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
            {
				m_camPoseText.text = "ARCore encountered a problem connecting. Please start the app again.";
                Application.Quit();
            }
        }
    }
}
