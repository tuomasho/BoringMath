using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1
{
    public class PaperScript : MonoBehaviour
    {

        public float ballLife = 10f;
        private GameObject gm;
        private GameManager gmScript;
        private float kerroin = 1f;
        void Start()
        {
            gm = GameObject.Find("GameManager");
            gmScript = gm.GetComponent<GameManager>();
        }

        void FixedUpdate()
        {
            ballLife -= 1 * Time.deltaTime;
            //Debug.Log(ballLife);
            if(ballLife < 0.1)
            {
                Destroy(gameObject);
            }

            if(gmScript.powerupOn)
            {
                kerroin = 2f;
            }
            else
            {
                kerroin = 1f;
            }

        }

        void OnCollisionEnter(Collision col)
        {
            if(col.gameObject.tag == "PointTrigger")
            {
                Debug.Log("You got a point!");

                gmScript.score += 10 * kerroin;
                if(gmScript.movableRoskis)
                {
                    gmScript.score += 20 * kerroin;
                }


                gmScript.thalmicMyo.Vibrate(Thalmic.Myo.VibrationType.Medium);
                Destroy(gameObject);
                gmScript.roskisLife = 0;
            }

            if(col.gameObject.tag == "Powerup")
            {
                gmScript.powerupOn = true;
            }
        }
    }
}
