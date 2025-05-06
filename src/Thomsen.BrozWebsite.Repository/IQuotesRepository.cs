namespace Thomsen.BrozWebsite.Repository;
public interface IQuotesRepository {
    Task CheckAndUpdateScheme();

    Task<Quote> GetQuoteAsync(int id);
    Task<IEnumerable<Quote>> GetAllQuotesAsync();

    Task InsertQuoteAsync(Quote quote);
    Task InsertQuotesAsync(IEnumerable<Quote> quote);

    Task UpdateQuoteAsync(Quote quote);

    Task DeleteQuoteAsync(int id);
}