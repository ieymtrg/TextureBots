using UnityEngine;
using toio;
using toio.Simulator;
using toio.Navigation;

namespace toio.tutorial
{
    public class TargetPole : MonoBehaviour
    {
        CubeManager cubeManager;
        Stage stage;
        public ConnectType connecctType;

        async void Start()
        {
            cubeManager = new CubeManager();
            await cubeManager.MultiConnect(1);

            // Get stage
            stage = GameObject.FindObjectOfType<Stage>();
        }

        bool reached = false;
        void Update()
        {

            foreach (var handle in cubeManager.syncHandles)
            {
                Movement mv = handle.Move2Target(stage.targetPoleCoord).Exec();

                if (mv.reached && !reached)
                {
                    Debug.Log($"Move2Target({stage.targetPoleCoord}) Reached.");
                    reached = true;
                }
                if (!mv.reached) reached = false;
            }

        }
    }

}