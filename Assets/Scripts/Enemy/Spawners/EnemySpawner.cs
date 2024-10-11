namespace Abyss.Environment.Enemy
{
    public class EnemySpawner : Spawner
    {
        public override bool Spawn(object attr)
        {
            if (!base.Spawn(attr)) return false;
            EnemyAttr providedAttr = (EnemyAttr)attr;
            _instance.GetComponent<EnemyManager>().attributes = providedAttr;
            return true;
        }
    }
}
