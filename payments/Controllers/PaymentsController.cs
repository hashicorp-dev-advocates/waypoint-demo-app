using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Payments.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class PaymentsController: ControllerBase {

    private readonly ILogger<PaymentsController> _logger;
    private readonly Database.PaymentsContext _dbContext;

    public PaymentsController(ILogger<PaymentsController> logger, Database.PaymentsContext dbContext) {
      _logger = logger;
      _dbContext = dbContext;
    }

    [HttpPost(Name = "CreatePayment")]
    public PaymentsResponse Post(Database.Payment payment) {
      _logger.LogInformation("Payment received: " + payment.ToString());

      _dbContext.Payments.Add(payment);
      _dbContext.SaveChanges();

      return new PaymentsResponse {
        Id = payment.Id,
      };
    }
  }

  public class PaymentsResponse {
    public int Id { get; set; }
  }
}