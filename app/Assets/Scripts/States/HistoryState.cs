using UnityEngine.SceneManagement;

namespace States
{
    /// <summary>
    ///     Contains all history state contexts
    /// </summary>
    public class HistoryState : State
    {
        public HistoryState(StateContext context) : base(context)
        {
        }

        public override void CameraOff()
        {
            throw new InvalidStateChangeException("History to Camera");
        }

        public override void OnStateEnter()
        {
            SceneManager.LoadScene("HistoryView");
        }

        public override void OnStateExit()
        {
        }

        public override void ShowHistory()
        {
            throw new InvalidStateChangeException("Already in History");
        }

        public override void ShowMainMenu()
        {
            Context.SetState(new MainMenuState(Context));
        }

        public override void ShowRestaurant()
        {
            Context.SetState(new RestaurantState(Context));
        }
    }
}