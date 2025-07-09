using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Post.Add
{
    public class AddCommandHandler : ICommandHandler<AddCommand>
    {
        public async Task<Result> Handle(AddCommand request, CancellationToken cancellationToken)
        {
            return Result.SuccessWithMessage("Success!");
        }
    }
}