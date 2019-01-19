using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsAndFog : MonoBehaviour
{
    public Transform playerTrans;
    //public GameObject mainRoomLights;
    public GameObject entryRoomLights;
    public ParticleSystem entryRoomFog;
    bool intheArena = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((intheArena==false) && playerTrans.position.z <= 8)//The player is in the room (only run once)
        {
            intheArena = true;
            entryRoomFog.Stop();
            //mainRoomLights.SetActive(true);
            entryRoomLights.SetActive(false);
        }
    }
}
