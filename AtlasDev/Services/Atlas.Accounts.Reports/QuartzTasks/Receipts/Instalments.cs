namespace Atlas.Accounts.Reports.QuartzTasks
{
  class Instalments
  {
    public int qty_tot { get; set; }
    public int qty_vap { get; set; }
    public decimal instal_value { get; set; }
    public string branch_num { get; set; }
    public string branch_name { get; set; }
    public string region_code { get; set; }
  }
}
