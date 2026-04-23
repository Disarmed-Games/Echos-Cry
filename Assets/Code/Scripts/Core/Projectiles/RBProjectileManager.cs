using System.Collections.Generic;
using UnityEngine;

public class RBProjectileManager : Singleton<RBProjectileManager>
{
    public struct PoolNode
    {
        public RBProjectilePool _pool;
        public int _count;
        public PoolNode(RBProjectilePool handler, int count)
        {
            _pool = handler;
            _count = count;
        }
    }

    private Dictionary<int, PoolNode> projectilePools;

    protected override void OnAwake()
    {
        projectilePools = new Dictionary<int, PoolNode>();
    }
    private void OnEnable()
    {
        SceneManager.OnSceneTransitionEvent += OnSceneTransition;
    }
    private void OnDisable()
    {
        SceneManager.OnSceneTransitionEvent -= OnSceneTransition;
    }
    private void OnSceneTransition()
    {
        projectilePools.Clear();
    }

    public RBProjectilePool RequestPool(GameObject prefab)
    {
        int id = prefab.GetInstanceID();

        if (projectilePools.TryGetValue(id, out PoolNode node))
        {
            node._count++;
            return node._pool;
        }
        else
        {
            RBProjectilePool pool = new GameObject(prefab.name + "//SceneHandler").AddComponent<RBProjectilePool>();
            pool.Init(prefab, 5, 50).ProjectileSpeed = 5f;
            PoolNode newNode = new(pool, 1);
            projectilePools.Add(id, newNode);
            return pool;
        }
            
    }
}
