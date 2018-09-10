using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameMenu : MonoBehaviour
    {

        private Slider _volumeSlider;
        // Use this for initialization
        void Start()
        {
            InitVolumeSlider();
        }
    

        void InitVolumeSlider()
        {
            _volumeSlider = transform.GetChild(0).Find("VolumeSlider").gameObject.GetComponent<Slider>();
            if (File.Exists("prefs.json"))
            {
                string saveJson = File.ReadAllText("prefs.json");
                LobbySave save = JsonUtility.FromJson<LobbySave>(saveJson);
                SetupSlider(save.Volume);
            }
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                GameObject menu = transform.GetChild(0).gameObject;
                if (menu.activeInHierarchy)
                    CloseMenu(menu);
                else
                {
                    ShowMenu(menu);
                }
            }
        }

        void ShowMenu(GameObject menu)
        {
            menu.SetActive(true);
            Time.timeScale = 0;
        }

        void CloseMenu(GameObject menu)
        {
            menu.SetActive(false);
            Time.timeScale = 1;
        }

        public void ContinueClick()
        {
            GameObject menu = transform.GetChild(0).gameObject;
            CloseMenu(menu);
        }

        public void ExitClick()
        {
            if (File.Exists("prefs.json"))
            {
                string saveJson = File.ReadAllText("prefs.json");
                LobbySave save = JsonUtility.FromJson<LobbySave>(saveJson);
                save.Volume = GetSliderVal();
                Debug.Log("Volume: " + save.Volume);
                saveJson = JsonUtility.ToJson(save);
                File.WriteAllText("prefs.json", saveJson);
            }
            Application.Quit();
        }

        public void VolumeSliderChanged()
        {
            AudioListener.volume = _volumeSlider.value;
        }

        public void SetupSlider(float val)
        {
            if (_volumeSlider == null)
            {
                InitVolumeSlider();
            }

            _volumeSlider.value = val;
        }

        public float GetSliderVal()
        {
            if (_volumeSlider == null)
            {
                InitVolumeSlider();
            }

            return _volumeSlider.value;
        }
    }
}
