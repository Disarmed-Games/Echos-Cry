using UnityEngine;

public abstract class CutsceneEvent
{
    public float triggerTime;
    public bool triggered = false;

    public CutsceneEvent(float time){
        triggerTime = time;
    }
    public virtual void Execute() {}
    public virtual void UpdateEvent(float currentTime){}
}

public class MoveEvent : CutsceneEvent{
    private Transform target;
    private Vector3 startPos;
    private Vector3 endPos;
    private float duration;
    private bool started = false;

    public MoveEvent(float time, Transform target, Vector3 endPos, float duration) : base(time){
        this.target = target;
        this.endPos = endPos;
        this.duration = duration;
    }
    public override void Execute(){
        startPos = target.position;
        started = true;
    }
    public override void UpdateEvent(float currentTime){
        if(!started) return;

        float t = (currentTime - triggerTime) / duration;
        t = Mathf.Clamp01(t);

        target.position = Vector3.Lerp(startPos, endPos, t);
    }
}
