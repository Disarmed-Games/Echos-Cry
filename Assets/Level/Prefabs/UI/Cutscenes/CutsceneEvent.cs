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
public class FlipEvent : CutsceneEvent{
    private Transform target;
    private bool faceLeft;

    public FlipEvent(float time, Transform target, bool faceLeft) : base(time){
        this.target = target;
        this.faceLeft = faceLeft;
    }

    public override void Execute(){
        if(target == null) return;

        Vector3 scale = target.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceLeft ? -1 : 1);
        target.localScale = scale;
    }
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
        if(!started){
            return;
        }

        float t = (currentTime - triggerTime) / duration;
        t = Mathf.Clamp01(t);

        target.position = Vector3.Lerp(startPos, endPos, t);
    }
}

public class SpawnDemonEvent : CutsceneEvent{
    private GameObject demon;
    private GameObject circle;
    private Vector3 spawnPosition;
    private bool hasSpawned = false;

    public SpawnDemonEvent(float time, GameObject demon, GameObject circle, Vector3 spawnPosition) : base(time){
        this.demon = demon;
        this.circle = circle;
        this.spawnPosition = spawnPosition;
    }
    public override void Execute(){
        if (hasSpawned) {return;}
        if(demon != null){
            demon.SetActive(true);
            demon.transform.position = spawnPosition;
        }
        if(circle != null){
            circle.SetActive(true);
            circle.transform.position = spawnPosition;
        }
        hasSpawned = true;
    }
}
