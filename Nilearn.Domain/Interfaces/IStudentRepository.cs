using Nilearn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student, CancellationToken cancellationToken = default);
        void Update(Student student, CancellationToken cancellationToken = default);
        Task<Student?> GetByUserId(string userId,CancellationToken cancellationToken = default);

    }
}
