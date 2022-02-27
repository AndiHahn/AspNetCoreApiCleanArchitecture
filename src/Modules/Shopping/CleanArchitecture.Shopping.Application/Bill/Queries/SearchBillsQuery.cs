using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Shared.Application.Cqrs;
using CleanArchitecture.Shared.Core.Models;
using CleanArchitecture.Shared.Core.Result;
using CleanArchitecture.Shopping.Core.Interfaces;

namespace CleanArchitecture.Shopping.Application.Bill.Queries
{
    public class SearchBillsQuery : IQuery<PagedResult<BillDto>>
    {
        public SearchBillsQuery(
            Guid currentUserId,
            int pageSize = 10,
            int pageIndex = 0,
            bool includeShared = false,
            string search = null)
        {
            this.CurrentUserId = currentUserId;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.IncludeShared = includeShared;
            this.Search = search;
        }

        public Guid CurrentUserId { get; }

        public int PageSize { get; }

        public int PageIndex { get; }

        public bool IncludeShared { get; }

        public string Search { get; }
    }

    internal class SearchBillsQueryHandler : IQueryHandler<SearchBillsQuery, PagedResult<BillDto>>
    {
        private readonly IBillRepository billRepository;
        private readonly IMapper mapper;

        public SearchBillsQueryHandler(
            IBillRepository billRepository,
            IMapper mapper)
        {
            this.billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResult<BillDto>> Handle(SearchBillsQuery request, CancellationToken cancellationToken)
        {
            var result = await billRepository.SearchBillsAsync(
                request.CurrentUserId,
                request.Search,
                request.PageSize,
                request.PageIndex,
                request.IncludeShared,
                cancellationToken);

            return new PagedResult<BillDto>(
                result.Value!.Select(this.mapper.Map<BillDto>),
                result.TotalCount);
        }
    }
}
