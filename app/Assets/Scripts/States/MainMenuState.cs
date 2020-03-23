using UnityEngine.SceneManagement;

namespace States
{
    public class MainMenuState : State
    {
        public MainMenuState(StateContext context) : base(context)
        {
        }

        public override void OnStateEnter()
        {
            SceneManager.LoadScene("MainMenuView");
        }

        public override void OnStateExit()
        {
        }

        public override void CameraOff()
        {
            Context.SetState(new CameraState(Context));
        }

        public override void ShowHistory()
        {
            Context.SetState(new HistoryState(Context));
        }

        public override void ShowMainMenu()
        {
            throw new InvalidStateChangeException("Already in Main menu");
        }

        public override void ShowRestaurant()
        {
            throw new InvalidStateChangeException("Main menu to Restaurant");
        }
    }
}