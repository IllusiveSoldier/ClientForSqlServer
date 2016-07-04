using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClientForSqlServer
{
    public partial class QueryForm : Form
    {
        public QueryForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mf = this.Owner as MainForm;
            if (mf != null)
            {
                mf.SetValue(richTextBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e) => CopyToClipbrd();

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) => richTextBox1.Font = new Font("Microsoft Sans Serif", (int)numericUpDown1.Value);

        public void ShowForm()
        {
            this.Show();
            richTextBox1.Text = Data.stringValueForRTBGetSet;
        }

        private void CopyToClipbrd()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var value in richTextBox1.Lines)
            {
                sb.AppendLine(value);
            }
            Clipboard.SetDataObject(sb.ToString());
            NotifyMessage(10000, "Client4SQL", "Success! :)", ToolTipIcon.Info);
        }

        private void NotifyMessage(int timing, string title, string message, ToolTipIcon icon)
        {
            notifyIcon1.Icon = SystemIcons.Exclamation;
            notifyIcon1.Visible = false;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(timing, title, message, icon);
        }
    }
}
