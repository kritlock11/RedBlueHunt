using Interfaces;
using UnityEngine;

namespace DefaultNamespace
{
    public class InputController: IOnInit, IOnUpdate
    {
        private UnitController _unitController;
        private Camera _mainCamera;

        public void OnStart()
        {
            _mainCamera = Camera.main;
            _unitController = SceneManager.Instance.UnitController;
        }

        public void OnUpdate()
        {
            Click();
        }


        private void Click()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return;
            var unit = hit.collider.gameObject.GetComponent<BaseUnit>();

            if (unit)
            {
                _unitController.UnitSwitchRole(unit);
            }
        }
    }
}