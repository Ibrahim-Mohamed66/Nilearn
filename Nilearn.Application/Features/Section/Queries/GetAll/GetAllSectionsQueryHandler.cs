using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Section.DTOs;
using Nilearn.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Features.Section.Queries.GetAll
{
    internal sealed class GetAllSectionsQueryHandler : IRequestHandler<GetAllSectionsQuery, Result<List<SectionResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllSectionsQueryHandler> _logger;
        public GetAllSectionsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllSectionsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Result<List<SectionResponse>>> Handle(GetAllSectionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching sections for CourseId: {CourseId}", request.CourseId);

            var sections = await _unitOfWork.SectionRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
            if(!sections.Any())
            {
                _logger.LogWarning("No sections found for CourseId: {CourseId}", request.CourseId);
                return Result<List<SectionResponse>>.SuccessResponse(new List<SectionResponse>(), "No sections found.");
            }

            var response = sections.Select(s => new SectionResponse(s.Id, s.Title, s.Description, s.Order, s.CourseId)).ToList();
            _logger.LogInformation("Successfully fetched {Count} sections for CourseId: {CourseId}", response.Count, request.CourseId);
            return Result<List<SectionResponse>>.SuccessResponse(response, "Sections fetched successfully.");



        }
    }
}
