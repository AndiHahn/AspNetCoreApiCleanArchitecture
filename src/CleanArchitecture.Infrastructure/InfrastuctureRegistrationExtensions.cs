﻿using CleanArchitecture.Core.Interfaces.Infrastructure.AzureStorage;
using CleanArchitecture.Core.Interfaces.Infrastructure.Email;
using CleanArchitecture.Core.Interfaces.SqlQueries;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Services.AzureStorage;
using CleanArchitecture.Infrastructure.Services.Email;
using CleanArchitecture.Infrastructure.SqlQueries;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    public static class InfrastuctureRegistrationExtensions
    {
        public static void RegisterInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAzureStorageClientFactory, AzureStorageClientFactory>();
            services.AddScoped<IBlobStorageRepository, BlobStorageRepository>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IBillQueries, BillQueries>();
        }
    }
}