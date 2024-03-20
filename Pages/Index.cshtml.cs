using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DatabaseModel;
using kriptaConnection;
using KK.Sdk;

namespace DotnetSDKTest.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly KriptaConnectionHandler _kriptaConnection;
    public List<CustomerInfo>? CustomerInfos { get; set; }

    public IndexModel(ILogger<IndexModel> logger, KriptaConnectionHandler kriptaConnectionHandler)
    {
        _kriptaConnection = kriptaConnectionHandler;
        _logger = logger;

    }

    public async Task OnGetAsync()
    {
        DatabaseHelper.DatabaseConnection test = new();
        CustomerInfos = await test.getDecryptedData("UserInformation",_kriptaConnection);
    }


}
