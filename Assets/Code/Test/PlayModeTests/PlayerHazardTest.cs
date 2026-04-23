using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerHazardTest
{
    Player player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(EchosCry.Scene.Name.Level3, LoadSceneMode.Single);
        //Scene does not finish loading until next frame, also wait for other entities of scene to load
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    [UnityTest]
    public IEnumerator Player_Damaged_By_Hazard()
    {
        float playerHealth = player.Health.CurrentHealth;
        float playerArmor = player.Health.CurrentArmor;
        player.RB.position = new Vector3(-0.09999999f, 1.498506f, 4.5f);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(
            player.Health.CurrentHealth < playerHealth ||
            player.Health.CurrentArmor < playerArmor
        );
    }
}