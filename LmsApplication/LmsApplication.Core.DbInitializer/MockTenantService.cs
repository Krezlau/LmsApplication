using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Tenants;

namespace LmsApplication.Core.DbInitializer;

public class MockTenantService : ITenantProviderService
{ 
    private readonly AppTenantsModel _tenantsModel;
    private readonly string _tenant;
    
    public MockTenantService(AppTenantsModel tenantsModel, string tenant)
    {
        _tenantsModel = tenantsModel;
        _tenant = tenant;
    }
    
    public string GetTenantId()
    {
        return _tenant;
    }

    public AppTenantInfo GetTenantInfo()
    {
        return _tenantsModel.Tenants.FirstOrDefault(x => x.Identifier == _tenant)!;
    }
}