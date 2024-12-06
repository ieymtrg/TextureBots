using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Cysharp.Threading.Tasks;
using toio;
using toio.Simulator;

namespace toio.Samples.Sample_DigitalTwin
{
    public class ExampleProgram2 : MonoBehaviour
    {
        public enum Method
        {
            Direct,
            AddForce,
        }

        [Tooltip("Table of local names and corresponding CubeSimulator objects to bind")]
        public DigitalTwinBindingTable bindingTable = new DigitalTwinBindingTable();

        public Mat mat;
        public Text label;

        //CubeManager cubeManager;

        public Method mappingMethod = Method.AddForce;

        public bool logScanned = false;

        internal List<Cube> cubes = new List<Cube>();
        private List<string> connectingNames = new List<string>();

        private float elapsedTime;
        private float timeRecordOrbit = 1f; //per 1s

        void Start()
        {
            var scanner = new CubeScanner(ConnectType.Real);
            scanner.StartScan(OnScan).Forget();

            //cubeManager = new CubeManager();

            string filePath = Application.dataPath + @"\Program\BotPos.txt";
            File.AppendAllText(filePath, "Generate Recorder.\n");
            Debug.Log("Generate Record File.");
        }

        //int state = 0;
        //int cnt_record = 0;
        //int cnt_play = 0;

        public void FixedUpdate()
        {
            if (this.mat == null) return;

            var orbx = new List<int>();
            var orby = new List<int>();
            var orba = new List<int>();

            //var navi0 = cubeManager.navigators[0];

            foreach (var cube in this.cubes)
            {
                if (!this.bindingTable.ContainsKey(cube.localName)) continue;

                var sim = this.bindingTable[cube.localName];
                if (sim == null) continue;

                if (!cube.isConnected || !this.mat.IsUnityCoordInside(cube.x, cube.y)) continue;

                sim.GetComponent<CubeInteraction>().enabled = false;

                var rb = sim.GetComponent<Rigidbody>();
                rb.useGravity = false;

                var pos = this.mat.MatCoord2UnityCoord(cube.x, cube.y);
                var deg = this.mat.MatDeg2UnityDeg(cube.angle);

                //Debug.Log("toio-xxx : " + (cube.x, cube.y));
                //Debug.Log(cube.localName + (cube.x, cube.y));
                //Debug.Log(pos);

                if (cube.isGrounded)
                {
                    elapsedTime += Time.deltaTime;
                    pos.y = 0f;

                    /*
                    if (state == 0)
                    {
                        orbx.Add(cube.x);
                        orby.Add(cube.y);
                        orba.Add(cube.angle);
                        //Debug.Log(orbx[cnt]);
                        //Debug.Log(orby[cnt]);
                        //Debug.Log(orba[cnt]);
                        cnt_record++;
                    }
                    else if (state == 1)
                    {
                        //Movement mv = Cube.Move2Target(orbx[cnt_play], orby[cnt_play]).Exec();
                        //var mv = navi0.Navi2Target(orbx[cnt_play], orby[cnt_play], maxSpd: 50).Exec();

                        if (mv.reached)
                        {
                            cnt_play += 1;
                        }
                    }
                    */

                    if (elapsedTime >= timeRecordOrbit)
                    {
                        elapsedTime = 0f;
                    }

                    if (this.mappingMethod == Method.Direct)
                    {
                        rb.MovePosition(pos);
                        rb.MoveRotation(Quaternion.Euler(0, deg, 0));
                    }
                    else if (this.mappingMethod == Method.AddForce)
                    {
                        var dpos = pos - sim.transform.position;
                        var ddeg = (deg - sim.transform.eulerAngles.y + 540) % 360 - 180;

                        rb.AddForce(dpos / Time.fixedDeltaTime * 4e-3f, ForceMode.Impulse);
                        rb.AddTorque(0, ddeg / Time.fixedDeltaTime * 2e-8f, 0, ForceMode.Impulse);
                    }

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        cube.Move(-20, 20, 70);
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        cube.Move(20, -20, 70);
                    }
                    else if (Input.GetKey(KeyCode.UpArrow))
                    {
                        cube.Move(50, 50, 70);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow))
                    {
                        cube.Move(-50, -50, 70);
                    }

                    string filePath = Application.dataPath + @"\Program\BotPos.txt";
                    File.AppendAllText(filePath, cube.localName + " " + Convert.ToString(cube.x) + "," + Convert.ToString(cube.y) + "," + Convert.ToString(cube.angle) + "\n");
                }
                else
                {
                    pos.y = 0.05f;
                    rb.MovePosition(pos);
                    //state = 1; mode flag
                }
            }
        }

        async void OnScan(BLEPeripheralInterface[] peripherals)
        {
            if (peripherals.Length == 0) return;

            if (logScanned)
            {
                Debug.Log(
                    "Scanned: " + string.Join(", ", peripherals.ToList().ConvertAll(p => p.device_name))
                );
            }

            foreach (var peri in peripherals)
            {
                if (!this.bindingTable.ContainsKey(peri.device_name)) continue;
                if (this.connectingNames.Contains(peri.device_name)) continue;

                // Connecting
                this.connectingNames.Add(peri.device_name);
                var cube = await new CubeConnecter(ConnectType.Real).Connect(peri);
                this.connectingNames.Remove(peri.device_name);
                this.cubes.Add(cube);
            }
        }
    }


    [Serializable]
    public class DigitalTwinBindingTableListExample2 : Dictionary<string, CubeSimulator>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class Pair
        {
            public string localName;
            public CubeSimulator digitalTwin;

            public Pair(string localName, CubeSimulator digitalTwin)
            {
                this.localName = localName;
                this.digitalTwin = digitalTwin;
            }
        }

        [SerializeField]
        private List<Pair> _localNameSimulatorPairList = null;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            foreach (Pair pair in _localNameSimulatorPairList)
            {
                if (ContainsKey(pair.localName))
                {
                    continue;
                }
                Add(pair.localName, pair.digitalTwin);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}