﻿using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Sport.Domain.Repositories
{
    public interface ISportsRepository
    {
        Task<IEnumerable<Sports>> GetAll();
        Task<Sports> GetById(Guid Id);
        Task<Sports> GetById(int Id);

    }
}
