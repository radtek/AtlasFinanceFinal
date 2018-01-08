using System;

namespace Atlas.Domain.DTO
{
    public class ProductTypeDTO
    {
        public Int64 ProductTypeId { get; set; }
        public string Description { get; set; }
        public string AssemblyName { get; set; }
        public string SearchField1 { get; set; }
        public string SearchField2 { get; set; }
    }
}
