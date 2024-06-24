using ModelLayer.Models.AddressModels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class AddressesService : IAddressesService
    {
        private readonly IAddressesRepo _addressRepo;

        public AddressesService(IAddressesRepo addressRepository)
        {
            _addressRepo= addressRepository;
        }

        public int InsertAddress(AddAddressModel address)
        {
            return _addressRepo.InsertAddress(address);
        }

        public List<AddressEntity> GetAddressesByUserId(int userId)
        {
            return _addressRepo.GetAddressesByUserId(userId);
        }

        public bool UpdateAddress(UpdateAddressModel address)
        {
            return _addressRepo.UpdateAddress(address);
        }

        public bool DeleteAddress(int addressId)
        {
            return _addressRepo.DeleteAddress(addressId);
        }
    }
}
