using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Specifications;

namespace User.Application.Specifications;

public class UserByIdSpec : Specification<UserReadModel, UserReadModel>
{
    public UserByIdSpec(int id) : base(x =>
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
        AddCriteria(u => u.Id == id);
    }
}
