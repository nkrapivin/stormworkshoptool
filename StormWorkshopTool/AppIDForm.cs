using System;
using System.Windows.Forms;

namespace StormWorkshopTool
{
    public partial class AppIDForm : Form
    {
        public uint ChosenAppId { get; private set; } = 0;

        public AppIDForm()
        {
            InitializeComponent();
            Text = Localize.Tr(Text, "AppIDForm.Text");
            SelectLabel.Text = Localize.Tr(SelectLabel.Text, "AppIDForm.SelectLabel.Text");
            CLFullButton.Text = Localize.Tr(CLFullButton.Text, "AppIDForm.CLFullButton.Text");
            CLSandboxButton.Text = Localize.Tr(CLSandboxButton.Text, "AppIDForm.CLSandboxButton.Text");

            // show the expecting culture name
            Text += " (" + Localize.CultureName + ")";
        }

        private void CloseWithAppId(uint appId)
        {
            DialogResult = DialogResult.Yes;
            ChosenAppId = appId;
            Close();
        }

        private void CLFullButton_Click(object sender, EventArgs e)
        {
            CloseWithAppId(2230980);
        }

        private void CLSandboxButton_Click(object sender, EventArgs e)
        {
            CloseWithAppId(1782680);
        }
    }
}
