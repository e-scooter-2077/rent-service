using AutoMapper;
using AutoMapper.QueryableExtensions;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Dal.EfCore.Utils;
using EasyDesk.Tools;
using EScooter.RentService.Application.Queries;
using System.Linq;
using System.Threading.Tasks;
using static EScooter.RentService.Application.Queries.GetRent;

namespace EScooter.RentService.Infrastructure.DataAccess.Queries
{
    public class GetRentQueryHandler : RequestHandlerBase<Query, RentSnapshot>
    {
        private readonly RentDbContext _rentDbContext;
        private readonly IMapper _mapper;

        public GetRentQueryHandler(RentDbContext rentDbContext, IMapper mapper)
        {
            _rentDbContext = rentDbContext;
            _mapper = mapper;
        }

        protected override async Task<Response<RentSnapshot>> Handle(Query request)
        {
            return await _rentDbContext.Rents
                .Where(r => r.Id == request.RentId)
                .ProjectTo<RentSnapshot>(_mapper.ConfigurationProvider)
                .FirstOptionAsync()
                .Map(x => x.OrElseNotFound());
        }
    }
}
