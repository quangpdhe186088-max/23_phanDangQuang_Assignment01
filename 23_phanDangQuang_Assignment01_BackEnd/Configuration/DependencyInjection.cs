using Microsoft.EntityFrameworkCore;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.OData;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddBackendLayers(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FUNewsManagement")
            ?? throw new InvalidOperationException("Connection string 'FUNewsManagement' is missing.");

        services.AddDbContext<FUNewsManagementContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<NewsReadDao>();
        services.AddScoped<INewsReadRepository, NewsReadRepository>();
        services.AddScoped<INewsQueryService, NewsQueryService>();
        services.AddScoped<AccountDao>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<CategoryDao>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<TagDao>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<NewsDao>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<INewsService, NewsService>();

        // The schema is shared; request-specific database objects are never singletons.
        services.AddSingleton(ODataModelProvider.Instance);
        return services;
    }
}
