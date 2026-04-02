using BuildingBlocks.Infra.Specifications;

namespace User.Application.Specifications;

public class UserByEmailSpec : Specification<Domain.Models.User>
{
    public UserByEmailSpec(string email) : base()
    {
        AddCriteria(u => u.Email == email);
        EnableTracking();
    }
}
