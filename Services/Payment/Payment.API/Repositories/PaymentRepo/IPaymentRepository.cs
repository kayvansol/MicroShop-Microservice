namespace Payment.API.Repositories.PaymentRepo
{
    public interface IPaymentRepository : IRepository<Entities.Payment, int>
    {
        
        Task<Entities.Payment> CreateAsync(Entities.Payment data);
    }
}
