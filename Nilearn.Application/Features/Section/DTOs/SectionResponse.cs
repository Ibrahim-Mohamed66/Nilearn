
namespace Nilearn.Application.Features.Section.DTOs;

public  record SectionResponse(
       int Id,
       string Title,
       string? Description,
       int Order,
       int CourseId);