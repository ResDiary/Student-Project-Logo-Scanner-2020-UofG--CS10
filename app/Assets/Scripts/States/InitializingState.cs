namespace States
{
    public class InitializingState : State
    {
        public InitializingState(StateContext context) : base(context)
        {
        }

        public override void OnStateEnter()
        {
            ShowMainMenu();
        }

        public override void OnStateExit()
        {
        }

        public override void CameraOff()
        {
            throw new InvalidStateChangeException("Initializing to Camera");
        }


        public override void ShowHistory()
        {
            throw new InvalidStateChangeException("Initializing to History");
        }

        public override void ShowMainMenu()
        {
            Context.SetState(new MainMenuState(Context));
        }

        public override void ShowRestaurant()
        {
            throw new InvalidStateChangeException("Initializing to Restaurant");
        }
    }
}