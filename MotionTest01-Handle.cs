using UnityEngine;
using toio.Simulator;

namespace toio.tutorial
{
    public class MotionTest01 : MonoBehaviour
    {
        int phase = 0;
        public ConnectType conectType;
        CubeManager cubeManager;

        async void Start()
        {
            cubeManager = new CubeManager();
            await cubeManager.MultiConnect(5);
            cubeManager.cubes[0].TurnLedOn(255, 0, 0, 0);
            cubeManager.cubes[1].TurnLedOn(255, 0, 0, 0);
            cubeManager.cubes[2].TurnLedOn(255, 0, 0, 0);
            cubeManager.cubes[3].TurnLedOn(255, 0, 0, 0);
            cubeManager.cubes[4].TurnLedOn(255, 0, 0, 0);
        }

        void Update()
        {
            foreach (var handles in cubeManager.syncHandles)
            {
                if (phase == 0)
                {
                    Debug.Log("--- Phase 0 ---");
                    Movement bot0 = cubeManager.handles[0].Move2Target(150, 150).Exec();
                    Movement bot1 = cubeManager.handles[1].Move2Target(200, 200).Exec();
                    Movement bot2 = cubeManager.handles[2].Move2Target(250, 250).Exec();
                    Movement bot3 = cubeManager.handles[3].Move2Target(300, 300).Exec();
                    Movement bot4 = cubeManager.handles[4].Move2Target(350, 350).Exec();

                    if (bot0.reached && bot1.reached && bot2.reached && bot3.reached && bot4.reached)
                    {
                        Debug.Log("Complete Move.");
                        phase = 1;
                    }
                }
                else if (phase == 1)
                {
                    Debug.Log("--- Phase 1 ---");
                    Movement bot0 = cubeManager.handles[0].Rotate2Deg(0).Exec();
                    Movement bot1 = cubeManager.handles[1].Rotate2Deg(0).Exec();
                    Movement bot2 = cubeManager.handles[2].Rotate2Deg(0).Exec();
                    Movement bot3 = cubeManager.handles[3].Rotate2Deg(0).Exec();
                    Movement bot4 = cubeManager.handles[4].Rotate2Deg(0).Exec();

                    if (bot0.reached && bot1.reached && bot2.reached && bot3.reached && bot4.reached)
                    {
                        Debug.Log("Complete Rotate.");
                        phase = 2;
                    }
                }
                else if (phase == 2)
                {
                    Debug.Log("---------- finish ----------");
                    phase = 3;
                }

            }

        }
    }

}