using UnityEngine.SceneManagement;

namespace States
{
    /// <summary>
    ///     Contains all camera state contexts
    /// </summary>
    public class CameraState : State
    {
        public CameraState(StateContext context) : base(context)
        {
        }

        public override void CameraOff()
        {
            throw new InvalidStateChangeException("Already in camera state");
        }

        public override void OnStateEnter()
        {
            SceneManager.LoadScene("CameraView");
        }

        public override void OnStateExit()
        {
        }

        public override void ShowHistory()
        {
            throw new InvalidStateChangeException("Camera to history");
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