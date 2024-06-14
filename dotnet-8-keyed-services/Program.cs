var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKeyedSingleton<IAlternativePaymentService, PayPalPaymentService>(AlternativePamynetMethodType.PAYPAL);
builder.Services.AddKeyedSingleton<IAlternativePaymentService, KlarnaPaymentService>(AlternativePamynetMethodType.KLARNA);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/create-payment", async (Payment payment, IServiceProvider serviceProvider) =>
{
    var apmService = serviceProvider.GetKeyedService<IAlternativePaymentService>(payment.Scheme);

    ArgumentNullException.ThrowIfNull(apmService); // For demo purposes

    var paymentId = await apmService.Create(payment.Id);

    return paymentId;
})
.WithName("CreatePayment")
.WithOpenApi();

app.Run();

record Payment(Guid Id, AlternativePamynetMethodType Scheme);

enum AlternativePamynetMethodType 
{
    PAYPAL = 0,
    KLARNA = 1
}

interface IAlternativePaymentService 
{
    Task<Guid> Create(Guid Id);
}

class PayPalPaymentService(ILogger<PayPalPaymentService> logger) : IAlternativePaymentService
{

    public Task<Guid> Create(Guid Id)
    {
        logger.LogInformation("Created payment with PayPal for {paymentId}", Id);

        return Task.FromResult(Id);
    }
}

class KlarnaPaymentService(ILogger<KlarnaPaymentService> logger) : IAlternativePaymentService
{
    public Task<Guid> Create(Guid Id)
    {
        logger.LogInformation("Created payment with Klarna for {paymentId}", Id);

        return Task.FromResult(Id);
    }
}