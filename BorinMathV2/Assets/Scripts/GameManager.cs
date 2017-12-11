using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

namespace R1
{
    public class GameManager : MonoBehaviour
    {
        public float score;

        //GUI muuttujat
        public Canvas ingameGUI;
        public Canvas pauseGUI;
        public Canvas endGUI;
        public Canvas startGUI;
        public Canvas helpGUI;
        private String lastGUI;

        //teksti muuttujat joita muutellaa pelin ajon aikana
        private Text scoreText;
        private Text powerText;
        private Text loadedText;
        private Text clockText;
        private Text yourPoints;

        //Game object muuttujat
        public GameObject roskisPrefab = null;
        public GameObject pulbettiSpawni = null;
        public GameObject roskisSpawni = null;
        public GameObject powerupSpawni = null;
        public GameObject powerupPrefab = null;
        public GameObject[] roskisSpawnit = new GameObject[24];
        public GameObject[] powerupSpawnit = new GameObject[24];

        //floatit
        public float roskisLife = 45f;
        public float shootables = 0f;
        public float shootForce = 0f;
        private float power = 0;

        public float timeLeft = 300f;
        private float minutes;
        private float seconds;

        public float powerupLife = 25f;
        public float powerupDelay = 30f;
        public float powerupDeSpawn = 30f;
        //booleanit
        public bool powerupSpawnable = true;
        public bool powerupOn = false;
        public bool roskisSpawnable = true;
        public bool freeze = true;
        private bool gameEnd = false;
        public bool pause = false;
        public bool movableRoskis = false;

        //myon vaatimat muuttujat
        public GameObject myo = null;
        public ThalmicMyo thalmicMyo;

        void Start()
        {
            thalmicMyo = myo.GetComponent<ThalmicMyo>();

            startGUI.enabled = true;
            lastGUI = "startGUI";

            //disabloidaan ei halutut GUIt
            ingameGUI.enabled = false;
            helpGUI.enabled = false;
            endGUI.enabled = false;
            pauseGUI.enabled = false;
            
            //alustetaan muuttujat
            score = 0f;
            
            //Etsitään teksti kentät
            scoreText = GameObject.Find("Points").GetComponent<Text>();
            powerText = GameObject.Find("Power").GetComponent<Text>();
            loadedText = GameObject.Find("Loaded").GetComponent<Text>();
            clockText = GameObject.Find("Clock").GetComponent<Text>();
            yourPoints = GameObject.Find("YourPointsText").GetComponent<Text>();


            //tehdään roskiksen spawnpointit
            int z = 0;
            for(int x = 0; x < 3; x++ )
            {
                for(int y = 0; y < 8; y++)
                {
                    var spawni = Instantiate(roskisSpawni, null, true);
                    spawni.transform.position = new Vector3(roskisSpawni.transform.position.x + -4.5f * y, roskisSpawni.transform.position.y, roskisSpawni.transform.position.z + 6f * x);
                    roskisSpawnit[z] = spawni;
                    z++;
                }
            }

            //tehdään poweruppien spawnpointit
            z = 0;
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    var spawni = Instantiate(powerupSpawni, null, true);
                    spawni.transform.position = new Vector3(powerupSpawni.transform.position.x + -4.5f * y, powerupSpawni.transform.position.y, powerupSpawni.transform.position.z + 6f * x);
                    powerupSpawnit[z] = spawni;
                    z++;
                }
            }

            //luodaan pulpetit luokka huoneeseen
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        y = 1;
                        var spawni = Instantiate(pulbettiSpawni);
                        spawni.transform.position = new Vector3(pulbettiSpawni.transform.position.x + -10f * y, pulbettiSpawni.transform.position.y, pulbettiSpawni.transform.position.z + 10f * x);
                    }
                    else
                    {
                        var spawni = Instantiate(pulbettiSpawni);
                        spawni.transform.position = new Vector3(pulbettiSpawni.transform.position.x + -7.5f * y, pulbettiSpawni.transform.position.y, pulbettiSpawni.transform.position.z + 9f * x);
                    }
                }
            }
        }

        void Update()
        {
            if(!freeze)
            {
                timeLeft -= Time.deltaTime;

                minutes = Mathf.Floor(timeLeft / 60);
                seconds = timeLeft % 60;

                if (seconds > 59)
                {
                    seconds = 59;
                }

                if (minutes < 0)
                {
                    gameEnd = true;
                    freeze = true;
                    minutes = 0;
                    seconds = 0; 
                }

                clockText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }

           if (pause)
            {
                 freeze = true;
                 pauseGUI.enabled = true;
                 lastGUI = "pauseGUI";
                 ingameGUI.enabled = false;
            }
 
           if (gameEnd)
            {
                ingameGUI.enabled = false;
                helpGUI.enabled = false;
                startGUI.enabled = false;
                pauseGUI.enabled = false;

                roskisLife = 0;

                endGUI.enabled = true;

                yourPoints.text = "Your points: " + score;
            }

            //Tehdään heitto voimastat %
            power = ((float)Math.Round(shootForce / 1000,2)) * 100f;
            //Debug.Log("Shootforce: " + (int)shootForce);
            //Debug.Log("power: "+power);
            //Päivitetään UI text elementit
            scoreText.text = "Points: " + score;
            powerText.text = "Power: " + power + "%";
            loadedText.text = "Ammo: " + shootables;
        }


        //
        //ALL BUTTON CLICK FUNCTIONS BELOW THIS
        //
        public void NewGameButtonClicked()
        {
            //disabloidaan ei halutut GUIt
            endGUI.enabled = false;
            pauseGUI.enabled = false;
            startGUI.enabled = false;
            ingameGUI.enabled = true;
            //alustetaan muuttujat
            freeze = false;
            gameEnd = false;
            pause = false;
            score = 0f;
            timeLeft = 300f;
            roskisLife = 25f;
        }

        public void HelpButtonClicked()
        {
            pause = false;
            ingameGUI.enabled = false;
            pauseGUI.enabled = false;
            startGUI.enabled = false;
            helpGUI.enabled = true;
        }

        public void ResumeButtonClicked()
        {
            ingameGUI.enabled = true;
            freeze = false;
            pause = false;
            pauseGUI.enabled = false;
        }

        public void BackButtonClicked()
        {
            helpGUI.enabled = false;

            Debug.Log(lastGUI);
            if (lastGUI == "pauseGUI")
            {
                pauseGUI.enabled = true;
                Debug.Log("pauseGUI enabled");
            }
            else
            {
                startGUI.enabled = true;
                Debug.Log("startGUI enabled");
            }
        }

        public void QuitButtonClicked()
        {
            Application.Quit();
        }

        //
        //ALL BUTTON CLICK FUNCTIONS ABOVE THIS
        //

        public void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
        {
            ThalmicHub hub = ThalmicHub.instance;

            if (hub.lockingPolicy == LockingPolicy.Standard)
            {
                myo.Unlock(UnlockType.Timed);
            }

            myo.NotifyUserAction();
        }
    }
}
