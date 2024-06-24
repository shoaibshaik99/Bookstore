using ModelLayer.Models.AddressModels;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IAddressesRepo
    {
        public int InsertAddress(AddAddressModel address);
        public List<AddressEntity> GetAddressesByUserId(int userId);
        public bool UpdateAddress(UpdateAddressModel address);
        public bool DeleteAddress(int addressId);
    }
}
