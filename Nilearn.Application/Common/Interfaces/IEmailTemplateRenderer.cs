
namespace Nilearn.Application.Common.Interfaces
{
    public interface IEmailTemplateRenderer
    {
        string Render(string templatePath, Dictionary<string, string> values);
    }
}
