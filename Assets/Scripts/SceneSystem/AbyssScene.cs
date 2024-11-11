namespace Abyss.SceneSystem
{
    public enum AbyssScene
    {
        None,
        Master,
        Lab,
        Room1,
        Room2,
        Room3
    }
}

namespace Abyss
{
    public static class AbyssSettings
    {
        public enum Layers
        {
            Default = 0,
            Player = 3,
            Enemy = 6,
            Ground = 7,
            PostProcessing = 8,
            Lights = 9,
            PickableItem = 10,
            Obstacle = 11,
            Breakable = 12,
            NPC = 13
        }
    }
}