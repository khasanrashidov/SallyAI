namespace OpenApiSample.Data.Repositories;

public class UserProjectRepository(AppDbContext context) : BaseRepository<UserProject>(context), IUserProjectRepository
{
}
