using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vuforia;

namespace Tests.PlayTests
{
    public class CameraTests
    {

        private GameObject _camera;
        
        [SetUp]
        public void SetUp()
        {
            _camera = new GameObject();
            _camera.AddComponent<Camera>();
            _camera.AddComponent<VuforiaBehaviour>();
            _camera.AddComponent<DefaultInitializationErrorHandler>();
            _camera.AddComponent<CameraFocusController>();
        }
        

//        [UnityTest]
//        public IEnumerator CameraFocusControllerSetsCameraFocusModeToContinuousAuto_Test()
//        {
//            Assert.AreEqual(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO, CameraFocusController.CurrentFocusMode);
//            yield return null;
//        }
    }
}
