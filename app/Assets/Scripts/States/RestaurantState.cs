using UnityEngine.SceneManagement;

namespace States
{
    public class RestaurantState : State
    {
        public RestaurantState(StateContext context) : base(context)
        {
        }

        public override void CameraOff()
        {
            Context.SetState(new CameraState(Context));
        }

        public override void OnStateEnter()
        {
            SceneManager.LoadScene("RestaurantView");
        }


        public override void OnStateExit()
        {
        }

        public override void ShowHistory()
        {
            throw new InvalidStateChangeException("Restaurant to History");
        }

        public override void ShowMainMenu()
        {
            Context.SetState(new MainMenuState(Context));
        }

        public override void ShowRestaurant()
        {
            throw new InvalidStateChangeException("Already in Restaurant");
        }
    }
}