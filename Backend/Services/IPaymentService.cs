using ZBooksApi.DTOs;
namespace ZBooksApi.Services;
public interface IPaymentService
{ 
     Task<List<PaymentDto>> GetPaymentsAsync(int userId);
     Task<PaymentDto?> GetPaymentAsync(int id,int userId);
     Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto,int userId);
     Task<List<PaymentDto>> CreatePaymentsAsync(CreateBatchPaymentDto dto, int userId);
     Task<PaymentDto?> UpdatePaymentAsync(int id,UpdatePaymentDto dto); 
     Task<bool> DeletePaymentAsync(int id); }
