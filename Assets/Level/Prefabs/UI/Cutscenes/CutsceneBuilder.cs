using UnityEngine;
using System.Collections.Generic;

public class CutsceneBuilder : MonoBehaviour
{
    public Transform shopkeep;
    public Transform demon;
    public GameObject demon2;
    public GameObject demon3;
    public GameObject circle;
    public CutsceneController controller;

    void Start(){
        List<CutsceneEvent> scene = new List<CutsceneEvent>();
        float t = 0f;
        //Shopkeeper is pacing
        //scene.Add(new FlipEvent(t, shopkeep, true));
        scene.Add(new MoveEvent(t, shopkeep, new Vector3(-4,1,11), 1.5f));
        t+= 1.5f;
        scene.Add(new FlipEvent(t, shopkeep, true));
        scene.Add(new MoveEvent(t, shopkeep, new Vector3(-2,1,11), 1.5f));

        t+= 1.5f;

        //Dramatic pause
        t+=1.0f;

        //Shopkeeper looks at entering demon
        //scene.Add(new FlipEvent(t, shopkeep, false));
        t+=0.5f;
        scene.Add(new MoveEvent(t, demon, new Vector3(7,1,11), 1.5f));
        t+=1.5f;
        
        t+=0.5f;
        scene.Add(new MoveEvent(t,demon,new Vector3(5,1,11),1.0f));
        t+=1.0f;
        scene.Add(new FlipEvent(t,shopkeep,false));
        scene.Add(new MoveEvent(t,shopkeep,new Vector3(-22,1,11), 1.2f));
        t+= 1.2f;

        //ending
        scene.Add(new FlipEvent(t,demon,true));

        t+=1.0f;

        scene.Add(new MoveEvent(t,demon,new Vector3(4,1,11), 0.8f));

        scene.Add(new SpawnDemonEvent(t, demon2, circle, new Vector3(-2, 2, 11)));
        t+=1.0f;
        scene.Add(new SpawnDemonEvent(t, demon3, circle, new Vector3(2, 0, 11)));
        t+=1.0f;
        scene.Add(new MoveEvent(t,demon,new Vector3(0,10,11), 0.8f));
        scene.Add(new MoveEvent(t,demon2.transform,new Vector3(0,10,11), 0.8f));
        scene.Add(new MoveEvent(t,demon3.transform,new Vector3(0,10,11), 0.8f));
        controller.Play(scene);
    }
}
