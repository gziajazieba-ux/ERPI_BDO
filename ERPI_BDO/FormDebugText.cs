using System.Windows.Forms;
using System.Drawing;

namespace ERPI_BDO;

public partial class FormDebugText : Form
{
    public FormDebugText(string title, string text)
    {
        Text = title;
        Width = 1000;
        Height = 700;

        var tb = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9),
            WordWrap = false,
            Text = text
        };

        Controls.Add(tb);
    }
}
