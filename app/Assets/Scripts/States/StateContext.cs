using System.Net;
using UnityEngine;

namespace States
{
    /// <summary>
    ///     A state context keeps track of the current state, the previous state and sets the current state, for back button functionality.
    ///     It is implemented using Singleton pattern. It also handles the DataReceived event and call the ShowRestaurant() method on the 
    ///     current state if the request was successful to try and display the data to the user.
    ///     
    /// </summary>
    public class StateContext : MonoBehaviour
    {
        private static StateContext _instance;

        private StateContext()
        {
        }

        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; } // Used for back functionality

        public static StateContext GetInstance()
        {
            return _instance;
        }

        // Try to go to restaurant view if restaurant data is received
        private void HandleDataReceived(object sender, ResdiaryClient.ResDataReceivedEventArgs args)
        {
            if (args.StatusCode == 200) CurrentState.ShowRestaurant();
        }

        
        private void Awake()
        {
            // Set the initial state to Initializing and call its OnStateExit/Enter methods.
            // Register any event listeners
            State initial = new InitializingState(this);
            SetState(initial);

            SetInstance();


            // Register listener to DataReceived
            ResdiaryClient.DataReceived += HandleDataReceived;
        }


        public void SetInstance()
        {
            // Set a static reference to self so that other scripts can reference it on load
            if (_instance != null) Debug.Log("Instance to StateContext has been assigned twice");

            _instance = this;
        }

        /// <summary>
        /// Set current state and call states on enter and exit method
        /// </summary>
        /// <param name="state">State to enter</param>
        public void SetState(State state)
        {
            if (CurrentState != null) CurrentState.OnStateExit();

            PreviousState = CurrentState;
            CurrentState = state;
            gameObject.name = "State - " + state.GetType().Name;

            if (CurrentState != null)
                CurrentState.OnStateEnter();
        }
    }
}