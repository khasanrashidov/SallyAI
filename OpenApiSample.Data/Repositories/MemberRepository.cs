namespace OpenApiSample.Data.Repositories
{
    public class MemberRepository(AppDbContext context) : BaseRepository<Member>(context), IMemberRepository
    {
    }
}
