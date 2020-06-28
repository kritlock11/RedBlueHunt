using System.Collections.Generic;
using Assets.Scripts;
using Interfaces;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance;

        public UnitController UnitController { get; private set; }
        public InputController InputController { get; private set; }
        public BoundsController BoundsController { get; private set; }
        public Spawner Spawner { get; private set; }
        public UIView UIView { get; private set; }


        List<IOnUpdate> updates = new List<IOnUpdate>();


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            Init();
        }

        private void Init()
        {
            Spawner = FindObjectOfType<Spawner>();
            UIView = FindObjectOfType<UIView>();

            UnitController = new UnitController();
            InputController = new InputController();
            BoundsController = new BoundsController(GetComponent<Collider>().bounds);
        }

        private bool _started;
        private void StartControllers()
        {
            if (_started) return;

            InputController.OnStart();

            updates.Add(UnitController);
            updates.Add(InputController);

            _started = true;
        }

        private void Start()
        {
            UIView.AddBtnListeners(
                Spawner.Spawn,
                StartControllers,
                UnitController.DeleteUnits,
                UnitController.MassSwitchRoles,
                () => EditorApplication.isPlaying = false
                );
        }

        private void Update()
        {
            for (var i = 0; i < updates.Count; i++)
            {
                updates[i].OnUpdate();
            }
        }
    }
}