using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

//TODO:
//Test player movement and dash functionality (without state machines)
//Test state machine functionality for each state
//Test spam prevention system functionality
//Test if player can be damaged
//Some other test ideas

//Other tests outside of player:
//Testing if managers exist (TickManager, TempoConductor, etc)
//Testing wave system functionality

public class Player_Town_Null_Tests
{
    GameObject player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(EchosCry.Scene.Name.Town, LoadSceneMode.Single);
        //Scene does not finish loading until next frame, also wait for other entities of scene to load
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    [Test]
    public void Check_That_Player_GameObject_Is_Not_Null()
    {
        Assert.That(player, Is.Not.Null);
    }
    [Test]
    public void Check_That_Player_GameObject_Has_Player_Component()
    {
        Player component = player.GetComponent<Player>();
        Assert.That(component, Is.Not.Null);
    }
}

public class Player_Level1_Null_Tests
{
    GameObject player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(EchosCry.Scene.Name.Level1, LoadSceneMode.Single);
        //Scene does not finish loading until next frame, also wait for other entities of scene to load
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    [Test]
    public void Check_That_Player_GameObject_Is_Not_Null()
    {
        Assert.That(player, Is.Not.Null);
    }
    [Test]
    public void Check_That_Player_GameObject_Has_Player_Component()
    {
        Player component = player.GetComponent<Player>();
        Assert.That(component, Is.Not.Null);
    }
}

public class Player_Level2_Null_Tests
{
    GameObject player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(EchosCry.Scene.Name.Level2, LoadSceneMode.Single);
        //Scene does not finish loading until next frame, also wait for other entities of scene to load
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    [Test]
    public void Check_That_Player_GameObject_Is_Not_Null()
    {
        Assert.That(player, Is.Not.Null);
    }
    [Test]
    public void Check_That_Player_GameObject_Has_Player_Component()
    {
        Player component = player.GetComponent<Player>();
        Assert.That(component, Is.Not.Null);
    }
}

public class Player_Level3_Null_Tests
{
    GameObject player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(EchosCry.Scene.Name.Level3, LoadSceneMode.Single);
        //Scene does not finish loading until next frame, also wait for other entities of scene to load
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    [Test]
    public void Check_That_Player_GameObject_Is_Not_Null()
    {
        Assert.That(player, Is.Not.Null);
    }
    [Test]
    public void Check_That_Player_GameObject_Has_Player_Component()
    {
        Player component = player.GetComponent<Player>();
        Assert.That(component, Is.Not.Null);
    }
}

public class Player_Movement_Tests
{
    Player player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene(EchosCry.Scene.Name.Town, LoadSceneMode.Single);
        //Scene does not finish loading until next frame, also wait for other entities of scene to load
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    //[UnityTest]
    //public IEnumerator Player_Can_Move_Left_Across_10_Frames()
    //{
    //    Vector3 position = player.transform.position;
    //    for (int i = 0; i < 10; i++)
    //    {
    //        player.Movement.Move(new Vector2(-1, 0));
    //        yield return null;
    //    }
    //    Assert.That(player.transform.position.x, Is.LessThan(position.x));
    //}

    //[UnityTest]
    //public IEnumerator Player_Can_Move_Right_Across_10_Frames()
    //{
    //    Vector3 position = player.transform.position;
    //    for (int i = 0; i < 10; i++)
    //    {
    //        player.Movement.Move(new Vector2(1, 0));
    //        yield return null;
    //    }
    //    Assert.That(player.transform.position.x, Is.GreaterThan(position.x));
    //}

    //[UnityTest]
    //public IEnumerator Player_Can_Move_Up_Across_10_Frames()
    //{
    //    Vector3 position = player.transform.position;
    //    for (int i = 0; i < 10; i++)
    //    {
    //        player.Movement.Move(new Vector2(0, 1));
    //        yield return null;
    //    }
    //    Assert.That(player.transform.position.z, Is.GreaterThan(position.z));
    //}

    //[UnityTest]
    //public IEnumerator Player_Can_Move_Down_Across_10_Frames()
    //{
    //    Vector3 position = player.transform.position;
    //    for (int i = 0; i < 10; i++)
    //    {
    //        player.Movement.Move(new Vector2(0, -1));
    //        yield return null;
    //    }
    //    Assert.That(player.transform.position.z, Is.LessThan(position.z));
    //}

    [UnityTest]
    public IEnumerator Player_Can_Dash_In_Up_Direction()
    {
        Vector3 position = player.transform.position;
        player.RB.linearVelocity = new Vector3(0, 0, 1);
        player.Movement.Dash();
        yield return new WaitForSeconds(1);
        Assert.That(player.transform.position.z, Is.GreaterThan(position.z));
    }
}

public class Player_Collision_Tests
{

}
