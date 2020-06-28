using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIView : MonoBehaviour
    {
        [SerializeField] private InputField _unitsAmount;
        [SerializeField] private InputField _terrainAmount;
        [SerializeField] private InputField _radius;
        [SerializeField] private InputField _speed;

        [SerializeField] private Button _createBtn;
        [SerializeField] private Button _deleteBtn;
        [SerializeField] private Button _invertBtn;
        [SerializeField] private Button _exitBtn;


        public void AddBtnListeners(Action<float, float, float, float> create, Action onCreateComplete, Action delete, Action invert, Action exit)
        {
            _createBtn.onClick.AddListener(delegate
            {
                create.Invoke(GetUnitsAmount(), GetTerrainAmount(), GetRadius(), GetSpeed());
                onCreateComplete.Invoke();
            });

            _deleteBtn.onClick.AddListener(delete.Invoke);
            _invertBtn.onClick.AddListener(invert.Invoke);
            _exitBtn.onClick.AddListener(exit.Invoke);
        }

        private float GetUnitsAmount() => float.TryParse(_unitsAmount.text, out var number) ? number : 0;
        private float GetTerrainAmount() => float.TryParse(_terrainAmount.text, out var number) ? number : 0;
        private float GetRadius() => float.TryParse(_radius.text, out var number) ? number : 0;
        private float GetSpeed() => float.TryParse(_speed.text, out var number) ? number : 0;
    }
}
