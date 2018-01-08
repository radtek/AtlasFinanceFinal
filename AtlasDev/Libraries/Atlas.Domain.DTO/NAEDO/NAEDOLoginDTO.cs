using System;

namespace Atlas.Domain.DTO
{
    public class NAEDOLoginDTO
    {
        public Int64 NAEDOLoginId { get; set; }
        public int MerchantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime DeletedDT { get; set; }
    }
}
