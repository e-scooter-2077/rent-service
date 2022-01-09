using AutoMapper;
using AutoMapper.QueryableExtensions;
using EasyDesk.CleanArchitecture.Application.Mapping;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Dal.EfCore.Utils;
using EasyDesk.Tools.Options;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using System.Linq;
using System.Threading.Tasks;
using static EScooter.RentService.Application.Queries.GetScooters;

namespace EScooter.RentService.Infrastructure.DataAccess.Queries;

public class GetScootersQueryHandler : PaginatedQueryHandlerBase<Query, ScooterSnapshot>
{
    private readonly RentDbContext _rentDbContext;
    private readonly IMapper _mapper;

    public GetScootersQueryHandler(RentDbContext rentDbContext, IMapper mapper)
    {
        _rentDbContext = rentDbContext;
        _mapper = mapper;
    }

    public class ScooterSnapshotMapping : DirectMapping<ScooterModel, ScooterSnapshot>
    {
        public ScooterSnapshotMapping() : base(src => new(
            src.Id,
            src.IsEnabled,
            src.OngoingRentId.AsOption()))
        {
        }
    }

    protected override async Task<Response<Page<ScooterSnapshot>>> Handle(Query request)
    {
        return await _rentDbContext.Scooters
            .Conditionally(request.Available, a => query => query.Where(s => !s.OngoingRentId.HasValue == a))
            .Conditionally(request.Rentable, r => query => query.Where(s => (s.IsEnabled && !s.IsInStandby && !s.IsOutOfService) == r))
            .OrderBy(s => s.Id)
            .ProjectTo<ScooterSnapshot>(_mapper.ConfigurationProvider)
            .GetPage(request.Pagination);
    }
}
