using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;

namespace Unitic_BE.Repositories
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly ApplicationDbContext _context;
        public UniversityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetAllUniversityNames()
        {
            // Lấy tất cả tên trường đại học từ bảng University
            return await _context.Universities.Select(u => u.Name).ToListAsync();
        }
        public async Task<List<University>> GetAllUniversity()
        {
            return await _context.Universities.ToListAsync();
        }

        public async Task AddUniversity(University university)
        {
            _context.Universities.Add(university);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateUniversityById(string id, UniversityRequest request)
        {
            var university = await _context.Universities.FindAsync(id);

            university.Name = request.Name;
            // Update other properties if needed

            _context.Universities.Update(university);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteUniversityById(string id)
        {
            var university = await _context.Universities.FindAsync(id);
            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();
        }

        public async Task<University?> GetUniversityById(string id)
        {
            return await _context.Universities.FindAsync(id);
        }
        public async Task<University?> GetUniversityByName(string name)
        {
            return await _context.Universities.Select(u => u)
            .FirstOrDefaultAsync(u => u.Name == name);
        }
        public async Task<string> GetLastId()
        {
            var lastUniversity = await _context.Universities.OrderByDescending(u => u.Id).FirstOrDefaultAsync();
            return lastUniversity?.Id;
        }
    }
}
