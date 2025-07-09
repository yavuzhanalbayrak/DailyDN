using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Post.GetList
{
    public class GetListQueryHandler : IQueryHandler<GetListQuery, GetListQueryResponse>
    {
        public async Task<Result<GetListQueryResponse>> Handle(GetListQuery request, CancellationToken cancellationToken)
        {
            return Result.Success(new GetListQueryResponse());
        }
    }
}