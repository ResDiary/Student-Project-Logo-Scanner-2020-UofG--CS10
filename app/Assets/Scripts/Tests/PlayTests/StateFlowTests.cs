using System;
using NUnit.Framework;
using States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.PlayTests
{
    public class StateFlowTests
    {
        private GameObject _stateMachine;
        private StateContext _context;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = new GameObject();
            _context = _stateMachine.AddComponent<StateContext>();
        }

        [Test]
        public void SetUpIsCreatingAGameObjectForStateMachine_Test()
        {
            // Tests one time setup is executing without error
            Assert.IsNotNull(_stateMachine);
        }

        [Test]
        public void SetUpIsAddingAStateContextToGameObject_Test()
        {
            // Tests one time setup is executing without error
            Assert.IsNotNull(_stateMachine.GetComponent<StateContext>());
        }


        // Helper method to move to CameraView
        public static void NavigateToCameraViewFromMainMenu(StateContext context)
        {
            context.CurrentState.CameraOff();
            Assert.AreEqual(typeof(CameraState), context.CurrentState.GetType());
        }

        // Helper method to move to RestaurantView
        public static void NavigateToRestaurantViewFromMainMenu(StateContext context)
        {
            NavigateToCameraViewFromMainMenu(context);
            context.CurrentState.ShowRestaurant();
            Assert.AreEqual(typeof(RestaurantState), context.CurrentState.GetType());
        }

        // Helper method to move to HistoryView
        public static bool NavigateToHistoryViewFromMainMenu(StateContext context)
        {
            context.CurrentState.ShowHistory();
            Assert.AreEqual(typeof(HistoryState), context.CurrentState.GetType());
            return true;
        }

        [Test]
        public void PossibleToNavigateToCameraStateFromMainMenuState_Test()
        {
            InMainMenu(_context);
            NavigateToCameraViewFromMainMenu(_context);
        }

        [Test]
        public void PossibleToNavigateToRestaurantStateFromMainMenuState_Test()
        {
            InMainMenu(_context);
            NavigateToRestaurantViewFromMainMenu(_context);
        }

        [Test]
        public void PossibleToNavigateToHistoryStateFromMainMenuState_Test()
        {
            InMainMenu(_context);
            NavigateToHistoryViewFromMainMenu(_context);
        }

        // Helper method to assert you are currently in the main menu state
        public static void InMainMenu(StateContext context)
        {
            Assert.AreEqual(typeof(MainMenuState), context.CurrentState.GetType());
        }

        [Test]
        public void AttemptingToGoToRestaurantStateFromMainMenuStateThrowsAnException_Test()
        {
            InMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.ShowRestaurant);
        }

        [Test]
        public void AttemptingToGoToCameraStateFromHistoryStateThrowsAnException_Test()
        {
            NavigateToHistoryViewFromMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.CameraOff);
        }

        [Test]
        public void AttemptingToGoToHistoryStateFromCameraStateThrowsAnException_Test()
        {
            NavigateToCameraViewFromMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.ShowHistory);
        }

        [Test]
        public void AttemptingToGoToHistoryStateFromRestaurantStateThrowsAnException_Test()
        {
            NavigateToRestaurantViewFromMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.ShowHistory);
        }

        [Test]
        public void AttemptingToGoToHistoryStateFromHistoryStateThrowsAnException_Test()
        {
            NavigateToHistoryViewFromMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.ShowHistory);
        }

        [Test]
        public void AttemptingToGoToMainMenuStateFromMainMenuStateThrowsAnException_Test()
        {
            InMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.ShowMainMenu);
        }

        [Test]
        public void AttemptingToGoToCameraStateFromCameraStateThrowsAnException_Test()
        {
            NavigateToCameraViewFromMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.CameraOff);
        }

        [Test]
        public void AttemptingToGoToRestaurantStateFromRestaurantStateThrowsAnException_Test()
        {
            NavigateToRestaurantViewFromMainMenu(_context);
            Assert.Throws<InvalidStateChangeException>(_context.CurrentState.ShowRestaurant);
        }

        [Test]
        public void PossibleToGoFromRestaurantStateToCameraState_Test()
        {
            NavigateToRestaurantViewFromMainMenu(_context);

            _context.CurrentState.CameraOff(); // Try to go to camera state

            Assert.AreEqual(typeof(CameraState), _context.CurrentState.GetType());
        }

        [Test]
        public void PossibleToGoFromRestaurantStateToMainMenuState_Test()
        {
            NavigateToRestaurantViewFromMainMenu(_context);

            _context.CurrentState.ShowMainMenu(); // Try to go to main menu state

            Assert.AreEqual(typeof(MainMenuState), _context.CurrentState.GetType());
        }

        [Test]
        public void PossibleToGoFromHistoryStateToMainMenuState_Test()
        {
            NavigateToHistoryViewFromMainMenu(_context);

            _context.CurrentState.ShowMainMenu(); // Try to go to main menu state

            Assert.AreEqual(typeof(MainMenuState), _context.CurrentState.GetType());
        }

        [Test]
        public void PossibleToGoFromCameraStateToMainMenuState_Test()
        {
            NavigateToCameraViewFromMainMenu(_context);

            _context.CurrentState.ShowMainMenu(); // Try to go to main menu state

            Assert.AreEqual(typeof(MainMenuState), _context.CurrentState.GetType());
        }

        // Testing all constructors are properly inherited in custom exception InvalidStateChangeException
        [Test]
        public void PossibleToThrowInvalidStateChangeExceptionWithoutMessage()
        {
            Assert.Throws<InvalidStateChangeException>(() => throw new InvalidStateChangeException());
        }

        [Test]
        public void PossibleToThrowInvalidStateChangeExceptionWithMessage()
        {
            Assert.Throws<InvalidStateChangeException>(() =>
                throw new InvalidStateChangeException("Testing Exception"));
        }

        [Test]
        public void InvalidStateChangeExceptionWithMessageReturnsCorrectMessage()
        {
            var ex = new InvalidStateChangeException("Testing Exception");
            Assert.AreEqual("Testing Exception", ex.Message);
        }

        [Test]
        public void PossibleToThrowInvalidStateChangeExceptionWithMessageAndInnerException()
        {
            Assert.Throws<InvalidStateChangeException>(() =>
                throw new InvalidStateChangeException("Testing Exception", new Exception()));
        }

        [Test]
        public void InvalidStateChangeExceptionPassesAnInnerExceptionWhenThrown()
        {
            Assert.NotNull(new InvalidStateChangeException("Testing Exception", new Exception())
                .InnerException);
        }

        [Test]
        public void InvalidStateChangeExceptionPassesCorrectInnerExceptionWhenThrown()
        {
            // Try to throw InvalidStateChangeException with an inner exception and check if the inner exception
            // returned after throwing is same as the one passed when throwing and that it returns the correct message
            var innerEx = new Exception();
            var ex = new InvalidStateChangeException("Testing Exception", innerEx);

            if (ex.InnerException != null)
            {
                Assert.AreEqual(innerEx, ex.InnerException, "Exception returned unexpected inner exception");
                Assert.AreEqual("Testing Exception", ex.Message, "Exception returned unexpected message");
            }
            else
            {
                Assert.Fail("Inner Exception was not passed when throwing exception");
            }
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_stateMachine);
        }
    }
}