using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class Tester : MonoBehaviour
    {
        private GameObject _currentTest;
        public GameObject TestDirectionPrefab;
        public GameObject TestDiffPrefab;
        public void TestDirectionClick()
        {
            if(_currentTest)
                Destroy(_currentTest);
            _currentTest = Instantiate(TestDirectionPrefab, GameObject.Find("Canvas").transform);
        }

        public void TestDifferenceClick()
        {
            if (_currentTest)
                Destroy(_currentTest);
            _currentTest = Instantiate(TestDiffPrefab, GameObject.Find("Canvas").transform);
        }
    }
}
