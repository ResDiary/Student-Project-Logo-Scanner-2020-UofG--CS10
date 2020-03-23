using NUnit.Framework;
using States;
using UnityEngine;

namespace Tests.EditorTests
{
    public class StateContextTests
    {
        private GameObject _stateMachine;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateMachine = new GameObject();
            var context = _stateMachine.AddComponent<StateContext>();
            context.SetInstance();
        }


        [Test]
        public void SetUpIsCreatingAGameObjectForStateMachine_Test()
        {
            // Tests one time setup is executing without error
            Assert.IsNotNull(_stateMachine);
        }

        [Test]
        public void SetUpIsAddingStateContextToGameObject_Test()
        {
            // Tests one time setup is executing without error
            Assert.IsNotNull(_stateMachine.GetComponent<StateContext>());
        }

        [Test]
        public void StateContextCanBeAttachedToAGameObject_Test()
        {
            // create a new GameObject and try attaching a State Context
            var gameObject = new GameObject();
            gameObject.AddComponent<StateContext>();

            Assert.IsNotNull(gameObject.GetComponent<StateContext>());
        }

        [Test]
        public void SetInstanceSetsAReferenceToTheCorrectInstance_Test()
        {
            // Create a fresh GameObject, attach a StateContext and try to set instance
            var gameObject = new GameObject();
            var context = gameObject.AddComponent<StateContext>();
            context.SetInstance();

            Assert.AreEqual(context, StateContext.GetInstance());
        }

        [Test]
        public void StateContextGetInstanceReturnsAValue_Test()
        {
            var context = StateContext.GetInstance();

            Assert.IsNotNull(context);
        }

        [Test]
        public void StateContextReturnsAnInstanceOfItself_Test()
        {
            var context = StateContext.GetInstance();

            Assert.IsTrue(context.GetType() == typeof(StateContext));
        }
        
    }
}