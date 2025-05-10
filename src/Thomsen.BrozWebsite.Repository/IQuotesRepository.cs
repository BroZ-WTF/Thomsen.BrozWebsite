namespace Thomsen.BrozWebsite.Repository;
public interface IQuotesRepository {
    Task<Quote> GetQuoteAsync(int id);
    Task<Quote[]> GetAllQuotesAsync();

    Task InsertQuoteAsync(Quote quote);
    Task InsertQuotesAsync(Quote[] quote);

    Task UpdateQuoteAsync(Quote quote);

    Task DeleteQuoteAsync(int id);
}