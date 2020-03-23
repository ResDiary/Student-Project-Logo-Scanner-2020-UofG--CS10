using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace States
{
    /// <summary>
    ///     Handles all the button functionalities in each view
    /// </summary>
    public class Buttons : MonoBehaviour
    {
        public void ShowCameraButton()
        {
            StateContext.GetInstance().CurrentState.CameraOff();
        }

        public void ShowHistoryButton()
        {
            StateContext.GetInstance().CurrentState.ShowHistory();
        }

        public void ShowMainMenuButton()
        {
            StateContext.GetInstance().CurrentState.ShowMainMenu();
        }

        public void CameraScanDown()
        {
            FindObjectOfType<SimpleCloudHandler>().EnableScan();
        }

        /// <summary>
        ///         Initiate the display of restaurant view
        /// </summary>
        /// <param name="req">Object requesting restaurant data</param>
        public static void DisplayRestaurant(MonoBehaviour req)
        {
            var client = FindObjectOfType<ResdiaryClient>();
            string microsite = EventSystem.current.currentSelectedGameObject.FindComponentInChildWithTag<Text>
                ("Microsite").text;
            print(microsite);
            req.StartCoroutine(client.RequestData(new[] {microsite}, 0, 0));
        }

        public void CameraScanUp()
        {
            FindObjectOfType<SimpleCloudHandler>().DisableScan();
        }

        public void Back()
        {
            StateContext.GetInstance().CurrentState.Back();
        }
    }
}