namespace APBD_Kolokwium1.Controller;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APBD_Kolokwium1.Models; 
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly S18295Context _context;

    public ClientsController(S18295Context context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClientSubscriptions(int id)
    {
        var client = await _context.Client
            .Where(c => c.IdClient == id)
            .Select(c => new
            {
                firstName = c.FirstName,
                lastName = c.LastName,
                email = c.Email,
                phone = c.Phone,
                subscriptions = _context.Subscription
                    .Where(s => _context.Payment
                        .Where(p => p.IdClient == id)
                        .Select(p => p.IdSubscription)
                        .Contains(s.IdSubscription))
                    .Select(s => new
                    {
                        IdSubscription = s.IdSubscription,
                        Name = s.Name,
                        TotalPaidAmount = _context.Payment
                            .Count(p => p.IdClient == id && p.IdSubscription == s.IdSubscription) * s.Price
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (client == null)
        {
            return NotFound();
        }

        return Ok(client);
    }
}
