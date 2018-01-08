using System;


namespace Atlas.Common.OTP
{
  public class HOTP : OTP
  {
    public HOTP(string secret)
      : base(secret, 6, HashAlgorithm.SHA1)
    {
    }

    public int at(int count)
    {
      return this.GenerateOTP(count);
    }


    public bool Verify(int otp, int counter)
    {
      return (otp == this.at(counter));
    }
  }
}