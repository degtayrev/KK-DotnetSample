using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using kriptaConnection;
using DatabaseHelper;
public class AddCustomerModel : PageModel
{
    private readonly KriptaConnectionHandler _kriptaConnection;
    // [Required]
    [BindProperty]
    public string? Name { get; set; }

    // [Required]
    [BindProperty]
    public string? Address { get; set; }

    [BindProperty]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "Phone Number must be 12 digits.")]
    public string? PhoneNumber { get; set; }

    public AddCustomerModel(KriptaConnectionHandler kriptaConnectionHandler)
    {
        _kriptaConnection = kriptaConnectionHandler;

    }
    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        // Your logic to add data to the database
        Console.WriteLine($"Name: {this.Name}, Address: {this.Address}, Phone Number: {this.PhoneNumber}");
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        DatabaseConnection db = new DatabaseConnection();
        try
        {
            db.AddEncryptedData("UserInformation",new List<string>(){Name,Address,PhoneNumber});
        }catch(Exception e)
        {
            Console.WriteLine(e);
        }
        
        
        // Redirect to the Customer Information page after adding the customer
        return RedirectToPage("/Index");
    }
}