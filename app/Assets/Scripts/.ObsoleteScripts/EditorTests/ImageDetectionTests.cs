using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

namespace Tests 
{
    public class ImageDetectionTests {

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CustomerDemo.unity");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator UIPanelIsEnabledWhenAnObjectIsTracked_Test()
        {
            // Get UI Canvas
            var canvas = GameObject.FindObjectOfType<Canvas>();
            Transform[] canvasPanels = canvas.GetComponentsInChildren<Transform>(true);

            // Find which panel corresponds to Restaurant Info
            GameObject panel = null;
            foreach (Transform transform in canvasPanels)
            {
                if (transform.gameObject.tag.Equals("RestaurantInfo"))
                {
                    panel = transform.gameObject;
                }
            }

            // Check panel starts as disabled
            Assert.IsFalse(panel.activeInHierarchy, "Expected UI panel to be disabled before tracking a logo");

            // Call method which is called when an image is tracked
            var trackEvent = GameObject.FindObjectOfType<MyDefaultTrackableEventHandler>();
            trackEvent.showRestaurantInfo();

            // Check panel has been enabled
            Assert.IsTrue(panel.activeInHierarchy, "UI Panel was not enabled after an image track call");

            // Use yield to skip a frame.
            yield return null;
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {

        }
    }
    
}
