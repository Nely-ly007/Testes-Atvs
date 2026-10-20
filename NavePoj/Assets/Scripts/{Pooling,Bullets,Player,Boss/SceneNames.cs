namespace ShooterBoss.Core
{
    /// <summary>
    /// Nomes exatos das cenas, pra não espalhar strings mágicas pelo projeto.
    /// Esses nomes TÊM que bater com o nome do arquivo .unity e com o que
    /// está cadastrado em File > Build Settings > Scenes In Build.
    /// </summary>
    public static class SceneNames
    {
        public const string Boot = "_Boot";
        public const string Gameplay = "Gameplay";
        public const string GUI = "GUI";
        public const string Resultado = "Resultado";
    }
}
