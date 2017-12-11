using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1
{
    public class Spawners : MonoBehaviour
    {

        private GameObject gm;
        private GameManager gmScript;
        private GameObject roskis;
        private GameObject powerup;
        private bool dirRight;
        private float speed = 1.5f;
        private Vector3 apu;

        void Start()
        {
            gm = GameObject.Find("GameManager");
            gmScript = gm.GetComponent<GameManager>();
        }

        void Update()
        {
            //Roskis spawnaus...
            Debug.Log(gmScript.roskisLife);
            if(gmScript.roskisSpawnable && (!gmScript.pause && !gmScript.freeze))
            {
                int paikka = (int)Random.Range(0f, 24f);
                //Debug.Log(paikka);
                gmScript.roskisLife = 25f;
                roskis = Instantiate(gmScript.roskisPrefab, null, true);
                //Debug.Log(gmScript.roskisSpawnit[paikka].transform.position);
                roskis.transform.position = gmScript.roskisSpawnit[paikka].transform.position;
                //Debug.Log(roskis.transform.position);
                gmScript.roskisSpawnable = false;
                apu = roskis.transform.position;

                int todenak = (int)Random.Range(0f, 4f);
                Debug.Log(todenak);
                if (todenak == 3)
                {
                    //tehdään liikkuva roskis
                    gmScript.movableRoskis = true;
                }
            }
            else if (!gmScript.pause && !gmScript.freeze)
            {
                gmScript.roskisLife -= Time.deltaTime;

                if (gmScript.roskisLife < 0)
                {
                    Destroy(roskis);
                    gmScript.movableRoskis = false;
                    gmScript.roskisSpawnable = true;
                }
            }

            if(gmScript.movableRoskis)
            {
                if(dirRight)
                {
                    roskis.transform.Translate(Vector3.right * speed * Time.deltaTime);
                }
                else
                {
                    roskis.transform.Translate(-Vector3.right * speed * Time.deltaTime);
                }

                if (roskis.transform.position.x >= apu.x + 2f)
                {
                    dirRight = false;
                }

                if (roskis.transform.position.x <= apu.x - 2f)
                {
                    dirRight = true;
                }
            }

            //Powerup spawnaus...
            if(gmScript.powerupSpawnable && (!gmScript.pause && !gmScript.freeze))
            {
                if(gmScript.powerupDelay < 0)
                {
                    int paikka = (int)Random.Range(0f, 24f);

                    gmScript.powerupLife = 25f;
                    powerup = Instantiate(gmScript.powerupPrefab, null, true);
                    powerup.transform.position = gmScript.powerupSpawnit[paikka].transform.position;
                    gmScript.powerupSpawnable = false;
                    gmScript.powerupDeSpawn = 30f;
                }
                else
                {
                    gmScript.powerupDelay -= Time.deltaTime;
                }
            }
            if(!gmScript.powerupSpawnable && (!gmScript.pause && !gmScript.freeze))
            {
                if(gmScript.powerupOn)
                {
                    gmScript.powerupLife -= Time.deltaTime;

                    if(gmScript.powerupLife < 0)
                    {
                        Destroy(powerup);
                        gmScript.powerupOn = false;
                        gmScript.powerupDelay = 45f;
                        gmScript.powerupSpawnable = true;
                    }
                }
                else
                {
                    gmScript.powerupDeSpawn -= Time.deltaTime;

                    if(gmScript.powerupDeSpawn < 0)
                    {
                        Destroy(powerup);
                        gmScript.powerupOn = false;
                        gmScript.powerupDelay = 45f;
                        gmScript.powerupSpawnable = true;
                    }
                }
                
            }
        }
    }
}
