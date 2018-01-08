using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Atlas.Common.Utils.Tests
{
  [TestClass()]
  public class IdValidator2Tests
  {
    [TestMethod()]
    public void IsValidSouthAfricanIdTest()
    {
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076082"), IdValidator2.ErrorTypes.None, "A valid ID failed check");

      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("1111111111111"), IdValidator2.ErrorTypes.CharsBad, "Repeating characters not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("71121551076083"), IdValidator2.ErrorTypes.CharsBad, "Bad checksum not detected");

      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076081"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076083"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076084"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076085"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076086"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076087"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076088"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076089"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7012155076089"), IdValidator2.ErrorTypes.FailedChecksum, "Bad checksum not detected");

      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7113155076082"), IdValidator2.ErrorTypes.InvalidDOB, "Bad DOB not detected");
      Assert.AreEqual(IdValidator2.CheckSouthAfricanId("7112155076382"), IdValidator2.ErrorTypes.BadCitizenType, "Bad citizen type not detected");

      var check = new IdValidator2("7112155076082");
      Assert.AreEqual(check.CitizenState, IdValidator2.CitizenTypes.RSACitizen, "Citizen type decode failed");
      Assert.AreEqual(check.DateOfBirth, new System.DateTime(1971, 12, 15), "DOB decode failed");
      Assert.AreEqual(check.Error, IdValidator2.ErrorTypes.None, "A valid ID failed check");
      Assert.AreEqual(check.Gender, IdValidator2.GenderTypes.Male, "Gender decode failed");    
    }
  }
}