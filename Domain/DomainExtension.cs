using System;
using Domain.Commands;
using Domain.Database;
using Domain.Event;
using Domain.Helpers;
using Domain.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DomainExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> dbOptionsAction)
        {
            return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpsertCellCommand).Assembly))
                .AddScoped<IParser, Parser>()
                .AddDbContext<SheetsDbContext>(dbOptionsAction)
                .AddSingleton<CellChangedEvent>();
        }
    }
}