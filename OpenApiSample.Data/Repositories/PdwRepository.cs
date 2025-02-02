using OpenApiSample.Data.Entities;

namespace OpenApiSample.Data.Repositories
{
    public class PdwRepository(AppDbContext context) : BaseRepository<Pdw>(context), IPdwRepository
    {
    }
}
