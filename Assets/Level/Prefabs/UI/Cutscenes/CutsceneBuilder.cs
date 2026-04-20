using UnityEngine;
using System.Collections.Generic;

public class CutsceneBuilder : MonoBehaviour
{
    public Transform shopkeep;
    public SpriteRenderer shopkeepSprite;
    public CutsceneController controller;

    void Start(){
        List<CutsceneEvent> scene = new List<CutsceneEvent>();
        scene.Add(new MoveEvent(1f, shopkeep, new Vector3(0, 1, 11), 2f));
        scene.Add(new FlipEvent(0f, shopkeepSprite, true));
        scene.Add(new FlipEvent(2f, shopkeepSprite, false));
        //call to ink dialogue

        controller.Play(scene);
    }
}
