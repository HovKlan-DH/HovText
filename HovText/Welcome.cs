/*
##################################################################################################
WELCOME GUIDE
-------------

This is a welcome guide that will be shown to the user the first time the application is launched.
It will not be shown if the "NotificationShown" has been set to "1" in registry.

##################################################################################################
*/

using System;
using System.Text;
using System.Windows.Forms;

namespace HovText
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi");
            sb.Append(@"{\colortbl;\red255\green255\blue255;\red0\green0\blue0;}");
            sb.Append(@"\viewkind4\uc1\pard\sa200\sl276\slmult1 HovText is a clipboard manager that helps you find previously copied texts or images and it will remove any formatting/styling from the text, leaving only the raw unformatted clear-text intact - though with the ability to restore the original formatting, if needed.\line ");
            sb.Append(@"\line ");
            sb.Append(@"Quick steps to get you started:\line ");
            sb.Append(@"\line ");
            sb.Append(@"* Copy some texts and images to populate the clipboard manager\line ");
            sb.Append(@"* Find a previous copied text or image by pressing \fs18\b\cf1\highlight2 ALT \cf0\highlight0\b0\fs24  + \fs18\b\cf1\highlight2 S \cf0\highlight0\b0\fs24 \line ");
            sb.Append(@"* Search for any text (wildcard search) and divide multiple search criterias with space\line ");
            sb.Append(@"* Navigate with arrow keys or mouse scroll and select with \fs18\b\cf1\highlight2 ENTER \cf0\highlight0\b0\fs24  or mouse click\line ");
            sb.Append(@"* When pasting text then it will paste only the clear-text and not the formatted styling\line ");
            sb.Append(@"* Disable application to restore original text formatting (paste again)\line ");
            sb.Append(@"\line ");
            sb.Append(@"HovText is active from now on your system and it will launch automatically at Windows start-up. You can find its green icon visible in the tray area.\line\line ");
            sb.Append(@"Now dig into the ""Settings"" to setup your personal preferences :-)\line ");
            sb.Append(@"}");
            richTextBox1.Rtf = sb.ToString();

            button1.Focus();

            Logging.Log("\"Welcome Guide\" was shown");
        }


        // ###########################################################################################
        // Make the "ESCAPE" key close the window.
        // ###########################################################################################

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true; // Indicate that you've handled the key press
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        // ###########################################################################################
        // Close the form.
        // ###########################################################################################

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // ###########################################################################################
    }
}
