using Microsoft.AspNetCore.Mvc;
using APBD_Kolokwium1.Models;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly S18295Context _context;

    public PaymentsController(S18295Context context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(int idClient, int idSubscription, decimal paymentAmount)
    {
        var client = await _context.Client.FindAsync(idClient);
        if (client == null)
            return NotFound("Brak Klieta o podanym ID");

        var subscription = await _context.Subscription.FindAsync(idSubscription);
        if (subscription == null)
            return NotFound("Brak subskrypcji o podanym ID");

        if (subscription.EndTime <= DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("Subskrypcja nie jest już aktywna");
        
        var lastPayment = await _context.Payment
            .Where(p => p.IdClient == idClient && p.IdSubscription == idSubscription)
            .OrderByDescending(p => p.Date)
            .FirstOrDefaultAsync();

        if (lastPayment != null && lastPayment.Date.AddMonths(subscription.RenewalPeriod) > DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("Płatność została już dokonana za daną subskrypcje");
        
        var discount = await _context.Discount
            .Where(d => d.IdSubscription == idSubscription && d.DateFrom <= DateOnly.FromDateTime(DateTime.Now) && d.DateTo >= DateOnly.FromDateTime(DateTime.Now))
            .OrderByDescending(d => d.Value)
            .FirstOrDefaultAsync();

        decimal finalAmount = paymentAmount;
        decimal finalWithDiscount = new decimal(0.0d);
        if (discount != null)
            finalWithDiscount = paymentAmount - (paymentAmount * discount.Value / 100);

        if (finalWithDiscount != subscription.Price)
            return BadRequest("Kwota płatności jest zbyt mała!");

        var newPayment = new Payment
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            IdClient = idClient,
            IdSubscription = idSubscription,
        };

        _context.Payment.Add(newPayment);
        await _context.SaveChangesAsync();

        return Ok(new { PaymentId = newPayment.IdPayment });
    }
}
