namespace Eugene_Ambilight.Enums
{
    /// <summary>
    /// Перечисления для установки скорости
    /// </summary>
    public enum Speed
    {
        /// <summary>
        /// Автоматическая скорость
        /// </summary>
        Auto = -1,

        /// <summary>
        /// Моментальная - 0 миллисекунд
        /// </summary>
        Momental,

        /// <summary>
        /// Очень медленная - 50 миллисекунд
        /// </summary>
        VeryFast = 50,

        /// <summary>
        /// Быстрая - 100 миллисекунд
        /// </summary>
        Fast = 100,

        /// <summary>
        /// Обычная - 200 миллисекунд
        /// </summary>
        Normal = 200,

        /// <summary>
        /// Медленная - 300 миллисекунд
        /// </summary>
        Slow = 300,

        /// <summary>
        /// Очень медленная - 600 миллисекунд
        /// </summary>
        VerySlow = 600,

        /// <summary>
        /// Мега медленная - 1.5 секунды
        /// </summary>
        MegaSlow = 1500
    }
}
