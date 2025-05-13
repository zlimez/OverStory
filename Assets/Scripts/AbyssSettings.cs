namespace Abyss.Settings
{
    public enum Scene
    {
        None,
        MainMenu,
        Master,
        Lab,
        Room1,
        Room2,
        Room3,
    }

    public enum Layer
    {
        Default = 0,
        Player = 3,
        Enemy = 6,
        Ground = 7,
        PickableItem = 10,
        Obstacle = 11,
        Breakable = 12,
        NPC = 13,
        Buildup = 15
    }

    public static class Tag
    {
        public static readonly string Player = "Player";
        public static readonly string Arena = "Arena";
        public static readonly string Enemy = "Enemy";
        public static readonly string Burnable = "Burnable";
        public static readonly string Hookable = "Hookable";
        public static readonly string Movable = "Movable";
        public static readonly string CameraBGTrigger = "CameraBGTrigger";
        public static readonly string WallConstruct = "Wall (Construct)";
        public static readonly string PitConstruct = "Pit (Construct)";
    }

    public static class LayerMask
    {
        public static readonly int OBSTACLE_LMASK = 1 << (int)Layer.Ground | 1 << (int)Layer.Obstacle | 1 << (int)Layer.Buildup;
        public static readonly int GROUND_LMASK = 1 << (int)Layer.Ground | 1 << (int)Layer.Buildup;
    }
}