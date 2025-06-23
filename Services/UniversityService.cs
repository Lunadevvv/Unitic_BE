using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepo;
        private readonly CustomValidator _validator;
        public UniversityService(IUniversityRepository universityRepo, CustomValidator validator)
        {
            _universityRepo = universityRepo;
            _validator = validator;
        }

        public async Task AddUniversity(UniversityRequest university)
        {
            //trim all
            StringTrimmerExtension.TrimAllString(university);
            //check xem universityName tồn tại chưa
            var universityExists = await _universityRepo.GetUniversityByName(university.Name);
            if (universityExists != null)
            {
                throw new UniversityNameAlreadyExistsException(university.Name);
            }
            //validate
            var (erros, isValid) =  _validator.ValidateUniversity(university);
            if (!isValid)
            {
                throw new UpdateAddFailedException(erros);
            }
            University add = new University { Id = await GenerateUniId(), Name = university.Name };
            
            await _universityRepo.AddUniversity(add);
        }

        public async Task DeleteUniversityById(string id)
        {
            var university = await _universityRepo.GetUniversityById(id);
            if (university == null)
            {
                throw new ObjectNotFoundException($"University with id {id}");
            }
            await _universityRepo.DeleteUniversityById(id);
        }

        public Task<List<University>> GetAllUniversity()
        {
            return _universityRepo.GetAllUniversity();
        }

        public Task<List<string>> GetAllUniversityNames()
        {
            return _universityRepo.GetAllUniversityNames();
        }

        public Task<University?> GetUniversityById(string id)
        {
            return _universityRepo.GetUniversityById(id);
        }

        public async Task UpdateUniversityById(string id, UniversityRequest university)
        {
            //trim all
            StringTrimmerExtension.TrimAllString(university);
            var universityExists1 = await _universityRepo.GetUniversityByName(university.Name);
            if (universityExists1 != null)
            {
                throw new UniversityNameAlreadyExistsException(university.Name);
            }
            //check xem universityUpdate có tồn tại ko
            var universityId = await _universityRepo.GetUniversityById(id);
            if (universityId == null)
            {
                throw new ObjectNotFoundException($"University with id {id}");
            }
            //check universityName đó đã tồn tại chưa
            var universityExists = await _universityRepo.GetUniversityByName(university.Name);

            if (universityExists != null && universityExists.Id != universityId.Id)
            {
                throw new UniversityNameAlreadyExistsException(university.Name);
            }
            //validate
            var (erros, isValid) = _validator.ValidateUniversity(university);
            if (!isValid)
            {
                throw new UpdateAddFailedException(erros);
            }
            await _universityRepo.UpdateUniversityById(id, university);
        }
        private async Task<string> GenerateUniId()
        {
            string lastId = await _universityRepo.GetLastId();
            if (lastId == null) return "Uni0001";
            int id = int.Parse(lastId.Substring(3)) + 1; // lấy id cuối cùng và cộng thêm 1
            string generatedId = "Uni" + id.ToString("D4"); //D4 là tự động fill hết 4 số
            return generatedId;
        }
    }
}
