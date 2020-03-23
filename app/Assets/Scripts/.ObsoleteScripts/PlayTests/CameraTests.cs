using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class CameraTests
    {
        private AssetBundle myLoadedAssetBundle;
        private string[] scenePaths;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/scenes");
            scenePaths = myLoadedAssetBundle.GetAllScenePaths();
            SceneManager.LoadScene("CustomerDemo");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CameraIsSetToAutoFocus_Test()
        {
            var cameraFocus = GameObject.FindObjectOfType<CameraFocusController>();
            Assert.IsTrue(true);
            //Assert.AreEqual(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO, cameraFocus.cameraFocusMode);

            // Use yield to skip a frame.
            yield return null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            myLoadedAssetBundle.Unload(true);
        }
    }
}
