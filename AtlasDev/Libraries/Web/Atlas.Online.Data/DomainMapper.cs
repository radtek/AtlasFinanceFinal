namespace Atlas.Online.Data
{
  using Atlas.Online.Data.Models.Definitions;
  using Atlas.Online.Data.Models.DTO;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static class DomainMapper
  {
    public static void Map()
    {
      AutoMapper.Mapper.CreateMap<Client, ClientDto>();
      AutoMapper.Mapper.CreateMap<Address, AddressDto>();
      AutoMapper.Mapper.CreateMap<AddressType, AddressTypeDto>();
      AutoMapper.Mapper.CreateMap<Application, ApplicationDto>();
      AutoMapper.Mapper.CreateMap<Affordability, AffordabilityDto>();
      AutoMapper.Mapper.CreateMap<Bank, BankDto>();
      AutoMapper.Mapper.CreateMap<BankAccountType, BankAccountTypeDto>();
      AutoMapper.Mapper.CreateMap<BankDetail, BankDetailDto>();
      AutoMapper.Mapper.CreateMap<BankPeriod, BankPeriodDto>();
      AutoMapper.Mapper.CreateMap<Client, ClientDto>();
      AutoMapper.Mapper.CreateMap<Contact, ContactDto>();
      AutoMapper.Mapper.CreateMap<ContactType, ContactTypeDto>();
      AutoMapper.Mapper.CreateMap<Province, ProvinceDto>();
      AutoMapper.Mapper.CreateMap<SiteSurvey, SiteSurveyDto>();
    }
  }
}
