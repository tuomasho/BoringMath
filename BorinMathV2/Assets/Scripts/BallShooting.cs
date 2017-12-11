using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

namespace R1
{
    public class BallShooting : MonoBehaviour
    {
        public GameObject ballspawn = null;
        public GameObject ball = null;
        private GameObject gm;
        private GameManager gmScript;
        // Use this for initialization
        void Start()
        {
            gm = GameObject.Find("GameManager");
            gmScript = gm.GetComponent<GameManager>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {

            if (!gmScript.freeze)
            {
                if ((gmScript.thalmicMyo.pose == Pose.WaveOut || Input.GetKey(KeyCode.Q)) && gmScript.shootables == 0)
                {

                    gmScript.shootables = 1;
                }


                if ((gmScript.thalmicMyo.pose == Pose.Fist || Input.GetKey(KeyCode.E)) && gmScript.shootables != 0)
                //if(thalmicMyo.pose == Pose.Fist)
                {

                    var pallo = Instantiate(ball);
                    pallo.transform.position = ballspawn.transform.position;
                    Debug.Log((ballspawn.transform.forward * gmScript.shootForce).ToString());
                    pallo.GetComponent<Rigidbody>().AddForce(-ballspawn.transform.forward * gmScript.shootForce);
                    gmScript.shootables = 0f;
                    gmScript.shootForce = 0f;
                    /* var pallo = Instantiate(ball, transform.position, transform.rotation);
                     pallo.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, shootForce));*/
                }

                if ((gmScript.thalmicMyo.pose == Pose.WaveIn || Input.GetKey(KeyCode.W)) && gmScript.shootables != 0)
                {
                    if (gmScript.shootForce < 1000)
                        gmScript.shootForce += 5f;
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    gmScript.pause = true;
                }
            }
        }
    }
}
