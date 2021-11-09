using AutoMapper;
using AutoMapper.QueryableExtensions;
using EasyDesk.CleanArchitecture.Application.Mapping;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Dal.EfCore.Utils;
using EasyDesk.Tools.Options;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using System.Linq;
using System.Threading.Tasks;
using static EasyDesk.Tools.Options.OptionImports;
using static EScooter.RentService.Application.Queries.GetRents;

namespace EScooter.RentService.Infrastructure.DataAccess.Queries
{
    public class GetRentsQueryHandler : PaginatedQueryHandlerBase<Query, RentSnapshot>
    {
        private readonly RentDbContext _rentDbContext;
        private readonly IMapper _mapper;

        public GetRentsQueryHandler(RentDbContext rentDbContext, IMapper mapper)
        {
            _rentDbContext = rentDbContext;
            _mapper = mapper;
        }

        public class RentSnapshotMapping : DirectMapping<RentModel, RentSnapshot>
        {
            public RentSnapshotMapping() : base(src => new RentSnapshot(
                src.Id,
                src.CustomerId,
                src.ScooterId,
                src.RequestTimestamp,
                src.ConfirmationTimestamp == null ? None : new RentConfirmationInfo(src.ConfirmationTimestamp),
                src.CancellationReason == null ? None : new RentCancellationInfo(src.CancellationReason.Value),
                src.StopReason == null ? None : new RentStopInfo(src.StopReason.Value, src.StopTimestamp)))
            {
            }
        }

        protected override async Task<Response<Page<RentSnapshot>>> Handle(Query request)
        {
            return await _rentDbContext.Rents
                .Conditionally(request.CustomerId, id => query => query.Where(r => r.CustomerId == id))
                .Conditionally(request.ScooterId, id => query => query.Where(r => r.ScooterId == id))
                .OrderBy(r => r.RequestTimestamp)
                .ProjectTo<RentSnapshot>(_mapper.ConfigurationProvider)
                .GetPage(request.Pagination);
        }
    }
}
