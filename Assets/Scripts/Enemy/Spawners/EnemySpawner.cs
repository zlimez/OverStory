namespace Abyss.Environment.Enemy
{
    public class EnemySpawner : Spawner
    {
        public override bool Setup(object attr)
        {
            if (!base.Setup(attr)) return false;
            EnemyAttr providedAttr = (EnemyAttr)attr;
            _instance.GetComponent<EnemyManager>().attributes = providedAttr;
            return true;
        }
    }
}
