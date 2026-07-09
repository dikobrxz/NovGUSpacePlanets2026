namespace Tour_Endi_Moon
{
    /// <summary>
    /// Этапы сценария "Луна". Вся игра идёт в одной сцене moon.unity.
    ///
    /// Spawn 1 → Spawn 2 → Spawn 3 → Spawn 2
    /// Intro/Landing (Spawn 1) → Monologue (Spawn 2) → Exploration/Collecting (Spawn 3) → Quiz/End (Spawn 2)
    /// </summary>
    public enum GameState
    {
        Intro,         // Spawn 1: вращающаяся Луна внутри корабля + вступительная озвучка
        Landing,       // Spawn 1: первый монолог о Луне (после интро)
        Monologue,     // Spawn 2: fade-телепорт → второй монолог
        Exploration,   // Spawn 3: fade-телепорт → раскопки лопатой
        Collecting,    // Spawn 3: все артефакты найдены — складываем в ящик
        Return,        // Trigger: все в ящике — fade-телепорт обратно на Spawn 2
        Quiz,          // Spawn 2: финальный квиз на корабле
        End            // Spawn 2: завершение + финальная озвучка
    }

    /// <summary>Типы артефактов из сценария.</summary>
    public enum ArtifactType
    {
        Boot,       // ботинок астронавта
        Metal,      // кусок металла от лунохода
        Notebook    // блокнот с записями
    }
}

