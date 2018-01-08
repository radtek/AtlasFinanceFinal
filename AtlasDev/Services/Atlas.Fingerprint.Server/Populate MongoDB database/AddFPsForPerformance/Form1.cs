using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using global::MongoDB.Driver.Linq;
using global::MongoDB.Driver.Builders;
using Atlas.WCF.FPServer.MongoDB.Entities;

namespace AddFPsForPerformance
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var connectionString = ConfigurationManager.AppSettings["FPDatabase"];
      var client = new global::MongoDB.Driver.MongoClient(connectionString);
      var database = client.GetServer().GetDatabase("fingerprint");
      var fpAllBitmaps = database.GetCollection<FPBitmap>("fpBitmap");
      var inc = 1;
      foreach (var bitmapDoc in fpAllBitmaps.AsQueryable())
      {
        // Convert RAW bytes to grayscale bitmap
        var normal = FPBufferUtils.BufferInvert(bitmapDoc.Bitmap);// FPBufferUtils.Rotate90Left(bitmapDoc.Bitmap));        
        var bitmap = FPBufferUtils.BufferToBitmap(normal, FPBufferUtils.SIZE_WIDTH, FPBufferUtils.SIZE_HEIGHT);
        bitmap.Save(string.Format("D:\\temp\\{0}-{1}-{2}.bmp", bitmapDoc.PersonId, bitmapDoc.FingerId, inc++));
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      
    }
  }
}
