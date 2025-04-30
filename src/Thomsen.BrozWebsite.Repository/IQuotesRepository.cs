namespace Thomsen.BrozWebsite.Repository;
public interface IQuotesRepository {
    Task<string> CheckAndUpdateScheme();

    Task<IEnumerable<Quote>> GetAllQuotesAsync();

    Task InsertQuoteAsync(Quote quote);
    Task InsertQuotesAsync(IEnumerable<Quote> quote);

    Task UpdateQuoteAsync(Quote quote);
}