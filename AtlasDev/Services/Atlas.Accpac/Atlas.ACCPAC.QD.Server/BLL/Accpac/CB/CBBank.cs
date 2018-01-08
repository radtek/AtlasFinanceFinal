namespace BLL.Accpac.CB
{
  public class CBBank
  {
    public CBBank()
    {
    }

     
    public string BankAccountNum { get; set; }
    public string BankName { get; set; }
    public string GLAccountClearing { get; set; }

    
    public string GLAccount_Clearing_New(string branchCode)
    {
      return string.Format("{0}{1}-{2}", GLAccountClearing.Substring(0, 9), branchCode.Trim(), GLAccountClearing.Substring(GLAccountClearing.Length - 1, 1));
    }

  }
}