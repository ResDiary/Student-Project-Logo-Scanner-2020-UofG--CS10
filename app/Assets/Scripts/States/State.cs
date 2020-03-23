namespace States
{
    /// <summary>
    ///     States keep track of what view the app is currently in and control which views the user can enter next.
    ///     Each state also takes care of loading scenes and assets for each view on entry.
    /// </summary>
    public abstract class State : object
    {
        protected State(StateContext context)
        {
            Context = context;
        }

        // Concrete State must define constructor to set a reference to the context
        public StateContext Context { get; }

        public abstract void OnStateEnter();
        public abstract void OnStateExit();
        public abstract void ShowMainMenu();
        public abstract void ShowHistory();
        public abstract void CameraOff();
        public abstract void ShowRestaurant();

        public void Back()
        {
            Context.SetState(Context.PreviousState);
        }
    }
}