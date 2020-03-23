using System;
using System.Collections;
using NUnit.Framework;
using States;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayTests
{
    public class StateContextPlayTests
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

        [Test]
        public void SameContextPersistsOnStateMachine_Test()
        {
            // Test if the same context instance persists on state machine
            Assert.AreSame(_context, _stateMachine.GetComponent<StateContext>());
        }

        [Test]
        public void StateIsMainMenuAfterInitialization_Test()
        {
            Assert.AreEqual(_context.CurrentState.GetType(), typeof(MainMenuState));
        }
        
        [Test]
        public void SetStateUpdatesCurrentState_Test()
        {
            var oldState = _context.CurrentState;
                
            // Should not be in camera state
            Assert.AreNotEqual(typeof(CameraState), _context.CurrentState.GetType());
            // Move to camera state
            _context.SetState(new CameraState(_context));
            // Check state was changed
            Assert.AreNotEqual(oldState, _context.CurrentState);
        }
        
        [Test]
        public void SetStateToHistoryStateUpdatesCurrentStateToHistoryState_Test()
        {
            // Should not be in HistoryState
            Assert.AreNotEqual(typeof(HistoryState), _context.CurrentState.GetType());
            // Move to camera state
            _context.SetState(new HistoryState(_context));
            Assert.AreEqual(typeof(HistoryState), _context.CurrentState.GetType());
        }
        
        [Test]
        public void SetStateToRestaurantStateUpdatesCurrentStateToRestaurantState_Test()
        {
            // Should not be in RestaurantState
            Assert.AreNotEqual(typeof(RestaurantState), _context.CurrentState.GetType());
            // Move to camera state
            _context.SetState(new RestaurantState(_context));
            
            Assert.AreEqual(typeof(RestaurantState), _context.CurrentState.GetType());
        }

        [Test]
        public void SetStateToCameraStateUpdatesCurrentStateToCameraState_Test()
        {
            // Should not be in camera state
            Assert.AreNotEqual(typeof(CameraState), _context.CurrentState.GetType());
            // Move to camera state
            _context.SetState(new CameraState(_context));
            
            Assert.AreEqual(typeof(CameraState), _context.CurrentState.GetType());
        }
        
        [Test]
        public void SetStateToMainMenuStateUpdatesCurrentStateToMainMenuState_Test()
        {
            // app starts in main menu state so move to history first
            _context.SetState(new HistoryState(_context));
            // Should not be in MainMenuState
            Assert.AreNotEqual(typeof(MainMenuState), _context.CurrentState.GetType());
            // Move to camera state
            _context.SetState(new MainMenuState(_context));
            
            Assert.AreEqual(typeof(MainMenuState), _context.CurrentState.GetType());
        }

    }
}