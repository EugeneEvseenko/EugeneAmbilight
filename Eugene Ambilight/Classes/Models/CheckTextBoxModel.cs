using System.Windows.Controls;

namespace Eugene_Ambilight.Classes.Models
{
    /// <summary>
    /// Модель объекта для валидации данных из TextBox-ов и вывода ошибок в Label-ы.
    /// </summary>
    public class CheckTextBoxModel
    {
        /// <summary>
        /// TextBox для валидации.
        /// </summary>
        public TextBox textBox { get; }

        /// <summary>
        /// Label в который будут выводиться ошибки.
        /// </summary>
        public Label errLabel { get; }

        /// <param name="textBox">TextBox для валидации.</param>
        /// <param name="label">Label в который будут выводиться ошибки.</param>
        public CheckTextBoxModel(TextBox textBox, Label label)
        {
            this.textBox = textBox;
            errLabel = label;
        }
    }
}
