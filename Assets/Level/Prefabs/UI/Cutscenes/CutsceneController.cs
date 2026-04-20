using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CutsceneController : MonoBehaviour{
    private List<CutsceneEvent> events = new List<CutsceneEvent>();
    private float currentTime = 0f;
    private bool playing = false;

    public void Play(List<CutsceneEvent> newEvents){
        events = newEvents;
        currentTime = 0f;
        playing = true;
    }
    /*IEnumerator PlayCutscene(List<CutsceneEvent> events){
        float time = 0f;

        while (true){
            time += Time.deltaTime;
            foreach (var e in events){
                if (!e.triggered && time >= e.triggerTime){
                    e.Execute();
                    e.triggered = true;
                }if (time >= e.triggerTime){
                    e.UpdateEvent(time);
                }
            }
            yield return null;
        }
    }*/
    void Update(){
        if(!playing || events == null){
            return;
        }
        currentTime+= Time.deltaTime;
        foreach(var e in events){
            if(!e.triggered && currentTime >= e.triggerTime){
                e.Execute();
                e.triggered = true;
            }
            if(e.triggered){
                e.UpdateEvent(currentTime);
            }
        }
    }
}