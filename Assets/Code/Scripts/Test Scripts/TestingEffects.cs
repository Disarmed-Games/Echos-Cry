using UnityEngine;
using EchosCry.Combo;

public class TestingEffects : MonoBehaviour
{
    [SerializeField] private EventChannel channel;    

    private bool added = false;
    [SerializeField] private bool adding = false;

    private void Update()
    {
        if(adding && !added)
        {
            channel.Invoke();
            added = true;
        }
    }
}
