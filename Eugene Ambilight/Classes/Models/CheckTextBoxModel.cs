using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Eugene_Ambilight.Classes.Models
{
    public class CheckTextBoxModel
    {
        public TextBox textBox { get; }
        public Label errLabel { get; }
        public CheckTextBoxModel(TextBox textBox, Label label)
        {
            this.textBox = textBox;
            errLabel = label;
        }
    }
}
