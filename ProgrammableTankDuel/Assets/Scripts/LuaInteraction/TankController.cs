using System;
using System.Collections;
using System.Reflection;
using NLua;
using NLua.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.LuaInteraction
{
    public class TankController : MonoBehaviour
    {
        public string ScriptPath;
        public float AiDelay = 0.3f;
        public GameObject TankLabelPrefab;
        public GameObject HpBarPrefab;
        public float NameShift = 20;
        public float HpBarShift = 10;

        private Lua _luaState;
        private LuaFunction _mainFile;
        private LuaFunction _update;
        private Tank _tank;
        private GameObject _tankLabel;
        private GameObject _hpBar;
        private Text _logger;
        private TextPrinter _printer;
        private bool _player = false;

        Color GetColor()
        {
            try
            {
                Double r = (Double) _luaState["Color.r"];
                Double g = (Double) _luaState["Color.g"];
                Double b = (Double) _luaState["Color.b"];
                return new Color( (float)r, (float) g, (float) b);
            }
            catch
            {
                return Color.black;
            }
        }

        void SetupPrefs()
        {
            // Flag
            //string flagColor = _luaState["Color"] as string;
            Color color = GetColor();
            //Debug.Log("Setting color! " + color);
            _tank.SetTeam(color);

            // Name
            string playerName = _luaState["Name"] as string;
            if (playerName != null)
            {
                _tankLabel = Instantiate(TankLabelPrefab, GameObject.Find("Canvas").transform);
                _tankLabel.GetComponent<Text>().text = playerName;
                _tankLabel.GetComponent<Text>().color = color;
            }
            SetupHpbar();
        }

        void SetupHpbar()
        {
            _hpBar = Instantiate(HpBarPrefab, GameObject.Find("Canvas").transform);
        }

        public static void Print(string msg)
        {
            Debug.Log(msg);
        }

  
        void RegFuncs()
        {
            MethodBase debugBase = MethodBase.GetMethodFromHandle(typeof(TankController).GetMethod("Print").MethodHandle);
            _luaState.RegisterFunction("DebugLog", debugBase);

            MethodBase angleDiff = MethodBase.GetMethodFromHandle(typeof(Mathf).GetMethod("DeltaAngle").MethodHandle);
            _luaState.RegisterFunction("AngleDifference", angleDiff);

            MethodBase closestAngle = MethodBase.GetMethodFromHandle(typeof(Extensions).GetMethod("GetClosestDirection").MethodHandle);
            _luaState.RegisterFunction("GetClosestDirection", closestAngle);

            //GetDirectionCoords
            MethodBase getDirectionCoords = MethodBase.GetMethodFromHandle(typeof(Extensions).GetMethod("GetDirectionCoords").MethodHandle);
            _luaState.RegisterFunction("GetDirectionCoords", getDirectionCoords);

            MethodBase toInt = MethodBase.GetMethodFromHandle(typeof(Extensions).GetMethod("ToInt").MethodHandle);
            _luaState.RegisterFunction("ToInt", toInt);

            MethodBase toVec2 = MethodBase.GetMethodFromHandle(typeof(Extensions).GetMethod("ToVector2").MethodHandle);
            _luaState.RegisterFunction("ToVector2", toVec2);

            MethodBase toDouble = MethodBase.GetMethodFromHandle(typeof(Extensions).GetMethod("ToDouble").MethodHandle);
            _luaState.RegisterFunction("ToDouble", toDouble);

            _luaState["Tank"] = new TankWrapper(_tank, AiDelay);
            _luaState["BattleGround"] = new BattleGround(_tank);



            _luaState["Out"] = new LoggerWrapper(_printer);
            _luaState["Messager"] = new TankMessagerWrapper(GetComponent<TankTextMessager>());
        }

        public void Create()
        {
            _logger = GameObject.Find("Logger").GetComponent<Text>();
            _printer = new TextPrinter(_logger);

            // Initializing variables
            _tank = GetComponent<Tank>();

            if (ScriptPath != "")
            {
                // Initializing LUA
                _luaState = new Lua();
                Debug.Log(Environment.CurrentDirectory);
                Debug.Log("Loading script: " + ScriptPath);
                try
                {
                    _mainFile = _luaState.LoadFile(ScriptPath);
                    _mainFile.Call();

                    SetupPrefs();
                    RegFuncs();
                }
                catch (LuaException ex)
                {
                    Debug.LogError("Exception occured while loading Lua script: " + ex);
                    _printer.Print("Exception occured while loading Lua script: " + ex, Color.red, true, false);
                    _printer.Endl();
                }
            }
            else
            {
                gameObject.AddComponent<ManualTankController>();
                _player = true;
            }
        }

        public void InitAi()
        {
            if (_player)
            {
                SetupHpbar();
                return;
            }
            try
            {
                LuaFunction start = _luaState.GetFunction("Start");
                if (start != null)
                    start.Call();

                _update = _luaState.GetFunction("Update");
                StartCoroutine(AiCaller(AiDelay));
            }
            catch (LuaScriptException ex)
            {
                string name = _luaState["Name"] as string;
                if (name == null)
                    name = "";

                Debug.LogError(name + ": Exception occured while starting Lua script: " + ex);
                _printer.Print(name + ": Exception occured while starting Lua script: " + ex, Color.red, true, false);
                _printer.Endl();
            }
        }

        void UpdateUi()
        {
            if (_tankLabel != null)
            {
                Color color = _tank.GetTeam();
                _tankLabel.GetComponent<Text>().color = color;
                RectTransform rect = _tankLabel.GetComponent<RectTransform>();
                Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                pos.y += NameShift;
                rect.position = pos;
            }
            if (_hpBar != null)
            {
                _hpBar.GetComponent<HpBar>().SetHp(_tank.HP / _tank.MaxHp);
                RectTransform rect = _hpBar.GetComponent<RectTransform>();
                Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                pos.y += HpBarShift;
                rect.position = pos;
            }
        }

        void Update()
        {
            UpdateUi();
        }

        void OnDestroy()
        {
            Destroy(_tankLabel);
            Destroy(_hpBar);
            Dispose();
        }

        public void Dispose()
        {
            if(_luaState != null)
                _luaState.Close();
        }

        IEnumerator AiCaller(float delay)
        {
            while (true)
            {
                //_update.Call();
                try
                {
                    _update.Call();
                }
                catch(LuaScriptException ex)
                {
                    _printer.Print(ex + ". Src: " + ex.Source , Color.red, false, false);
                    _printer.Endl();
                    _tank.SetTeam(Color.white);
                    //StopCoroutine("AiCaller");
                    StopAllCoroutines();
                }
                yield return new WaitForSeconds(delay);
            }
        }

    }
}
