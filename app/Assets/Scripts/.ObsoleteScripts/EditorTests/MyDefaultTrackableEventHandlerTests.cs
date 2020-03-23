using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

namespace Tests
{
    public class MyDefaultTrackableEventHandlerTests
    {
        MyDefaultTrackableEventHandler[] eventHandlers;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CustomerDemo.unity");



        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator RestaurantInfoPanelIsAssignedCorrectReference_Test()
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

            // Get all MyTrackableEventHandler Components in scene
            eventHandlers = GameObject.FindObjectsOfType<MyDefaultTrackableEventHandler>();

            Assert.IsNotNull(eventHandlers, "No Image Targets in scene");

            foreach (MyDefaultTrackableEventHandler eventHandler in eventHandlers)
            {
                Assert.AreEqual(panel, eventHandler.restaurantInfoPanel, "Wrong reference in: " + eventHandler.name);
            }

            yield return null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {

        }
    }

}
