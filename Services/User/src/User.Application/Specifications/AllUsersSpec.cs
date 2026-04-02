using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Specifications;

namespace User.Application.Specifications;

public class AllUsersSpec : Specification<UserReadModel, UserReadModel>
{
    public AllUsersSpec(int skip, int take) : base(x => 
        new UserReadModel
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email,
            Status = x.Status,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
        })
    {
        ApplyPaging(skip, take);
    }
}
