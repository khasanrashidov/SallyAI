namespace OpenApiSample.Data.Repositories
{
    public class IdeaRepository(AppDbContext context) : BaseRepository<Idea>(context), IIdeaRepository
    {
    }
}
