﻿using AutoMapper;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Core.Interfaces.Services.BillCategory.Models
{
    public class BillCategoryCreateModel
    {
        public string Name { get; set; }
        public string Color { get; set; }

        public static void ApplyMappingConfiguration(IMapperConfigurationExpression config)
        {
            config.CreateMap<BillCategoryCreateModel, BillCategoryEntity>();
        }
    }
}
