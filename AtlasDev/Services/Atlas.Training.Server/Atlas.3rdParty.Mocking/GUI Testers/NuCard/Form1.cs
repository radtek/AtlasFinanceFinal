using Atlas.ThirdParty.XMLRPC.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NuCardTest
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      string xmlSent;
      string xmlRecv;
      string error;
      var card = new Atlas.ThirdParty.XMLRPC.Classes.AllocateCard_Input { cardNumber = "11111", 
        cellPhoneNumber="344343", firstName = "john", idOrPassportNumber = "34543545454353453445", lastName = "Bloggs", profileNumber = "1234324", terminalID ="1223344", 
        transactionDate = DateTime.Now, transactionID = Guid.NewGuid().ToString()};
      var result = NuCardXMLRPCUtils.AllocateCard("http://localhost:9000/", "1111", card, out xmlSent, out xmlRecv, out error);
    }
  }
}
