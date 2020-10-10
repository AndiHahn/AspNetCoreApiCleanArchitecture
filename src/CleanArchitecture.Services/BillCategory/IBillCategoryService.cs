﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Services.BillCategory.Models;

namespace CleanArchitecture.Services.BillCategory
{
    public interface IBillCategoryService
    {
        Task<IEnumerable<BillCategoryModel>> GetAllAsync();
        Task<BillCategoryModel> GetByIdAsync(int id);
        Task<BillCategoryModel> CreateAsync(BillCategoryCreateModel createModel);
        Task UpdateAsync(int id, BillCategoryUpdateModel updateModel);
        Task DeleteAsync(int id);
    }
}
