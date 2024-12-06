using UnityEngine;
using toio;
using toio.Simulator;
using toio.Navigation;

namespace toio.tutorial
{
    public class MotionTest02 : MonoBehaviour
    {
        int phase = 0;
        public ConnectType conectType;
        CubeManager cubeManager;

        async void Start()
        {
            cubeManager = new CubeManager();
            await cubeManager.MultiConnect(3);

            //cubeManager.navigators[0].AddBorder(width:80, x1:40, x2:460, y1:40, y2:460);

            cubeManager.cubes[0].TurnLedOn(255, 0, 0, 0);
            cubeManager.cubes[1].TurnLedOn(255, 0, 0, 0);
            cubeManager.cubes[2].TurnLedOn(255, 0, 0, 0);

            foreach (var navigator in cubeManager.navigators)
            {
                navigator.mode = Navigator.Mode.BOIDS_AVOID; //集団制御と衝突回避
                //衝突回避のみ：Navigator.Mode.AVOID
                //集団制御のみ：Navigator.Mode.BOIDS
            }
        }

        void Update()
        {
            foreach (var navigator in cubeManager.syncNavigators)
            {
                if (phase == 0)
                {
                    Debug.Log("--- Phase 0 ---");

                    Movement bot0 = cubeManager.navigators[0].Navi2Target(174, 250).Exec();
                    Movement bot1 = cubeManager.navigators[1].Navi2Target(250, 250).Exec();
                    Movement bot2 = cubeManager.navigators[2].Navi2Target(326, 250).Exec();

                    if (bot0.reached && bot1.reached && bot2.reached)
                    {
                        Debug.Log("Complete Phase 0.");
                        phase = 1;
                    }
                }
                else if (phase == 1)
                {
                    Debug.Log("--- Phase 1 ---");

                    Movement bot0 = cubeManager.handles[0].Rotate2Deg(-90).Exec();
                    Movement bot1 = cubeManager.handles[1].Rotate2Deg(-90).Exec();
                    Movement bot2 = cubeManager.handles[2].Rotate2Deg(-90).Exec();

                    if (bot0.reached && bot1.reached && bot2.reached)
                    {
                        Debug.Log("Complete Phase 1.");
                        phase = 2;
                    }
                }
                else if (phase == 2)
                {
                    Debug.Log("--- Phase 2 ---");

                    Movement bot0 = cubeManager.navigators[0].Navi2Target(174, 280).Exec();
                    Movement bot1 = cubeManager.navigators[1].Navi2Target(250, 250).Exec();
                    Movement bot2 = cubeManager.navigators[2].Navi2Target(350, 300).Exec();

                    if (bot0.reached && bot1.reached && bot2.reached)
                    {
                        Debug.Log("Complete Phase 2.");
                        phase = 3;
                    }
                }
                else if (phase == 3)
                {
                    Debug.Log("--- Phase 3 ---");
                    phase = 4;
                }
            }
        }
    }
}