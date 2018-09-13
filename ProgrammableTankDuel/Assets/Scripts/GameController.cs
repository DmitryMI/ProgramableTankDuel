using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.LuaInteraction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    [Serializable]
    public struct SpawnPoint
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    public class GameController : MonoBehaviour
    {

        public int MinimumTanks;
        public int MaximumTanks;
        public Text TankNumberBox;
        public ListBox ScriptList;
        public GameObject TankPrefab;
        public HpBar ProgressBar;
        public GameObject ScriptBoxPrefab;
        public GameObject GameOverMessagePrefab;

        private int _number = 2;
        private string[] _scripts;
        private List<int> _colorIndexes = new List<int>();

        void SetupLobby()
        {
            ProgressBar.Setup();
            ProgressBar.SetHp(0);

            if (File.Exists("prefs.json"))
            {
                string saveJson = File.ReadAllText("prefs.json");
                LobbySave save = JsonUtility.FromJson<LobbySave>(saveJson);

                //GameObject.Find("Menu").GetComponent<GameMenu>().SetupSlider(save.Volume);

                GameObject box1 = ScriptList.GetAt(0);
                box1.GetComponent<InputField>().text = save.ScriptPaths[0];
                
                if(save.ColorInds != null)
                    ScriptList.SetColorIndex(0, save.ColorInds[0]);
                   
                GameObject box2 = ScriptList.GetAt(1);
                box2.GetComponent<InputField>().text = save.ScriptPaths[1];
                if (save.ColorInds != null)
                      ScriptList.SetColorIndex(1, save.ColorInds[1]);


                for (int i = 2; i < save.ScriptCount; i++)
                {
                    IncCount();
                    GameObject box = ScriptList.GetAt(i);
                    box.GetComponent<InputField>().text = save.ScriptPaths[i];
                    if (save.ColorInds != null)
                        ScriptList.SetColorIndex(i, save.ColorInds[i]);
                }
            }
        }
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SetupLobby();
        }

        public void IncCount()
        {
            if (_number < MaximumTanks)
                _number++;
            else
                return;
            TankNumberBox.text = _number.ToString();
            ScriptList.Add(ScriptBoxPrefab);
        }

        public void DecCount()
        {
            if (_number > MinimumTanks)
                _number--;
            else
                return;
            TankNumberBox.text = _number.ToString();
            ScriptList.RemoveLast();
        }

        public void StartGame()
        {
            LobbySave save = new LobbySave();
            save.ScriptCount = _number;
            save.ScriptPaths = new string[_number];
            save.ColorInds = new int[_number];

            _scripts = new string[_number];
            for (int i = 0; i < _number; i++)
            {
                GameObject box = ScriptList.GetAt(i);
                _scripts[i] = box.GetComponent<InputField>().text;
                save.ScriptPaths[i] = _scripts[i];
                save.ColorInds[i] = ScriptList.GetColorIndex(i);
            }

            float oldVolume = 1.0f;
            if (File.Exists("prefs.json"))
            {
                string saveJson = File.ReadAllText("prefs.json");
                LobbySave oldSave = JsonUtility.FromJson<LobbySave>(saveJson);
                oldVolume = oldSave.Volume;
            }

            save.Volume = oldVolume;
            string json = JsonUtility.ToJson(save, true);
            File.WriteAllText("prefs.json", json);
            
            StartCoroutine(LoadMainScene());
        }

        private void SetupGame()
        {
            int playerCount = 0;

            GameObject cam = Camera.main.gameObject;
            cam.GetComponent<AutoCam>().enabled = false;
            //cam.transform.GetChild(1).gameObject.GetComponent<AutoCam>().enabled = false;
            List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

            GameObject field = GameObject.Find("Field");

            for (int i = 0; i < _number; i++)
            {
                SpawnPoint point = new SpawnPoint();
                point.Position = Extensions.RandomPointInRect(field.GetComponent<Collider2D>().bounds);
                point.Rotation = new Vector3(0, 0, Random.Range(0, 360));
                //SpawnPoints[i] = point;
                spawnPoints.Add(point);
            }

            GameObject env = GameObject.Find("Environment");
            List<TankController> tankList = new List<TankController>();
            for (int i = 0; i < _number; i++)
            {
                GameObject tank = Instantiate(TankPrefab, spawnPoints[i].Position, Quaternion.Euler(spawnPoints[i].Rotation), env.transform);
                TankController tankScript = tank.GetComponent<TankController>();
                if(_scripts[i] != "")
                    tank.GetComponent<TankController>().ScriptPath = _scripts[i];
                else
                {
                    tank.GetComponent<TankController>().ScriptPath = "";
                    playerCount++;
                    if (playerCount > 1)
                    {
                        GameObject logger = GameObject.Find("Logger");
                        logger.GetComponent<Logger>().Print("There can be maximum one player-controlled tank", Color.red, true, false);
                        return;
                    }
                }
                tankList.Add(tankScript);
                tankScript.Create();

                Color forceColor = ColorPicker.Colors[_colorIndexes[i]];
                if(forceColor != Color.white)
                    tank.GetComponent<Tank>().SetTeam(forceColor);
            }

            foreach (var tank in tankList)
            {
                tank.InitAi();
            }

            cam.GetComponent<AutoCam>().enabled = true;
            cam.GetComponent<AutoCam>().Setup();
            StartCoroutine(CheckGameEndConditions());
        }

        IEnumerator LoadMainScene()
        {
            _colorIndexes.Clear();
            for(int i = 0; i < _number; i++)
                _colorIndexes.Add(ScriptList.GetColorIndex(i));
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            while (!loadingOperation.isDone)
            {
                ProgressBar.SetHp(loadingOperation.progress);
                yield return new WaitForEndOfFrame();
            }
            SetupGame();
        }

        Color[] FindActiveTeams()
        {
            Tank[] tanks = FindObjectsOfType<Tank>();
            List<Color> teams = new List<Color>();
            foreach (var tank in tanks)
            {
                Color color = tank.GetTeam();
                if(color == Color.white)
                    continue;
                bool equal = false;
                foreach (var col in teams)
                {
                    if (col.Equals(color))
                        equal = true;
                }
                if(!equal)
                    teams.Add(color);
            }
            return teams.ToArray();
        }

        IEnumerator CheckGameEndConditions()
        {
            if (SceneManager.GetActiveScene().name != "Main")
                yield return null;

            Color[] activeTeams;
            int teams = 0;
            do
            {
                yield return new WaitForSeconds(1.0f);
                activeTeams = FindActiveTeams();
                teams = activeTeams.Length;
            } while (teams > 1);

            GameObject gameover = Instantiate(GameOverMessagePrefab, Vector3.zero, Quaternion.identity,
                GameObject.Find("Canvas").transform);
            
            gameover.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            if(activeTeams.Length == 1)
                gameover.transform.GetChild(1).gameObject.GetComponent<Image>().color = activeTeams[0];
            else
            {
                gameover.transform.GetChild(1).gameObject.GetComponent<Image>().color = Color.white;
                gameover.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Draw!";
            }

            yield return new WaitForSeconds(5.0f);
            
            /*string loggerText = GameObject.Find("Logger").GetComponent<Logger>().GetText();

            StartCoroutine(QuitToLobby(loggerText));

            AsyncOperation operation = SceneManager.LoadSceneAsync("Lobby");

            while (!operation.isDone)
                yield return null;

            //GameObject.Find("LobbyLogger").GetComponent<Text>().text = loggerText;
            GameObject logger = GameObject.Find("LobbyLogger");
            logger.GetComponent<Logger>().SetText(loggerText);*/

            InitLoadingLobbyScene();

            Destroy(gameObject);
        }

        IEnumerator QuitToLobby(string loggerText)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("Lobby");

            while (!operation.isDone)
                yield return null;

            //GameObject.Find("LobbyLogger").GetComponent<Text>().text = loggerText;
            GameObject logger = GameObject.Find("LobbyLogger");
            logger.GetComponent<Logger>().SetText(loggerText);

            Destroy(gameObject);
        }

        private void InitLoadingLobbyScene()
        {
            string loggerText = GameObject.Find("Logger").GetComponent<Logger>().GetText();

            StartCoroutine(QuitToLobby(loggerText));
        }

        public void RequestQuitGame()
        {
            InitLoadingLobbyScene();
        }

        public void ExitClick()
        {
            Application.Quit();
        }

    }
}
