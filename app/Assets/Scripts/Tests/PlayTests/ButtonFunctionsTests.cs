using System.Collections;
using NUnit.Framework;
using States;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Vuforia;

namespace Tests.PlayTests
{
    public class ButtonFunctionsTests
    {
        private GameObject _stateMachine;
        private StateContext _context;
        private Buttons _buttons;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = new GameObject();
            _context = _stateMachine.AddComponent<StateContext>();
            _buttons = _stateMachine.AddComponent<Buttons>(); // button methods have to be attached to a game object
        }
        

        [Test]
        public void ShowCameraButtonChangesStateToCameraState_Test()
        {
            StateFlowTests.InMainMenu(_context); // Start from main menu

            _buttons.ShowCameraButton();
            Assert.AreEqual(typeof(CameraState), _context.CurrentState.GetType());
        }

        [Test]
        public void ShowHistoryButtonChangesStateToHistoryState_Test()
        {
            StateFlowTests.InMainMenu(_context); // Start from main menu

            _buttons.ShowHistoryButton();
            Assert.AreEqual(typeof(HistoryState), _context.CurrentState.GetType());
        }

        [Test]
        public void ShowMainMenuButtonChangesStateToMainMenuState_Test()
        {
            StateFlowTests.NavigateToCameraViewFromMainMenu(_context); // Have to move out of main menu to move back

            _buttons.ShowMainMenuButton();
            Assert.AreEqual(typeof(MainMenuState), _context.CurrentState.GetType());
        }

        [UnityTest]
        public IEnumerator ShowCameraButtonExistsOnMainMenu_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");

            Assert.NotNull(cameraButton);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnlySingleShowCameraButtonExistsOnMainMenu_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectsWithTag("ToCamera");

            Assert.AreEqual(1, cameraButton.Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowCameraButtonGameObjectOnMainMenuHasButtonComponent_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");

            Assert.NotNull(cameraButton.GetComponent<Button>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowCameraButtonOnMainMenuHasRegisteredEventListeners_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");
            var listenerCount = cameraButton.GetComponent<Button>().onClick.GetPersistentEventCount();

            Assert.IsTrue(listenerCount > 0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowHistoryButtonExistsOnMainMenu_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectWithTag("ToHistory");

            Assert.NotNull(cameraButton);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnlySingleShowHistoryButtonExistsOnMainMenu_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectsWithTag("ToHistory");

            Assert.AreEqual(1, cameraButton.Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowHistoryButtonGameObjectOnMainMenuHasButtonComponent_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectWithTag("ToHistory");

            Assert.NotNull(cameraButton.GetComponent<Button>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowHistoryButtonOnMainMenuHasRegisteredEventListeners_Test()
        {
            // use coroutine test because it needs to wait a few frames for scene to load
            StateFlowTests.InMainMenu(_context);
            var cameraButton = GameObject.FindGameObjectWithTag("ToHistory");
            var listenerCount = cameraButton.GetComponent<Button>().onClick.GetPersistentEventCount();

            Assert.IsTrue(listenerCount > 0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BackButtonExistsOnHistoryView_Test()
        {
            yield return SceneManager.LoadSceneAsync("HistoryView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("Backbutton");

            Assert.NotNull(cameraButton);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnlySingleBackButtonExistsOnHistoryView_Test()
        {
            yield return SceneManager.LoadSceneAsync("HistoryView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectsWithTag("Backbutton");

            Assert.AreEqual(1, cameraButton.Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BackButtonButtonGameObjectOnHistoryHasButtonComponent_Test()
        {
            yield return SceneManager.LoadSceneAsync("HistoryView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("Backbutton");

            Assert.NotNull(cameraButton.GetComponent<Button>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator BackButtonOnHistoryHasRegisteredEventListeners_Test()
        {
            yield return SceneManager.LoadSceneAsync("HistoryView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("Backbutton");
            var listenerCount = cameraButton.GetComponent<Button>().onClick.GetPersistentEventCount();

            Assert.IsTrue(listenerCount > 0);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CameraButtonExistsOnRestaurantView_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");

            Assert.NotNull(cameraButton);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnlySingleCameraButtonExistsOnRestaurantView_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectsWithTag("ToCamera");

            Assert.AreEqual(1, cameraButton.Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CameraButtonButtonGameObjectOnRestaurantViewHasButtonComponent_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");

            Assert.NotNull(cameraButton.GetComponent<Button>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator CameraButtonOnRestaurantViewHasRegisteredEventListeners_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");
            var listenerCount = cameraButton.GetComponent<Button>().onClick.GetPersistentEventCount();

            Assert.IsTrue(listenerCount > 0);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator RestaurantMenuButtonExistsOnRestaurantView_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToRestaurantMenu");

            Assert.NotNull(cameraButton);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnlySingleRestaurantMenuButtonExistsOnRestaurantView_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectsWithTag("ToRestaurantMenu");

            Assert.AreEqual(1, cameraButton.Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RestaurantMenuButtonButtonGameObjectOnRestaurantViewHasButtonComponent_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToRestaurantMenu");

            Assert.NotNull(cameraButton.GetComponent<Button>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator RestaurantMenuButtonOnRestaurantViewHasRegisteredEventListeners_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToRestaurantMenu");
            var listenerCount = cameraButton.GetComponent<Button>().onClick.GetPersistentEventCount();

            Assert.IsTrue(listenerCount > 0);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator RestaurantMenuButtonOnRestaurantViewHasAnEventListenerOpenMenuUrl_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToRestaurantMenu");
            var eventListener = cameraButton.GetComponent<Button>().onClick.GetPersistentMethodName(0);
            
            Assert.AreEqual("OpenMenuUrl",eventListener);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CameraButtonOnRestaurantViewHasAnEventListenerShowCameraButton_Test()
        {
            yield return SceneManager.LoadSceneAsync("RestaurantView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");
            var eventListener = cameraButton.GetComponent<Button>().onClick.GetPersistentMethodName(0);
            
            Assert.AreEqual("ShowCameraButton",eventListener);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CameraButtonOnMainMenuViewHasAnEventListenerShowCameraButton_Test()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToCamera");
            var eventListener = cameraButton.GetComponent<Button>().onClick.GetPersistentMethodName(0);
            
            Assert.AreEqual("ShowCameraButton",eventListener);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator HistoryButtonOnMainMenuViewHasAnEventListenerShowHistoryButton_Test()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("ToHistory");
            var eventListener = cameraButton.GetComponent<Button>().onClick.GetPersistentMethodName(0);
            
            Assert.AreEqual("ShowHistoryButton",eventListener);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator BackButtonOnHistoryViewHasAnEventListenerShowMainMenuButton_Test()
        {
            yield return SceneManager.LoadSceneAsync("HistoryView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("Backbutton");
            var eventListener = cameraButton.GetComponent<Button>().onClick.GetPersistentMethodName(0);
            
            Assert.AreEqual("Back",eventListener);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator BackButtonOnCameraViewHasAnEventListenerShowMainMenuButton_Test()
        {
            yield return SceneManager.LoadSceneAsync("CameraView", LoadSceneMode.Single);

            var cameraButton = GameObject.FindGameObjectWithTag("Backbutton");
            var eventListener = cameraButton.GetComponent<Button>().onClick.GetPersistentMethodName(0);
            
            Assert.AreEqual("ShowMainMenuButton",eventListener);
            yield return null;
        }
    }
}