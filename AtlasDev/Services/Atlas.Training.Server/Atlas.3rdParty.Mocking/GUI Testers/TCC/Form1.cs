using System;
using System.Windows.Forms;



namespace TCCTest
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      try
      {
        var client = new TermRC.TermRCSoapClient();
        client.TermStatusCheck("11111", "1111");
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
    }

  }
}
