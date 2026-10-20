namespace ShooterBoss.Core
{
    /// <summary>
    /// Carrega o resultado da partida (vitória ou derrota) da cena Gameplay
    /// pra cena Resultado. É só um campo estático porque é lido uma única vez,
    /// logo após o LoadScene — não precisa de objeto DontDestroyOnLoad nem
    /// PlayerPrefs pra isso.
    /// </summary>
    public static class MatchResult
    {
        public static bool PlayerWon { get; private set; }

        public static void Set(bool playerWon) => PlayerWon = playerWon;
    }
}
