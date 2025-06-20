using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Requests;
using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface IUniversityRepository
    {
        Task<List<string>> GetAllUniversityNames();
        Task<List<University>> GetAllUniversity();
        Task AddUniversity(University university);
        Task UpdateUniversityById(string id, UniversityRequest request);
        Task DeleteUniversityById(string id);
        Task<University?> GetUniversityById(string id);
        Task<University?> GetUniversityByName(string name);
        Task<string> GetLastId();
    }
}
